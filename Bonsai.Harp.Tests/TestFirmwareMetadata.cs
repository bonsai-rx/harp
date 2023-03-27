using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonsai.Harp.Tests
{
    [TestClass]
    public class TestFirmwareMetadata
    {
        [TestMethod]
        public void ParseAndToString_AreReversible()
        {
            var x = new FirmwareMetadata("Behavior", new HarpVersion(2, 5), new HarpVersion(1, 6), new HarpVersion(1, 2), 0);
            var y = FirmwareMetadata.Parse(x.ToString());
            Assert.IsTrue(x.Equals(y));
        }
    }
}
