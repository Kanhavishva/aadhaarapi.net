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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Resident;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Provides a wrapper for helper and extension methods.
    /// </summary>
    public static class AadhaarHelper
    {
        /// <summary>
        /// Represents the no value used in XML.
        /// </summary>
        /// <value>The no value used in XML</value>
        public const char No = 'n';

        /// <summary>
        /// Represents the no value in upper case used in XML.
        /// </summary>
        /// <value>The no value in upper case used in XML</value>
        public const char NoUpper = 'N';

        /// <summary>
        /// Represents the yes value used in XML.
        /// </summary>
        /// <value>The yes value used in XML</value>
        public const char Yes = 'y';

        /// <summary>
        /// Represents the yes value in upper case used in XML.
        /// </summary>
        /// <value>The yes value in upper case used in XML</value>
        public const char YesUpper = 'Y';

        /// <summary>
        /// Represents the maximum match percentage allowed. This field is constant.
        /// </summary>
        /// <value>The maximum match percentage allowed</value>
        public const int MaxMatchPercent = 100;

        /// <summary>
        /// Represent the public terminal identifier. This field is read-only.
        /// </summary>
        /// <value>The public terminal identifier</value>
        public static readonly string PublicTerminal = "public";

        /// <summary>
        /// Represents the timestamp format derived from ISO 8601 used in serialization. This field is read-only.
        /// </summary>
        /// <value>The timestamp format derived from ISO 8601 used in serialization</value>
        public static readonly string TimestampFormat = "s";

        /// <summary>
        /// Represents an Aadhaar number formatter.
        /// </summary>
        /// <value>An Aadhaar number formatter</value>
        public static readonly AadhaarNumberFormatter AadhaarFormatInfo = new AadhaarNumberFormatter();

        private static readonly int[,] Multiplication =
        {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
            {2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
            {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
            {4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
            {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
            {6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
            {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
            {8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
            {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
        };

        private static readonly int[,] Permutation =
        {
            {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
            {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
            {5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
            {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
            {9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
            {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
            {2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
            {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
        };

        /// <summary>
        /// Determines whether a 12-digit Aadhaar number is valid.
        /// </summary>
        /// <param name="aadhaarNumber">The number to check.</param>
        /// <returns>true if the number is Verhoeff compliant; otherwise, false.</returns>
        public static bool ValidateAadhaarNumber(string aadhaarNumber)
        {
            if (string.IsNullOrWhiteSpace(aadhaarNumber) || !Regex.IsMatch(aadhaarNumber, @"^\d{12}$"))
                return false;

            var digits = new List<int>(12);
            for (var i = aadhaarNumber.Length - 1; i >= 0; i--)
                digits.Add(aadhaarNumber[i] - '0');

            var checksum = 0;
            for (var i = 0; i < digits.Count; i++)
                checksum = Multiplication[checksum, Permutation[i % 8, digits[i]]];

            return checksum == 0;
        }

        /// <summary>
        /// Determines whether an 6-digit pincode is valid.
        /// </summary>
        /// <param name="pincode">The pincode.</param>
        /// <returns>true if pincode is valid; otherwise, false.</returns>
        public static bool ValidatePincode(string pincode) => !string.IsNullOrWhiteSpace(pincode) && Regex.IsMatch(pincode, @"^\d{6}$");

        /// <summary>
        /// Decodes personal information of a resident from the XML generated by scanning QR code given in UIDAI Aadhaar letter. 
        /// </summary>
        /// <param name="personalInfo">An instance of <see cref="PersonalInfo"/>.</param>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="personalInfo"/> or <paramref cref="element"/> is null.</exception>
        public static void DecodeQRCodeXml(this PersonalInfo personalInfo, XElement element)
        {
            ValidateNull(personalInfo, nameof(personalInfo));
            ValidateNull(element, nameof(element));

            personalInfo.AadhaarNumber = element.Attribute("uid").Value;
            personalInfo.Demographic = new Demographic
            {
                Identity = new Identity(element),
                Address = new Address(element)
            };
        }

        /// <summary>
        /// Encodes all the characters in the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="value">The string containing the characters to encode.</param>
        /// <returns>A byte array containing the results of encoding the specified set of characters.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static byte[] GetBytes(this string value)
        {
            ValidateNull(value, nameof(value));

            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Removes attributes which are empty or contains white spaces from its parent element.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public static void RemoveEmptyAttributes(this XElement element) => ValidateNull(element, nameof(element))
                                                                            .Attributes()
                                                                            .Where(a => string.IsNullOrWhiteSpace(a.Value))
                                                                            .Remove();

        /// <summary>
        /// Encodes all the characters in the specified string into a hexadecimal format.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <returns>A string of hexadecimal value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static string ToHex(this byte[] value)
        {
            ValidateNull(value, nameof(value));

            return BitConverter.ToString(value).Replace("-", string.Empty);
        }

        /// <summary>
        /// Gets the address of the specified API.
        /// </summary>
        /// <param name="agencyInfo">The agency information.</param>
        /// <param name="apiName">Name of the API.</param>
        /// <param name="aadhaarNumber">The Aadhaar number.</param>
        /// <returns>An instance of <see cref="Uri"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="agencyInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiName"/> or AUA code is empty.
        /// Or, hostname is not found in <see cref="UserAgency.Hosts"/>.
        /// </exception>
        public static Uri GetAddress(this UserAgency agencyInfo, string apiName, string aadhaarNumber = null)
        {
            ValidateNull(agencyInfo, nameof(agencyInfo));
            ValidateEmptyString(apiName, nameof(apiName));
            ValidateEmptyString(agencyInfo.AuaCode, nameof(UserAgency.AuaCode));

            Uri host;
            if (!agencyInfo.Hosts.TryGetValue(apiName, out host))
                throw new ArgumentException(NoHostName);

            var encodedAsaLicenseKey = WebUtility.UrlEncode(agencyInfo.AsaLicenseKey) ?? string.Empty;

            return string.IsNullOrWhiteSpace(aadhaarNumber) ?
                new Uri(host, $"{agencyInfo.AuaCode}/{encodedAsaLicenseKey}") :
                new Uri(host, $"{agencyInfo.AuaCode}/{aadhaarNumber[0]}/{aadhaarNumber[1]}/{encodedAsaLicenseKey}");
        }
    }
}