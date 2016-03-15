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
    public class BfdRequestTest
    {
        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var bfdRequest = new BfdRequest();
            var xml = XElement.Parse(File.ReadAllText(Data.BfdRequestXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => bfdRequest.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                bfdRequest.FromXml(element);
                // Special case to remove IrisDeviceCode from final Bfd XML.
                element.Element("Meta").Attribute("idc").Remove();
                Assert.True(XNode.DeepEquals(element, bfdRequest.ToXml("Bfd")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var bfdRequest = Data.BfdRequest;
            var xml = XElement.Parse(File.ReadAllText(Data.BfdRequestXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], bfdRequest.ToXml("Bfd")));

            // Remove: Hmac
            bfdRequest.Hmac = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.Hmac), () => bfdRequest.ToXml("Bfd"));

            // Remove: AadhaarNumber
            bfdRequest.AadhaarNumber = null;
            Assert.Throws<ArgumentException>(nameof(AuthRequest.AadhaarNumber), () => bfdRequest.ToXml("Bfd"));

            // Remove: Data
            bfdRequest.Data = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.Data), () => bfdRequest.ToXml("Bfd"));

            // Remove: KeyInfo
            bfdRequest.KeyInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.KeyInfo), () => bfdRequest.ToXml("Bfd"));

            // Remove: DeviceInfo
            bfdRequest.DeviceInfo = null;
            Assert.Throws<ArgumentNullException>(nameof(AuthRequest.DeviceInfo), () => bfdRequest.ToXml("Bfd"));
        }
    }
}