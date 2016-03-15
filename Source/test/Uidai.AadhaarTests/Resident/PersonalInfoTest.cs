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
    public class PersonalInfoTest
    {
        [Fact]
        public void AuthUsageTest()
        {
            var personalInfo = Data.PersonalInfo;
            var uses = Data.AuthUsage;

            // Set: All
            Assert.True(XNode.DeepEquals(uses.ToXml("Uses"), personalInfo.Uses.ToXml("Uses")));

            // Remove: Biometrics
            uses.AuthUsed ^= AuthTypes.Biometric;
            personalInfo.Biometrics.Clear();
            Assert.True(XNode.DeepEquals(uses.ToXml("Uses"), personalInfo.Uses.ToXml("Uses")));

            // Remove: Identity, Address, Otp, Pin
            uses.AuthUsed ^= AuthTypes.FullAddress | AuthTypes.Identity | AuthTypes.Otp | AuthTypes.Pin;
            personalInfo.Demographic.FullAddress = null;
            personalInfo.Demographic.Identity = null;
            personalInfo.PinValue.Otp = personalInfo.PinValue.Pin = null;
            Assert.True(XNode.DeepEquals(uses.ToXml("Uses"), personalInfo.Uses.ToXml("Uses")));
        }

        [Fact]
        public void ToXmlTest()
        {
            var personalInfo = Data.PersonalInfo;
            var xml = XElement.Parse(File.ReadAllText(Data.PersonalInfoXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], personalInfo.ToXml("Pid")));

            // Set: Biometrics[Fingerprint]
            var fingerprint = new Biometric { Type = BiometricType.Fingerprint };
            personalInfo.Biometrics.Add(fingerprint);
            Assert.Throws<ArgumentException>(nameof(PersonalInfo.Biometrics), () => personalInfo.ToXml("Pid"));
            personalInfo.Biometrics.Remove(fingerprint);

            // Remove: All
            personalInfo.Demographic = null;
            personalInfo.PinValue = null;
            personalInfo.Biometrics.Clear();
            Assert.Throws<ArgumentException>(() => personalInfo.ToXml("Pid"));
        }
    }
}