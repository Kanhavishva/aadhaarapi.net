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
    public class MetadataTest
    {
        [Fact]
        public void UniqueDeviceCodeTest()
        {
            var metadata = new Metadata();
            var inside = new[] { null, string.Empty, "A", new string('A', 20) };
            var outside = new[] { new string('A', 21) };

            // Valid Tests.
            foreach (var uniqueDeviceCode in inside)
            {
                metadata.UniqueDeviceCode = uniqueDeviceCode;
                Assert.Equal(uniqueDeviceCode, metadata.UniqueDeviceCode);
            }

            // Invalid Tests.
            metadata.UniqueDeviceCode = inside[0];
            foreach (var uniqueDeviceCode in outside)
            {
                Assert.ThrowsAny<ArgumentException>(() => metadata.UniqueDeviceCode = uniqueDeviceCode);
                Assert.NotEqual(uniqueDeviceCode, metadata.UniqueDeviceCode);
            }
        }

        [Fact]
        public void FingerprintDeviceCodeTest()
        {
            var metadata = new Metadata();
            var inside = new[] { "A", new string('A', 10) };
            var outside = new[] { null, string.Empty, new string('A', 11) };

            // Valid Tests.
            foreach (var fingerprintDeviceCode in inside)
            {
                metadata.FingerprintDeviceCode = fingerprintDeviceCode;
                Assert.Equal(fingerprintDeviceCode, metadata.FingerprintDeviceCode);
            }

            // Invalid Tests.
            metadata.FingerprintDeviceCode = inside[0];
            foreach (var fingerprintDeviceCode in outside)
            {
                Assert.ThrowsAny<ArgumentException>(() => metadata.FingerprintDeviceCode = fingerprintDeviceCode);
                Assert.NotEqual(fingerprintDeviceCode, metadata.FingerprintDeviceCode);
            }
        }

        [Fact]
        public void IrisDeviceCodeTest()
        {
            var metadata = new Metadata();
            var inside = new[] { "A", new string('A', 10) };
            var outside = new[] { null, string.Empty, new string('A', 11) };

            // Valid Tests.
            foreach (var irisDeviceCode in inside)
            {
                metadata.IrisDeviceCode = irisDeviceCode;
                Assert.Equal(irisDeviceCode, metadata.IrisDeviceCode);
            }

            // Invalid Tests.
            metadata.IrisDeviceCode = inside[0];
            foreach (var irisDeviceCode in outside)
            {
                Assert.ThrowsAny<ArgumentException>(() => metadata.IrisDeviceCode = irisDeviceCode);
                Assert.NotEqual(irisDeviceCode, metadata.IrisDeviceCode);
            }
        }

        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var metadata = new Metadata();
            var xml = XElement.Parse(File.ReadAllText(Data.MetadataXml)).Elements().ToArray();

            // Validate null argument.
            Assert.Throws<ArgumentNullException>("element", () => metadata.FromXml(null));

            // XML must be same after loading and deserializing it.
            foreach (var element in xml)
            {
                metadata.FromXml(element);
                Assert.True(XNode.DeepEquals(element, metadata.ToXml("Meta")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var metadata = Data.Metadata;
            var xml = XElement.Parse(File.ReadAllText(Data.MetadataXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], metadata.ToXml("Meta")));

            // Set: LocationType = Pincode, PublicAddress = null
            metadata.SetPincode(Data.Pincode);
            metadata.PublicAddress = null;
            Assert.True(XNode.DeepEquals(xml[1], metadata.ToXml("Meta")));

            // Remove: Location
            metadata.Location = null;
            Assert.Throws<ArgumentException>(nameof(Metadata.Location), () => metadata.ToXml("Meta"));

            // Remove: UniqueDeviceCode
            metadata.UniqueDeviceCode = null;
            Assert.Throws<ArgumentException>(nameof(Metadata.UniqueDeviceCode), () => metadata.ToXml("Meta"));
        }

        [Fact]
        public void CreateTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var metadata = Data.Metadata;
            var clone = metadata.Create();

            Assert.True(XNode.DeepEquals(metadata.ToXml("Meta"), clone.ToXml("Meta")));
        }

        [Fact]
        public void SetCoordinateTest()
        {
            /*
            Rule:       Latitude:   -90 to +90.
                        Longitude:  -180 to +180.
                        Altitude:   -999 to +999.
            */
            var metadata = new Metadata();
            var latitudeInside = new[] { -90.0, 0.0, 90.0 };
            var longitudeInside = new[] { -180, 0.0, 180 };
            var altitudeInside = new[] { -999.0, 0.0, 999.0 };
            var geoCoordinateInside = new[] { "-90,-180,-999", "0,0,0", "90,180,999" };

            var latitudeOutside = new[] { -90.0 - 0.1, 90.0 + 0.1 };
            var longitudeOutside = new[] { -180 - 0.1, 180 + 0.1 };
            var altitudeOutside = new[] { -999.0 - 0.1, 999.0 + 0.1 };

            // Valid Tests.
            for (var i = 0; i < latitudeInside.Length; i++)
            {
                metadata.SetGeoCoordinate(latitudeInside[i], longitudeInside[i], altitudeInside[i]);
                Assert.Equal(geoCoordinateInside[i], metadata.Location);
            }

            // Invalid Tests.
            foreach (var latitude in latitudeOutside)
                Assert.Throws<ArgumentOutOfRangeException>("latitude", () => metadata.SetGeoCoordinate(latitude, longitudeInside[0], altitudeInside[0]));

            foreach (var longitude in longitudeOutside)
                Assert.Throws<ArgumentOutOfRangeException>("longitude", () => metadata.SetGeoCoordinate(latitudeInside[0], longitude, altitudeInside[0]));

            foreach (var altitude in altitudeOutside)
                Assert.Throws<ArgumentOutOfRangeException>("altitude", () => metadata.SetGeoCoordinate(latitudeInside[0], longitudeInside[0], altitude));
        }
    }
}