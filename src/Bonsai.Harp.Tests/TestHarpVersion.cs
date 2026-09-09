using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Bonsai.Harp.Tests
{
    [TestClass]
    public class TestHarpVersion
    {
        static void AssertEquals(HarpVersion a, HarpVersion b)
        {
            Assert.IsTrue(a == b); Assert.IsTrue(b == a);
            Assert.IsFalse(a < b); Assert.IsFalse(b < a);
            Assert.IsFalse(a > b); Assert.IsFalse(b > a);
            AssertOperatorConsistent(a, b); AssertOperatorConsistent(b, a);
            Assert.IsTrue(EqualityComparer<HarpVersion>.Default.Equals(a, b));
            Assert.AreEqual(0, Comparer<HarpVersion>.Default.Compare(a, b));
        }

        static void AssertLessThan(HarpVersion a, HarpVersion b)
        {
            Assert.IsFalse(a == b); Assert.IsFalse(b == a);
            Assert.IsTrue(a < b); Assert.IsFalse(b < a);
            Assert.IsFalse(a > b); Assert.IsTrue(b > a);
            AssertOperatorConsistent(a, b); AssertOperatorConsistent(b, a);
            Assert.IsFalse(EqualityComparer<HarpVersion>.Default.Equals(a, b));
            Assert.IsLessThan(0, Comparer<HarpVersion>.Default.Compare(a, b));
        }

        static void AssertGreaterThan(HarpVersion a, HarpVersion b)
        {
            Assert.IsFalse(a == b); Assert.IsFalse(b == a);
            Assert.IsFalse(a < b); Assert.IsTrue(b < a);
            Assert.IsTrue(a > b); Assert.IsFalse(b > a);
            AssertOperatorConsistent(a, b); AssertOperatorConsistent(b, a);
            Assert.IsFalse(EqualityComparer<HarpVersion>.Default.Equals(a, b));
            Assert.IsGreaterThan(0, Comparer<HarpVersion>.Default.Compare(a, b));
        }

        static void AssertOperatorConsistent(HarpVersion a, HarpVersion b)
        {
            Assert.AreNotEqual(a == b, a != b);
            Assert.AreNotEqual(a < b, a >= b);
            Assert.AreNotEqual(a > b, a <= b);
        }

        [TestMethod]
        public void CompareNullVersions()
        {
            HarpVersion a = null;
            HarpVersion b = null;
            AssertEquals(a, b);
        }

        [TestMethod]
        public void CompareNullWithNonNullVersion()
        {
            HarpVersion a = null;
            HarpVersion b = new HarpVersion(null, null);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
        }

        [TestMethod]
        public void CompareSpecificVersions_Equal()
        {
            HarpVersion a = new HarpVersion(1, 0, 0);
            HarpVersion b = new HarpVersion(1, 0, 0);
            AssertEquals(a, b);
        }

        [TestMethod]
        public void ComparePatchSpecificVersions_NotEqual()
        {
            HarpVersion a = new HarpVersion(1, 0, 0);
            HarpVersion b = new HarpVersion(1, 0, 1);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
        }

        [TestMethod]
        public void CompareMinorSpecificVersions_NotEqual()
        {
            HarpVersion a = new HarpVersion(1, 0);
            HarpVersion b = new HarpVersion(1, 1);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
        }

        [TestMethod]
        public void CompareMajorSpecificVersions_NotEqual()
        {
            HarpVersion a = new HarpVersion(2, 0);
            HarpVersion b = new HarpVersion(1, 1);
            AssertLessThan(b, a);
            AssertGreaterThan(a, b);
        }

        [TestMethod]
        public void ComparePatchFloatingVersion_NotEqual()
        {
            HarpVersion a = new HarpVersion(2, 1, null);
            HarpVersion b = new HarpVersion(2, 1, 0);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
            Assert.IsTrue(a.Satisfies(b));
            Assert.IsTrue(b.Satisfies(a));
        }

        [TestMethod]
        public void CompareMinorFloatingVersion_NotEqual()
        {
            HarpVersion a = new HarpVersion(2, null);
            HarpVersion b = new HarpVersion(2, 1, 0);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
            Assert.IsTrue(a.Satisfies(b));
            Assert.IsTrue(b.Satisfies(a));
        }

        [TestMethod]
        public void ComparePatchFloatingVersion_NotSatisfies()
        {
            HarpVersion a = new HarpVersion(2, 2, null);
            HarpVersion b = new HarpVersion(2, 1);
            AssertLessThan(b, a);
            AssertGreaterThan(a, b);
            Assert.IsFalse(a.Satisfies(b));
            Assert.IsFalse(b.Satisfies(a));
        }

        [TestMethod]
        public void CompareMinorFloatingVersion_NotSatisfies()
        {
            HarpVersion a = new HarpVersion(2, null);
            HarpVersion b = new HarpVersion(1, 1);
            AssertLessThan(b, a);
            AssertGreaterThan(a, b);
            Assert.IsFalse(a.Satisfies(b));
            Assert.IsFalse(b.Satisfies(a));
        }

        [TestMethod]
        public void CompareMajorFloatingVersion_NotEqual()
        {
            HarpVersion a = new HarpVersion(null, null);
            HarpVersion b = new HarpVersion(1, 1);
            AssertLessThan(a, b);
            AssertGreaterThan(b, a);
            Assert.IsTrue(a.Satisfies(b));
            Assert.IsTrue(b.Satisfies(a));
        }

        [TestMethod]
        public void ParseAndToStringSpecificVersion_AreReversible()
        {
            var x = new HarpVersion(2, 1, 0);
            var text = x.ToString();
            var y = HarpVersion.Parse(text);
            AssertEquals(x, y);
        }

        [TestMethod]
        public void ParseAndToStringPatchFloating_AreReversible()
        {
            var input = "1.0.x";
            var x = HarpVersion.Parse(input);
            Assert.AreEqual("1.0", x.ToString());
            var y = HarpVersion.Parse(x.ToString());
            Assert.AreEqual(x, y);
        }

        [TestMethod]
        public void InvalidParseRunawayCharacters_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => HarpVersion.Parse("1.xx"));
        }

        [TestMethod]
        public void InvalidParseInvalidSeparator_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => HarpVersion.Parse("1;2"));
        }

        [TestMethod]
        public void InvalidParseWithFloatingMajor_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => HarpVersion.Parse("x.1"));
        }

        [TestMethod]
        public void InvalidParseWithFloatingMinor_ThrowsException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => HarpVersion.Parse("1.x.0"));
        }
    }
}
