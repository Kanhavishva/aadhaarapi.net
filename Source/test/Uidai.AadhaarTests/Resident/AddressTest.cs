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
    public class AddressTest
    {
        [Fact]
        public void PincodeTest()
        {
            /*
            Assume:     AadhaarHelper.ValidatePincode(string) is correct.
            */
            var address = new Address();
            var inside = new[] { null, string.Empty, "000000", "999999" };

            // Valid Tests.
            foreach (var pincode in inside)
            {
                address.Pincode = pincode;
                Assert.Equal(pincode, address.Pincode);
            }
        }

        [Fact]
        public void IsUsedTest()
        {
            Assert.True(Data.Address.IsUsed());
            Assert.True(new Address { CareOf = "co" }.IsUsed());
            Assert.False(new Address().IsUsed());
        }

        [Fact]
        public void FromXml()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var address = new Address();
            var xml = XElement.Parse(File.ReadAllText(Data.AddressXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => address.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                address.FromXml(element);
                Assert.True(XNode.DeepEquals(element, address.ToXml("Pa")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var address = Data.Address;
            var xml = XElement.Parse(File.ReadAllText(Data.AddressXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], address.ToXml("Pa")));

            // Remove: All
            address.CareOf = address.House = address.Street = address.Landmark = address.Locality =
            address.VillageOrCity = address.SubDistrict = address.District = address.State = address.Pincode =
            address.PostOffice = null;
            Assert.True(XNode.DeepEquals(xml[1], address.ToXml("Pa")));
        }

        [Fact]
        public void ToStringTest()
        {
            /*
            Test:       Empty lines and unnecessary separators are removed.
                        Locality, District, Pincode.
            */
            var address = Data.Address;
            var addressString = File.ReadAllText(Data.AddressTxt);

            // Set: All
            Assert.Equal(addressString, address.ToString());

            // Remove: Locality
            addressString = addressString.Replace(", loc", string.Empty);
            address.Locality = null;
            Assert.Equal(addressString, address.ToString());
        }
    }
}