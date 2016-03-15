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
using Uidai.Aadhaar.Api;
using Xunit;

namespace Uidai.AadhaarTests.Api
{
    public class AuthRequestTest
    {
        [Fact]
        public void AuaCodeTest()
        {
            var authRequest = new AuthRequest();

            var inside = new[] { null, string.Empty, new string('A', 10) };
            var outside = new[] { new string('A', 11) };

            // Valid Tests.
            foreach (var auaCode in inside)
            {
                authRequest.AuaCode = auaCode;
                authRequest.SubAuaCode = auaCode;
                Assert.Equal(auaCode, authRequest.AuaCode);
                Assert.Equal(auaCode, authRequest.SubAuaCode);
            }

            // Invalid Tests.
            authRequest.AuaCode = inside[0];
            foreach (var auaCode in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(nameof(AuthRequest.AuaCode), () => authRequest.AuaCode = auaCode);
                Assert.Throws<ArgumentOutOfRangeException>(nameof(AuthRequest.SubAuaCode), () => authRequest.SubAuaCode = auaCode);
                Assert.NotEqual(auaCode, authRequest.AuaCode);
                Assert.NotEqual(auaCode, authRequest.SubAuaCode);
            }
        }

        [Fact]
        public void TransactionTest()
        {
            var authRequest = new AuthRequest();

            var inside = new[] { null, string.Empty, new string('A', 50) };
            var outside = new[] { new string('A', 51) };

            // Valid Tests.
            foreach (var transaction in inside)
            {
                authRequest.Transaction = transaction;
                Assert.Equal(transaction, authRequest.Transaction);
            }

            // Invalid Tests.
            authRequest.Transaction = inside[0];
            foreach (var transaction in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(nameof(AuthRequest.Transaction), () => authRequest.Transaction = transaction);
                Assert.NotEqual(transaction, (string)authRequest.Transaction);
            }
        }

        [Fact]
        public void AuaLicenseKey()
        {
            var authRequest = new AuthRequest();

            var inside = new[] { null, string.Empty, new string('A', 64) };
            var outside = new[] { new string('A', 65) };

            // Valid Tests.
            foreach (var auaLicenseKey in inside)
            {
                authRequest.AuaLicenseKey = auaLicenseKey;
                Assert.Equal(auaLicenseKey, authRequest.AuaLicenseKey);
            }

            // Invalid Tests.
            authRequest.AuaLicenseKey = inside[0];
            foreach (var auaLicenseKey in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(nameof(AuthRequest.AuaLicenseKey), () => authRequest.AuaLicenseKey = auaLicenseKey);
                Assert.NotEqual(auaLicenseKey, authRequest.AuaLicenseKey);
            }
        }

        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var authRequest = new AuthRequest();
            var xml = XElement.Parse(File.ReadAllText(Data.AuthRequestXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => authRequest.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                authRequest.FromXml(element);
                Assert.True(XNode.DeepEquals(element, authRequest.ToXml("Auth")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var authRequest = Data.AuthRequest;
            var xml = XElement.Parse(File.ReadAllText(Data.AuthRequestXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], authRequest.ToXml("Auth")));

            // Remove: SubAuaCode
            authRequest.SubAuaCode = null;
            Assert.True(XNode.DeepEquals(xml[0], authRequest.ToXml("Auth")));

            // Remove: Token
            authRequest.Token = null;
            Assert.True(XNode.DeepEquals(xml[1], authRequest.ToXml("Auth")));

            // Remove: AuaLicenseKey
            authRequest.AuaLicenseKey = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.AuaLicenseKey), () => authRequest.ToXml("Auth"));

            // Remove: Transaction
            authRequest.Transaction = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.Transaction), () => authRequest.ToXml("Auth"));

            // Remove: AuaCode
            authRequest.AuaCode = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.AuaCode), () => authRequest.ToXml("Auth"));

            // Remove: Terminal
            authRequest.Terminal = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.Terminal), () => authRequest.ToXml("Auth"));

            // Remove: Hmac
            authRequest.Hmac = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.Hmac), () => authRequest.ToXml("Auth"));

            // Remove: AadhaarNumber
            authRequest.AadhaarNumber = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.AadhaarNumber), () => authRequest.ToXml("Auth"));

            // Remove: Data
            authRequest.Data = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.Data), () => authRequest.ToXml("Auth"));

            // Remove: KeyInfo
            authRequest.KeyInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.KeyInfo), () => authRequest.ToXml("Auth"));

            // Remove: DeviceInfo
            authRequest.DeviceInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.DeviceInfo), () => authRequest.ToXml("Auth"));

            // Remove: Uses
            authRequest.Uses = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.Uses), () => authRequest.ToXml("Auth"));
        }
    }
}