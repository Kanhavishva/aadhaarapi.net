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
    public class DemographicTest
    {
        [Fact]
        public void FromXml()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var demographic = new Demographic();
            var xml = XElement.Parse(File.ReadAllText(Data.DemographicXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => demographic.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                demographic.FromXml(element);
                Assert.True(XNode.DeepEquals(element, demographic.ToXml("Demo")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var demographic = Data.Demographic;
            var xml = XElement.Parse(File.ReadAllText(Data.DemographicXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], demographic.ToXml("Demo")));

            // Set: Address
            demographic.Address = Data.Address;
            Assert.Throws<ArgumentException>(() => demographic.ToXml("Demo"));

            // Remove: LanguageUsed
            demographic.LanguageUsed = null;
            Assert.Throws<ArgumentException>(nameof(Demographic.LanguageUsed), () => demographic.ToXml("Demo"));

            // Set: LanguageUsed | Remove: FullAddress, Identity.ILName
            demographic.LanguageUsed = Data.Demographic.LanguageUsed;
            demographic.FullAddress = null;
            demographic.Identity.ILName = null;
            Assert.True(XNode.DeepEquals(xml[1], demographic.ToXml("Demo")));

            // Remove: All
            demographic.Address = null;
            demographic.Identity = null;
            Assert.True(XNode.DeepEquals(xml[2], demographic.ToXml("Demo")));
        }
    }
}