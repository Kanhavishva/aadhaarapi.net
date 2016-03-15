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
    public class KycResponseTest
    {
        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var kycResponse = new KycResponse();
            var xml = XElement.Parse(File.ReadAllText(Data.KycResponseXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => kycResponse.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml.Where(element => element.Name != "Resp"))
            {
                kycResponse.FromXml(element);
                Assert.True(XNode.DeepEquals(element, kycResponse.ToXml("KycRes")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var kycResponse = Data.KycResponse;
            var xml = XElement.Parse(File.ReadAllText(Data.KycResponseXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], kycResponse.ToXml("KycRes")));

            // Remove: ILAddress
            kycResponse.Resident.Demographic.ILAddress = null;
            Assert.True(XNode.DeepEquals(xml[1], kycResponse.ToXml("KycRes")));
        }
    }
}