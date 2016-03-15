#region Copyright
/********************************************************************************
 * Aadhaar API for .NET
 * Copyright © 2015 Souvik Dey Chowdhury
 * 
 * This file is part of Aadhaar API for .NET.
 * 
 * Aadhaar API for .NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 * 
 * Aadhaar API for .NET is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with Aadhaar API for .NET. If not, see http://www.gnu.org/licenses.
 ********************************************************************************/
#endregion

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Resident;
using Xunit;

namespace Uidai.AadhaarTests.Resident
{
    public class TestFingerTest
    {
        [Fact]
        public void NumberOfAttemptsTest()
        {
            var testFinger = new TestFinger();
            var inside = new[] { 1, 10 };
            var outside = new[] { -1, 0 };

            // Valid Tests.
            foreach (var numberOfAttempts in inside)
            {
                testFinger.NumberOfAttempts = numberOfAttempts;
                Assert.Equal(numberOfAttempts, testFinger.NumberOfAttempts);
            }

            // Invalid Tests.
            testFinger.NumberOfAttempts = inside[0];
            foreach (var numberOfAttempts in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => testFinger.NumberOfAttempts = numberOfAttempts);
                Assert.NotEqual(numberOfAttempts, testFinger.NumberOfAttempts);
            }
        }

        [Fact]
        public void EqualsTest()
        {
            var testFinger = new TestFinger { Data = new string('A', 10), Position = BiometricPosition.LeftIndex };
            var differByPosition = new TestFinger { Data = new string('A', 10), Position = BiometricPosition.RightIndex };
            var differByData = new TestFinger { Data = new string('B', 10), Position = BiometricPosition.LeftIndex };
            var differByClass = new Identity();

            Assert.False(testFinger.Equals(differByPosition));
            Assert.NotEqual(testFinger.GetHashCode(), differByPosition.GetHashCode());

            Assert.True(testFinger.Equals(differByData));
            Assert.Equal(testFinger.GetHashCode(), differByData.GetHashCode());

            Assert.False(testFinger.Equals(differByClass));
            Assert.NotEqual(testFinger.GetHashCode(), differByClass.GetHashCode());
        }

        [Fact]
        public void ToXmlTest()
        {
            /*
            Test:       LeftIris, RightIris, Unknown.
            */
            var testFinger = Data.TestFinger;
            var xml = XElement.Parse(File.ReadAllText(Data.TestFingerXml)).Elements().ToArray();

            // Set: Position = LeftIris
            testFinger.Position = BiometricPosition.LeftIris;
            Assert.Throws<ArgumentException>(nameof(TestFinger.Position), () => testFinger.ToXml("Bio"));

            // Set: Position = RightIris
            testFinger.Position = BiometricPosition.RightIris;
            Assert.Throws<ArgumentException>(nameof(TestFinger.Position), () => testFinger.ToXml("Bio"));

            // Set: Position = Unknown
            testFinger.Position = BiometricPosition.Unknown;
            Assert.Throws<ArgumentException>(nameof(TestFinger.Position), () => testFinger.ToXml("Bio"));

            // Set Position = LeftIndex, Data = null
            testFinger.Position = BiometricPosition.LeftIndex;
            testFinger.Data = null;
            Assert.Throws<ArgumentException>(nameof(TestFinger.Data), () => testFinger.ToXml("Bio"));

            // Set: Position = LeftIndex, Data = MMMMMMMMMM
            testFinger.Position = BiometricPosition.LeftIndex;
            testFinger.Data = new string('M', 10);
            Assert.True(XNode.DeepEquals(xml[0], testFinger.ToXml("Bio")));
        }
    }
}