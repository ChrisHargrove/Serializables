using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BatteryAcid.Serializables.Tests
{
    public class GuidTests
    {
        [Test]
        public void TestImplicitConversionToGuid()
        {
            SerializableGuid testGuid = new SerializableGuid(Guid.NewGuid());
            Guid converted = testGuid;
            Assert.AreEqual(testGuid, converted);
        }

        [Test]
        public void TestImplicitConversionToSerializableGuid()
        {
            Guid guid = Guid.NewGuid();
            SerializableGuid testGuid = guid;
            Assert.AreEqual(testGuid, guid);
        }

        [Test]
        public void TestEqualityOperators()
        {
            Guid guid = Guid.NewGuid();
            SerializableGuid testGuid = guid;

            Assert.IsTrue(guid == testGuid);
        }

        [Test]
        public void TestNonEqualityOperators()
        {
            Guid guid = Guid.NewGuid();
            SerializableGuid testGuid = Guid.NewGuid();

            Assert.IsTrue(guid != testGuid);
        }
    }
}
