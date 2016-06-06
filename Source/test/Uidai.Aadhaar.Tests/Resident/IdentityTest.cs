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
    public class IdentityTest
    {
        [Fact]
        public void NameTest()
        {
            var identity = new Identity();
            var inside = new[] { null, string.Empty, " ", new string('A', 60) };
            var outside = new[] { new string('A', 61) };

            // Valid Tests.
            foreach (var name in inside)
            {
                identity.Name = name;
                Assert.Equal(name, identity.Name);
            }

            // Invalid Tests.
            identity.Name = inside[0];
            foreach (var name in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(nameof(Identity.Name), () => identity.Name = name);
                Assert.NotEqual(name, identity.Name);
            }
        }

        [Fact]
        public void AgeTest()
        {
            var identity = new Identity();
            var inside = new[] { 0, 150 };
            var outside = new[] { -1, 151 };

            // Valid Tests.
            foreach (var age in inside)
            {
                identity.Age = age;
                Assert.Equal(age, identity.Age);
            }

            // Invalid Tests.
            identity.Age = inside[0];
            foreach (var age in outside)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => identity.Age = age);
                Assert.NotEqual(age, identity.Age);
            }
        }

        [Fact]
        public void IsUsedTest()
        {
            Assert.True(Data.Identity.IsUsed());
            Assert.True(new Identity { Name = "name" }.IsUsed());
            Assert.False(new Identity().IsUsed());
        }

        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var identity = new Identity();
            var xml = XElement.Parse(File.ReadAllText(Data.IdentityXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => identity.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                identity.FromXml(element);
                Assert.True(XNode.DeepEquals(element, identity.ToXml("Pi")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var identity = Data.Identity;
            var xml = XElement.Parse(File.ReadAllText(Data.IdentityXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], identity.ToXml("Pi")));

            // Remove: ILName
            identity.ILName = null;
            Assert.True(XNode.DeepEquals(xml[1], identity.ToXml("Pi")));

            // Set: Match = Exact, VerifyOnlyBirthYear = true, Remove: DoBType
            identity.Match = MatchingStrategy.Exact;
            identity.VerifyOnlyBirthYear = true;
            identity.DoBType = null;
            Assert.True(XNode.DeepEquals(xml[2], identity.ToXml("Pi")));

            // Remove: Age, DateOfBirth, Gender
            identity.Age = 0;
            identity.DateOfBirth = null;
            identity.Gender = null;
            Assert.True(XNode.DeepEquals(xml[3], identity.ToXml("Pi")));

            // Remove: All
            identity.Name = identity.ILName = identity.Phone = identity.Email = null;
            Assert.True(XNode.DeepEquals(xml[4], identity.ToXml("Pi")));
        }
    }
}