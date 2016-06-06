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
using Uidai.Aadhaar.Device;
using Xunit;

namespace Uidai.AadhaarTests.Device
{
    public class AuthContextTest
    {
        [Fact]
        public void EncryptTest()
        {
            var authContext = new AuthContext
            {
                DeviceInfo = Data.Metadata,
                Terminal = Data.PublicTerminal
            };
            var personalInfo = Data.PersonalInfo;
            var sessionKey = Data.SessionKey;

            // Test 1: Validate null argument.
            Assert.Throws<ArgumentNullException>("key", () => authContext.Encrypt(personalInfo, null));
            Assert.Throws<ArgumentNullException>("data", () => authContext.Encrypt(null, sessionKey));

            // Test 2: All fields are set after call.
            authContext.Encrypt(personalInfo, sessionKey);
            Assert.NotNull(authContext.AadhaarNumber);
            Assert.NotNull(authContext.Data);
            Assert.NotNull(authContext.DeviceInfo);
            Assert.NotNull(authContext.Hmac);
            Assert.NotNull(authContext.KeyInfo);
            Assert.Equal(personalInfo.Timestamp, authContext.Timestamp);

            // Test 3: DeviceInfo device value are set to NA.
            personalInfo.Biometrics.Clear();
            authContext.Encrypt(personalInfo, sessionKey);
            Assert.Equal(Metadata.DeviceNotApplicable, authContext.DeviceInfo.FingerprintDeviceCode);
            Assert.Equal(Metadata.DeviceNotApplicable, authContext.DeviceInfo.IrisDeviceCode);
        }

        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var authContext = new AuthContext();
            var xml = XElement.Parse(File.ReadAllText(Data.AuthContextXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => authContext.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                authContext.FromXml(element);
                Assert.True(XNode.DeepEquals(element, authContext.ToXml("Auth")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var authContext = Data.AuthContext;
            var xml = XElement.Parse(File.ReadAllText(Data.AuthContextXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], authContext.ToXml("Auth")));

            // Remove: Hmac
            authContext.Hmac = null;
            Assert.Throws<ArgumentException>(nameof(AuthContext.Hmac), () => authContext.ToXml("Auth"));

            // Remove: AadhaarNumber
            authContext.AadhaarNumber = null;
            Assert.Throws<ArgumentException>(nameof(AuthContext.AadhaarNumber), () => authContext.ToXml("Auth"));

            // Remove: Data
            authContext.Data = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthContext.Data), () => authContext.ToXml("Auth"));

            // Remove: KeyInfo
            authContext.KeyInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthContext.KeyInfo), () => authContext.ToXml("Auth"));

            // Remove: Device
            authContext.DeviceInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthContext.DeviceInfo), () => authContext.ToXml("Auth"));

            // Remove: Uses
            authContext.Uses = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthContext.Uses), () => authContext.ToXml("Auth"));
        }
    }
}