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
    public class AuthResponseTest
    {
        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var authResponse = new AuthResponse();
            var xml = XElement.Parse(File.ReadAllText(Data.AuthResponseXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => authResponse.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                authResponse.FromXml(element);
                Assert.True(XNode.DeepEquals(element, authResponse.ToXml("AuthRes")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var authResponse = Data.AuthResponse;
            var xml = XElement.Parse(File.ReadAllText(Data.AuthResponseXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], authResponse.ToXml("AuthRes")));

            // Remove: Transaction
            authResponse.Transaction = null;
            Assert.Throws<ArgumentException>(nameof(AuthResponse.Transaction), () => authResponse.ToXml("AuthRes"));

            // Remove: ResponseCode
            authResponse.ResponseCode = null;
            Assert.Throws<ArgumentException>(nameof(AuthResponse.ResponseCode), () => authResponse.ToXml("AuthRes"));
        }
    }
}