using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bonsai.Harp.Tests
{
    /// <summary>
    /// Provides shared configuration, opt-in checks and measurement reporting to tests that require
    /// a physical Harp device to be connected.
    /// </summary>
    /// <remarks>
    /// A test built on these should skip itself when no port has been configured, so the ordinary
    /// suite runs on a machine without hardware and without any command line option. Any new test
    /// should call one of the require methods below before opening a connection and interacting with
    /// the physical device.
    /// </remarks>
    static class HardwareTestHelper
    {
        /// <summary>
        /// The test category used to select or exclude tests requiring physical hardware.
        /// </summary>
        public const string Category = "Hardware";

        const string PortVariable = "BONSAI_HARP_TEST_PORT";
        const string ResetConsentVariable = "BONSAI_HARP_TEST_ALLOW_RESET";
        const string FirmwareVariable = "BONSAI_HARP_TEST_FIRMWARE";
        const string TestIterationsVariable = "BONSAI_HARP_TEST_ITERATIONS";

        /// <summary>
        /// Returns the serial port of the device under test, or skips the calling test
        /// when no port has been configured.
        /// </summary>
        public static string RequirePort()
        {
            var portName = Environment.GetEnvironmentVariable(PortVariable);
            if (string.IsNullOrEmpty(portName))
            {
                Assert.Inconclusive(
                    $"Set {PortVariable} to the serial port of a connected Harp device to run this test.");
            }

            return portName;
        }

        /// <summary>
        /// Skips the calling test unless the operator has explicitly consented to tests
        /// that reset the device.
        /// </summary>
        /// <remarks>
        /// Resetting is separated from <see cref="RequirePort"/> so that configuring a port
        /// is never sufficient to run a test that changes device state.
        /// </remarks>
        public static void RequireResetConsent()
        {
            var consent = Environment.GetEnvironmentVariable(ResetConsentVariable);
            if (!string.Equals(consent, "1", StringComparison.Ordinal))
            {
                Assert.Inconclusive(
                    $"This test resets the device under test. Set {ResetConsentVariable} to 1 to consent.");
            }
        }

        /// <summary>
        /// Returns the path of the firmware image to use, or skips the calling test when none
        /// has been configured.
        /// </summary>
        /// <remarks>
        /// The variable carries both the configuration and the consent, since a test that
        /// flashes the device has no safe default. The image must match the device, because
        /// the update refuses unsupported firmware unless forced.
        /// </remarks>
        public static string RequireFirmware()
        {
            var path = Environment.GetEnvironmentVariable(FirmwareVariable);
            if (string.IsNullOrEmpty(path))
            {
                Assert.Inconclusive(
                    $"Set {FirmwareVariable} to a firmware image matching the device to run this test. The test may flash the device repeatedly.");
            }
            else if (!File.Exists(path))
            {
                Assert.Inconclusive($"No firmware image was found at '{path}'.");
            }

            return path;
        }

        /// <summary>
        /// Returns the configured number of iterations, or <paramref name="defaultValue"/>
        /// when none has been configured, so a longer run can be requested without a rebuild.
        /// </summary>
        public static int GetTestIterations(int defaultValue)
        {
            var configuredValue = Environment.GetEnvironmentVariable(TestIterationsVariable);
            if (!string.IsNullOrEmpty(configuredValue) &&
                int.TryParse(configuredValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iterations) &&
                iterations > 0)
            {
                return iterations;
            }

            return defaultValue;
        }

        /// <summary>
        /// Opens a connection to the device on the specified port and reads its identity, returning
        /// the exception that prevented a response or <see langword="null"/> when it responded.
        /// </summary>
        /// <remarks>
        /// Reading the identity class is enough to establish that a physical device sitting on a port is
        /// a Harp device. The timeout only has to exceed the time a responding device can take, and the
        /// slowest Harp implementation still replies well under ten milliseconds.
        /// </remarks>
        static async Task<Exception> TryReadWhoAmIAsync(string portName)
        {
            const int ResponseTimeoutMilliseconds = 500;

            try
            {
                using (var device = new AsyncDevice(portName))
                {
                    var response = device.ReadWhoAmIAsync();
                    if (await Task.WhenAny(response, Task.Delay(ResponseTimeoutMilliseconds)) != response)
                    {
                        return new TimeoutException("The device did not respond within the timeout.");
                    }

                    await response;
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// Polls the device on the specified port until it responds and returns the elapsed duration,
        /// or <see langword="null"/> when it never responded within the timeout.
        /// </summary>
        /// <remarks>
        /// This helper is intended to wait for a device application that is still starting, without
        /// the need for a fixed settle delay. Polling is safe here only because the device is known
        /// to be running its application. A device in bootloader mode would be held there by the
        /// polling commands. Use <see cref="CheckDeviceResponseAsync"/> to handle that case instead.
        /// </remarks>
        public static async Task<double?> WaitUntilDeviceRespondsAsync(string portName, int timeoutMilliseconds)
        {
            const int PollIntervalMilliseconds = 50;

            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (await TryReadWhoAmIAsync(portName) == null)
                {
                    return stopwatch.Elapsed.TotalMilliseconds;
                }

                await Task.Delay(PollIntervalMilliseconds);
            }
            while (stopwatch.ElapsedMilliseconds < timeoutMilliseconds);

            return null;
        }

        /// <summary>
        /// Determines whether a Harp device on the specified port responds, waiting quietly before
        /// each check, and returns the exception that prevented a response or <see langword="null"/>
        /// when the device responded.
        /// </summary>
        /// <remarks>
        /// The ATxmega bootloader clears its fall-through timer on every byte it receives and does
        /// not respond to Harp at all, so a host that keeps probing holds the device in the
        /// bootloader rather than detecting it. The quiet wait therefore has to exceed the
        /// fall-through, roughly three seconds, plus the further interval the application needs to
        /// start responding, measured at over a second. The returned exception describes which
        /// condition was found. A refused port clears on its own within about a tenth of a second,
        /// while a silent device may be starting up or still waiting.
        /// </remarks>
        public static async Task<Exception> CheckDeviceResponseAsync(string portName)
        {
            const int QuietMilliseconds = 5000;
            const int MaxResponseChecks = 2;

            Exception lastError = null;
            for (int i = 0; i < MaxResponseChecks; i++)
            {
                await Task.Delay(QuietMilliseconds);
                lastError = await TryReadWhoAmIAsync(portName);
                if (lastError == null)
                {
                    return null;
                }
            }

            return lastError;
        }

        /// <summary>
        /// Writes the distribution of a set of timing samples to the test output.
        /// </summary>
        /// <remarks>
        /// The samples are copied before sorting, so the caller retains acquisition order.
        /// </remarks>
        public static void ReportTimingStats(TestContext context, string label, IReadOnlyList<double> milliseconds)
        {
            if (milliseconds.Count == 0)
            {
                context.WriteLine("{0}: no samples", label);
                return;
            }

            var sorted = milliseconds.OrderBy(sample => sample).ToArray();
            context.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                "{0}: n={1} min={2:F1}ms median={3:F1}ms max={4:F1}ms",
                label,
                sorted.Length,
                sorted[0],
                Median(sorted),
                sorted[sorted.Length - 1]));
        }

        /// <summary>
        /// Writes a failure count and every recorded failure to the test output.
        /// </summary>
        public static void ReportFailures(TestContext context, string label, IReadOnlyList<string> failures, int total)
        {
            context.WriteLine("{0}: {1} of {2}", label, failures.Count, total);
            foreach (var failure in failures)
            {
                context.WriteLine("  {0}", failure);
            }
        }

        /// <summary>
        /// Returns the chain of exception type names, outermost first, so that a wrapped cause stays
        /// visible in a one-line summary.
        /// </summary>
        public static string DescribeExceptionTypes(Exception exception)
        {
            var types = new List<string>();
            for (var current = exception; current != null; current = current.InnerException)
            {
                types.Add(current.GetType().Name);
            }

            return string.Join(" -> ", types);
        }

        static double Median(double[] sorted)
        {
            var middle = sorted.Length / 2;
            return sorted.Length % 2 == 0
                ? (sorted[middle - 1] + sorted[middle]) / 2.0
                : sorted[middle];
        }
    }
}
