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

using System.IO;
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Xunit;

namespace Uidai.AadhaarTests.Helper
{
    public class AadhaarHelperTest
    {
        [Fact]
        public void ValidateAadhaarNumberTest()
        {
            var inside = new[] { "999999999999" };
            var outside = new[] { null, string.Empty, "999999999990", "9999 9999 9999" };

            // Valid Tests.
            foreach (var aadhaarNumber in inside)
                Assert.True(AadhaarHelper.ValidateAadhaarNumber(aadhaarNumber));

            // Invalid Tests.
            foreach (var aadhaarNumber in outside)
                Assert.False(AadhaarHelper.ValidateAadhaarNumber(aadhaarNumber));
        }

        public void ValidatePincodeTest()
        {
            var inside = new[] { "000000", "999999" };
            var outside = new[] { null, string.Empty, "9999999", "999 999" };

            // Valid Tests.
            foreach (var aadhaarNumber in inside)
                Assert.True(AadhaarHelper.ValidatePincode(aadhaarNumber));

            // Invalid Tests.
            foreach (var aadhaarNumber in outside)
                Assert.False(AadhaarHelper.ValidatePincode(aadhaarNumber));
        }

        [Fact]
        public void DecodeQRCodeXmlTest()
        {
            var personalInfo = new PersonalInfo();
            var xml = XElement.Parse(File.ReadAllText(Data.AadhaarLetterXml)).Elements().ToArray();

            personalInfo.DecodeQRCodeXml(xml[0]);

            var identity = new Identity();
            identity.FromXml(xml[0]);
            Assert.True(XNode.DeepEquals(identity.ToXml("Pi"), personalInfo.Demographic.Identity.ToXml("Pi")));

            var address = new Address();
            address.FromXml(xml[0]);
            Assert.True(XNode.DeepEquals(address.ToXml("Pa"), personalInfo.Demographic.Address.ToXml("Pa")));
        }
    }
}