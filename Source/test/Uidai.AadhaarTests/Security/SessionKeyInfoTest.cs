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
using Uidai.Aadhaar.Security;
using Xunit;

namespace Uidai.AadhaarTests.Security
{
    public class SessionKeyInfoTest
    {
        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var sessionKey = new SessionKeyInfo();
            var xml = XElement.Parse(File.ReadAllText(Data.SessionKeyInfoXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => sessionKey.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                sessionKey.FromXml(element);
                Assert.True(XNode.DeepEquals(element, sessionKey.ToXml("Skey")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var sessionKeyInfo = Data.SessionKeyInfo;
            var xml = XElement.Parse(File.ReadAllText(Data.SessionKeyInfoXml)).Elements().ToArray();

            Assert.True(XNode.DeepEquals(xml[0], sessionKeyInfo.ToXml("Skey")));

            // Remove: KeyIdentifier
            sessionKeyInfo.KeyIdentifier = Guid.Empty;
            Assert.True(XNode.DeepEquals(xml[1], sessionKeyInfo.ToXml("Skey")));
        }
    }
}