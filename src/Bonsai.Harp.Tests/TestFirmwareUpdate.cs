using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bonsai.Harp.Tests
{
    /// <summary>
    /// Repeatedly updates the firmware of a physical device and classifies any failures by the stage
    /// they occurred at, so that a change to <see cref="Bootloader.UpdateFirmwareAsync"/> can be
    /// judged against a measured failure rate.
    /// </summary>
    /// <remarks>
    /// This exercises the real call, which matters because the failures are timing dependent and
    /// none of them reproduce through a simplified stand-in.
    /// </remarks>
    [TestClass]
    [DoNotParallelize]
    public class TestFirmwareUpdate
    {
        /// <summary>
        /// The time allowed for the device to start responding again after a successful update, before
        /// the next one begins.
        /// </summary>
        const int ReadyTimeoutMilliseconds = 15000;

        public TestContext TestContext { get; set; }

        /// <summary>
        /// Records a progress value synchronously, so the last stage reached is observable at the
        /// point an exception is caught.
        /// </summary>
        /// <remarks>
        /// <see cref="Progress{T}"/> is unsuitable here because it marshals its callbacks, so the
        /// final value can arrive after the exception has already been handled.
        /// </remarks>
        sealed class StageProgress : IProgress<int>
        {
            const int BootloaderStage = 40;

            public int Stage { get; private set; } = -1;

            public int BootloaderAttempts { get; private set; }

            public void Report(int value)
            {
                if (value == BootloaderStage)
                {
                    BootloaderAttempts++;
                }

                Stage = value;
            }
        }

        static async Task<Exception> TryForceUpdateAsync(string portName, DeviceFirmware firmware)
        {
            try
            {
                await Bootloader.UpdateFirmwareAsync(portName, firmware, forceUpdate: true);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        [TestMethod]
        [TestCategory(HardwareTestHelper.Category)]
        public async Task UpdateFirmware_RepeatedUpdates_ClassifyFailuresByStage()
        {
            var portName = HardwareTestHelper.RequirePort();
            var firmwarePath = HardwareTestHelper.RequireFirmware();
            var iterations = HardwareTestHelper.GetTestIterations(5);
            HardwareTestHelper.RequireResetConsent();

            var firmware = DeviceFirmware.FromFile(firmwarePath);
            TestContext.WriteLine("firmware image: {0}", firmwarePath);

            // Wait until the device responds, to avoid starting with a device still restarting
            if (!(await HardwareTestHelper.WaitUntilDeviceRespondsAsync(portName, ReadyTimeoutMilliseconds)).HasValue)
            {
                Assert.Inconclusive("The device did not respond before the run started.");
            }

            var successMilliseconds = new List<double>();
            var failures = new List<string>();
            var failureStages = new Dictionary<int, int>();
            var failureTypes = new Dictionary<string, int>();
            var recoveryFailures = new List<string>();
            var responseMilliseconds = new List<double>();
            var readyMilliseconds = new List<double>();
            var neverReady = 0;
            var recovered = 0;
            var retried = 0;
            var attempted = 0;

            // Stage 30 means a failure while reopening the port or waiting for the bootloader, and
            // beyond 40 while writing the image, which leaves the device in bootloader mode.
            for (int i = 0; i < iterations; i++)
            {
                attempted = i + 1;
                var progress = new StageProgress();
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await Bootloader.UpdateFirmwareAsync(portName, firmware, progress);
                    var elapsed = stopwatch.Elapsed.TotalMilliseconds;
                    successMilliseconds.Add(elapsed);
                    if (progress.BootloaderAttempts > 1)
                    {
                        retried++;
                        TestContext.WriteLine(
                            "iteration {0}: succeeded after {1} attempts at the bootloader",
                            i,
                            progress.BootloaderAttempts);
                    }

                    // Start the next update from a device that has demonstrably responded, rather
                    // than after a fixed delay, so that every iteration begins in the same state.
                    var ready = await HardwareTestHelper.WaitUntilDeviceRespondsAsync(portName, ReadyTimeoutMilliseconds);
                    if (ready.HasValue)
                    {
                        readyMilliseconds.Add(ready.GetValueOrDefault());
                        TestContext.WriteLine(
                            "iteration {0}: succeeded in {1:F0}ms, responding again after {2:F0}ms",
                            i,
                            elapsed,
                            ready.GetValueOrDefault());
                    }
                    else
                    {
                        neverReady++;
                        TestContext.WriteLine(
                            "iteration {0}: succeeded in {1:F0}ms but never responded within {2}ms",
                            i,
                            elapsed,
                            ReadyTimeoutMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    var failureType = HardwareTestHelper.DescribeExceptionTypes(ex);
                    var description = string.Format(
                        "iteration {0}: reached stage {1} on bootloader attempt {2}: {3}: {4}",
                        i,
                        progress.Stage,
                        progress.BootloaderAttempts,
                        failureType,
                        ex.Message);
                    failures.Add(description);
                    TestContext.WriteLine(description);

                    failureStages.TryGetValue(progress.Stage, out var stageCount);
                    failureStages[progress.Stage] = stageCount + 1;
                    failureTypes.TryGetValue(failureType, out var typeCount);
                    failureTypes[failureType] = typeCount + 1;

                    var responseTimer = Stopwatch.StartNew();
                    var noResponse = await HardwareTestHelper.CheckDeviceResponseAsync(portName);
                    if (noResponse == null)
                    {
                        responseMilliseconds.Add(responseTimer.Elapsed.TotalMilliseconds);
                        TestContext.WriteLine("  device responded again after {0:F0}ms", responseTimer.Elapsed.TotalMilliseconds);
                        continue;
                    }

                    TestContext.WriteLine(
                        "  device did not respond after {0:F0}ms ({1}: {2}), forcing an update to recover it",
                        responseTimer.Elapsed.TotalMilliseconds,
                        noResponse.GetType().Name,
                        noResponse.Message);

                    var recoveryError = await TryForceUpdateAsync(portName, firmware);
                    if (recoveryError == null)
                    {
                        recoveryError = await HardwareTestHelper.CheckDeviceResponseAsync(portName);
                    }

                    if (recoveryError == null)
                    {
                        recovered++;
                        TestContext.WriteLine("  recovered");
                        continue;
                    }

                    recoveryFailures.Add(string.Format(
                        "iteration {0}: reached stage {1} and left the device unresponsive ({2}), recovery failed with {3}: {4}",
                        i,
                        progress.Stage,
                        noResponse.GetType().Name,
                        recoveryError.GetType().Name,
                        recoveryError.Message));
                    TestContext.WriteLine("  recovery failed, abandoning the run");
                    break;
                }
            }

            if (attempted < iterations)
            {
                TestContext.WriteLine("abandoned after {0} of {1} iterations", attempted, iterations);
            }

            TestContext.WriteLine("succeeded: {0} of {1}", successMilliseconds.Count, attempted);
            HardwareTestHelper.ReportTimingStats(TestContext, "successful update duration", successMilliseconds);

            foreach (var stage in failureStages)
            {
                TestContext.WriteLine("failures at stage {0}: {1}", stage.Key, stage.Value);
            }

            foreach (var type in failureTypes)
            {
                TestContext.WriteLine("failures of type {0}: {1}", type.Key, type.Value);
            }

            HardwareTestHelper.ReportTimingStats(TestContext, "time to respond again after a successful update", readyMilliseconds);
            TestContext.WriteLine("successful updates after which the device never responded: {0}", neverReady);
            HardwareTestHelper.ReportTimingStats(TestContext, "time until the device responded again after a failure", responseMilliseconds);
            TestContext.WriteLine("failures that left the device unresponsive: {0}", recovered + recoveryFailures.Count);
            TestContext.WriteLine("recovered by forcing an update: {0}", recovered);
            TestContext.WriteLine("updates that succeeded only after a retry: {0}", retried);
            HardwareTestHelper.ReportFailures(TestContext, "failures", failures, attempted);
            HardwareTestHelper.ReportFailures(TestContext, "recovery failures", recoveryFailures, attempted);

            Assert.IsEmpty(
                recoveryFailures,
                "A firmware update left the device unresponsive and forcing an update did not recover it.");
            Assert.IsEmpty(failures, "At least one firmware update failed. The stage and exception type identify where.");
        }
    }
}
