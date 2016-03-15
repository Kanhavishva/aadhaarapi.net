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
    public class BiometricTest
    {
        [Fact]
        public void EqualsTest()
        {
            var biometric = new Biometric { Data = new string('A', 10), Position = BiometricPosition.LeftIndex, Type = BiometricType.Fingerprint };
            var differByType = new Biometric { Data = new string('A', 10), Position = BiometricPosition.LeftIndex, Type = BiometricType.Minutiae };
            var differByPosition = new Biometric { Data = new string('A', 10), Position = BiometricPosition.RightIndex, Type = BiometricType.Fingerprint };
            var differByData = new Biometric { Data = new string('B', 10), Position = BiometricPosition.LeftIndex, Type = BiometricType.Fingerprint };
            var differByClass = new Identity();

            Assert.False(biometric.Equals(differByType));
            Assert.NotEqual(biometric.GetHashCode(), differByType.GetHashCode());

            Assert.False(biometric.Equals(differByPosition));
            Assert.NotEqual(biometric.GetHashCode(), differByPosition.GetHashCode());

            Assert.True(biometric.Equals(differByData));
            Assert.Equal(biometric.GetHashCode(), differByData.GetHashCode());

            Assert.False(biometric.Equals(differByClass));
            Assert.NotEqual(biometric.GetHashCode(), differByClass.GetHashCode());
        }

        [Fact]
        public void ToXmlTest()
        {
            /*
            Test:       Fingerprint/Minutiae, LeftIris.
                        Fingerprint/Minutiae, RightIris.
                        Iris, LeftIndex.
            */
            var biometric = Data.BiometricMinutiae;
            var xml = XElement.Parse(File.ReadAllText(Data.BiometricXml)).Elements().ToArray();

            #region Invalid Tests

            // Set: Type = Fingerprint, Position = LeftIris
            biometric.Type = BiometricType.Fingerprint;
            biometric.Position = BiometricPosition.LeftIris;
            Assert.Throws<ArgumentException>(nameof(Biometric.Position), () => biometric.ToXml("Bio"));

            // Set: Type = Fingerprint, Position = RightIris
            biometric.Type = BiometricType.Fingerprint;
            biometric.Position = BiometricPosition.RightIris;
            Assert.Throws<ArgumentException>(nameof(Biometric.Position), () => biometric.ToXml("Bio"));

            // Set: Type = Minutiae, Position = LeftIris
            biometric.Type = BiometricType.Minutiae;
            biometric.Position = BiometricPosition.LeftIris;
            Assert.Throws<ArgumentException>(nameof(Biometric.Position), () => biometric.ToXml("Bio"));

            // Set: Type = Minutiae, Position = RightIris
            biometric.Type = BiometricType.Minutiae;
            biometric.Position = BiometricPosition.RightIris;
            Assert.Throws<ArgumentException>(nameof(Biometric.Position), () => biometric.ToXml("Bio"));

            // Set Type = Iris, Position = LeftIndex
            biometric.Type = BiometricType.Iris;
            biometric.Position = BiometricPosition.LeftIndex;
            Assert.Throws<ArgumentException>(nameof(Biometric.Position), () => biometric.ToXml("Bio"));

            // Set Type = Iris, Position = LeftIris, Data = null
            biometric.Type = BiometricType.Iris;
            biometric.Position = BiometricPosition.LeftIris;
            biometric.Data = null;
            Assert.Throws<ArgumentException>(nameof(Biometric.Data), () => biometric.ToXml("Bio"));

            #endregion

            #region Valid Tests

            // Set Type = Minutiae, Position = LeftIndex, Data = MMMMMMMMMM
            biometric.Type = BiometricType.Minutiae;
            biometric.Position = BiometricPosition.LeftIndex;
            biometric.Data = new string('M', 10);
            Assert.True(XNode.DeepEquals(xml[0], biometric.ToXml("Bio")));

            // Set Type = Fingerprint, Position = LeftIndex, Data = FFFFFFFFFF
            biometric.Type = BiometricType.Fingerprint;
            biometric.Position = BiometricPosition.LeftIndex;
            biometric.Data = new string('F', 10);
            Assert.True(XNode.DeepEquals(xml[1], biometric.ToXml("Bio")));

            // Set Type = Iris, Position = LeftIris, Data = IIIIIIIIII
            biometric.Type = BiometricType.Iris;
            biometric.Position = BiometricPosition.LeftIris;
            biometric.Data = new string('I', 10);
            Assert.True(XNode.DeepEquals(xml[2], biometric.ToXml("Bio")));

            // Set Type = Fingerprint, Position = Unknown, Data = FFFFFFFFFF
            biometric.Type = BiometricType.Fingerprint;
            biometric.Position = BiometricPosition.Unknown;
            biometric.Data = new string('F', 10);
            Assert.True(XNode.DeepEquals(xml[3], biometric.ToXml("Bio")));

            // Set Type = Iris, Position = Unknown, Data = IIIIIIIIII
            biometric.Type = BiometricType.Iris;
            biometric.Position = BiometricPosition.Unknown;
            biometric.Data = new string('I', 10);
            Assert.True(XNode.DeepEquals(xml[4], biometric.ToXml("Bio")));

            #endregion
        }
    }
}