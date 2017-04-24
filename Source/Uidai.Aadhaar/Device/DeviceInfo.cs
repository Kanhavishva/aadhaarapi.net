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
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents metadata information of device.
    /// </summary>
    public class DeviceInfo : IXml
    {
        /// <summary>
        /// Represents device not applicable. This field is read-only.
        /// </summary>
        /// <value>Device not applicable.</value>
        public static readonly string DeviceNotApplicable = "NA";

        /// <summary>
        /// Represents device not certified. This field is read-only.
        /// </summary>
        /// <value>Device not certified.</value>
        public static readonly string DeviceNotCertified = "NC";

        /// <summary>
        /// Represents the geocoordinate string format used in serialization. This field is read-only.
        /// </summary>
        /// <value>The geocoordinate string format used in serialization</value>
        public static readonly string GeoCoordinateStringFormat = "{0:0.####},{1:0.####},{2:0.##}";

        private string fingerprintDeviceCode = DeviceNotCertified, irisDeviceCode = DeviceNotCertified, uniqueDeviceCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> class.
        /// </summary>
        public DeviceInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> class from an XML.
        /// <param name="element">The XML to deserialize.</param>
        /// </summary>
        public DeviceInfo(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the terminal device code. Suggested format is [vendor code][date of deployment][serial number].
        /// Maximum length is 20 characters.
        /// </summary>
        /// <value>The terminal device code.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 20 characters.</exception>
        public string UniqueDeviceCode
        {
            get { return uniqueDeviceCode; }
            set
            {
                if (value?.Length > 20)
                    throw new ArgumentOutOfRangeException(nameof(UniqueDeviceCode), OutOfRangeDeviceCode);
                uniqueDeviceCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the fingerprint device code.
        /// Maximum length is 10 characters.
        /// Default is <see cref="DeviceNotCertified"/>.
        /// </summary>
        /// <value>The fingerprint device code.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 10 characters.</exception>
        public string FingerprintDeviceCode
        {
            get { return fingerprintDeviceCode; }
            set
            {
                if (ValidateEmptyString(value, nameof(FingerprintDeviceCode)).Length > 10)
                    throw new ArgumentOutOfRangeException(nameof(FingerprintDeviceCode), OutOfRangeDeviceCode);
                fingerprintDeviceCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the iris device code.
        /// Maximum length is 20 characters.
        /// Default is <see cref="DeviceNotCertified"/>.
        /// </summary>
        /// <value>The iris device code.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 10 characters.</exception>
        public string IrisDeviceCode
        {
            get { return irisDeviceCode; }
            set
            {
                if (ValidateEmptyString(value, nameof(IrisDeviceCode)).Length > 10)
                    throw new ArgumentOutOfRangeException(nameof(IrisDeviceCode), OutOfRangeDeviceCode);
                irisDeviceCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the public IP address of the device.
        /// </summary>
        /// <value>The public IP address of the device.</value>
        public IPAddress PublicAddress { get; set; }

        /// <summary>
        /// Gets or sets the geographic coordinate or pincode of the device.
        /// <see cref="SetGeoCoordinate(double, double, double)"/> or <see cref="SetPincode(string)"/> should be used to set this property.
        /// </summary>
        /// <value>The geographic coordinate or pincode of the device.</value>
        public string Location { get; set; } = "0,0,0";

        /// <summary>
        /// Gets or sets the location type used by the device.
        /// Default is <see cref="LocationType.GeoCoordinate"/>.
        /// </summary>
        /// <value>The location type used by the device.</value>
        public LocationType LocationType { get; set; } = LocationType.GeoCoordinate;

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            UniqueDeviceCode = element.Attribute("udc").Value;
            FingerprintDeviceCode = element.Attribute("fdc").Value;
            IrisDeviceCode = element.Attribute("idc").Value;
            LocationType = (LocationType)element.Attribute("lot").Value[0];
            Location = element.Attribute("lov").Value;

            var pip = element.Attribute("pip").Value;
            PublicAddress = pip == DeviceNotApplicable ? null : IPAddress.Parse(pip);
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the parent element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="UniqueDeviceCode"/> or <see cref="Location"/> is empty.</exception>
        public XElement ToXml(string elementName)
        {
            ValidateEmptyString(UniqueDeviceCode, nameof(UniqueDeviceCode));
            ValidateEmptyString(Location, nameof(Location));

            var metadata = new XElement(elementName,
                new XAttribute("udc", UniqueDeviceCode),
                new XAttribute("fdc", FingerprintDeviceCode),
                new XAttribute("idc", IrisDeviceCode),
                new XAttribute("pip", PublicAddress?.ToString() ?? DeviceNotApplicable),
                new XAttribute("lot", (char)LocationType),
                new XAttribute("lov", Location));

            return metadata;
        }

        /// <summary>
        /// Creates a new <see cref="DeviceInfo"/> object from an existing instance.
        /// </summary>
        /// <returns>An instance of <see cref="DeviceInfo"/>.</returns>
        public DeviceInfo Create()
        {
            return new DeviceInfo
            {
                uniqueDeviceCode = uniqueDeviceCode,
                fingerprintDeviceCode = fingerprintDeviceCode,
                irisDeviceCode = irisDeviceCode,
                PublicAddress = PublicAddress,
                Location = Location,
                LocationType = LocationType
            };
        }

        /// <summary>
        /// Sets <see cref="Location"/> to the specified geocoordinate according to ISO 6709 specification.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="latitude"/>, <paramref name="longitude"/> or <paramref name="altitude"/> is out of range.</exception>
        public void SetGeoCoordinate(double latitude, double longitude, double altitude)
        {
            if (latitude > 90.0 || latitude < -90.0)
                throw new ArgumentOutOfRangeException(nameof(latitude), OutOfRangeLatitude);
            if (longitude > 180.0 || longitude < -180.0)
                throw new ArgumentOutOfRangeException(nameof(longitude), OutOfRangeLongitude);
            if (altitude > 999.0 || altitude < -999.0)
                throw new ArgumentOutOfRangeException(nameof(altitude), OutOfRangeAltitude);

            Location = string.Format(CultureInfo.InvariantCulture, GeoCoordinateStringFormat, latitude, longitude, altitude);
            LocationType = LocationType.GeoCoordinate;
        }

        /// <summary>
        /// Sets <see cref="Location"/> to the specified pincode.
        /// </summary>
        /// <param name="pincode">The pincode.</param>
        /// <exception cref="ArgumentException"><paramref name="pincode"/> is invalid.</exception>
        public void SetPincode(string pincode)
        {
            if (!AadhaarHelper.ValidatePincode(pincode))
                throw new ArgumentException(nameof(pincode), InvalidPincode);

            Location = pincode;
            LocationType = LocationType.Pincode;
        }
    }
}