using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonsai.Harp.Tests
{
    [TestClass]
    public class TestFirmwareMetadata
    {
        [TestMethod]
        public void ParseAndToString_AreReversible()
        {
            var x = new FirmwareMetadata("Behavior",
                firmwareVersion: new HarpVersion(2, 5, 2),
                protocolVersion: new HarpVersion(1, 9, 0),
                hardwareVersion: new HarpVersion(1, 2),
                assemblyVersion: 0);
            var text = x.ToString();
            var y = FirmwareMetadata.Parse(text);
            Assert.IsTrue(x.Equals(y));
        }

        [TestMethod]
        public void ParseAndToStringPatchFloating_AreReversible()
        {
            var x = new FirmwareMetadata("Behavior",
                firmwareVersion: new HarpVersion(2, 5),
                protocolVersion: new HarpVersion(1, 6),
                hardwareVersion: new HarpVersion(1, 2));
            var text = x.ToString();
            var y = FirmwareMetadata.Parse(text);
            Assert.IsTrue(x.Equals(y));
        }
    }
}
