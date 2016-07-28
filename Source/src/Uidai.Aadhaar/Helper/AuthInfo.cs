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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Uidai.Aadhaar.Resident;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Represents the meta information of authentication data.
    /// </summary>
    public class AuthInfo
    {
        /// <summary>
        /// Represents the info version. This field is read-only.
        /// </summary>
        /// <value>The info version.</value>
        public static readonly string InfoVersion = "02";

        private static readonly string TimestampFormat = "yyyyMMddHHmmss";

        private static readonly Lazy<UsageMatcher[]> Usage = new Lazy<UsageMatcher[]>(() => new[]
        {
            // 2nd, 9th Hexadecimal Digit
            new UsageMatcher {UseLocation = 4, MatchLocation = 32, Name = nameof(Identity.Name)},
            new UsageMatcher {UseLocation = 5, MatchLocation = 33, Name = nameof(Identity.ILName)},
            new UsageMatcher {UseLocation = 6, MatchLocation = 34, Name = nameof(Identity.Gender)},
            new UsageMatcher {UseLocation = 7, MatchLocation = 35, Name = nameof(Identity.DateOfBirth)},

            // 3rd, 10th Hexadecimal Digit
            new UsageMatcher {UseLocation = 8, MatchLocation = 36, Name = nameof(Identity.Phone)},
            new UsageMatcher {UseLocation = 9, MatchLocation = 37, Name = nameof(Identity.Email)},
            new UsageMatcher {UseLocation = 10, MatchLocation = 38, Name = nameof(Identity.Age)},
            new UsageMatcher {UseLocation = 11, MatchLocation = 39, Name = nameof(Address.CareOf)},

            // 4th, 11th Hexadecimal Digit
            new UsageMatcher {UseLocation = 12, MatchLocation = 40, Name = nameof(Address.House)},
            new UsageMatcher {UseLocation = 13, MatchLocation = 41, Name = nameof(Address.Street)},
            new UsageMatcher {UseLocation = 14, MatchLocation = 42, Name = nameof(Address.Landmark)},
            new UsageMatcher {UseLocation = 15, MatchLocation = 43, Name = nameof(Address.Locality)},

            // 5th, 12th Hexadecimal Digit
            new UsageMatcher {UseLocation = 16, MatchLocation = 44, Name = nameof(Address.VillageOrCity)},
            new UsageMatcher {UseLocation = 17, MatchLocation = 45, Name = nameof(Address.District)},
            new UsageMatcher {UseLocation = 18, MatchLocation = 46, Name = nameof(Address.State)},
            new UsageMatcher {UseLocation = 19, MatchLocation = 47, Name = nameof(Address.Pincode)},

            // 6th, 13th Hexadecimal Digit
            new UsageMatcher {UseLocation = 20, MatchLocation = 48, Name = nameof(FullAddress.Address)},
            new UsageMatcher {UseLocation = 21, MatchLocation = 49, Name = nameof(FullAddress.ILAddress)},
            new UsageMatcher {UseLocation = 22, MatchLocation = 50, Name = nameof(BiometricType.Minutiae)},
            new UsageMatcher {UseLocation = 23, MatchLocation = 50, Name = nameof(BiometricType.Fingerprint)},

            // 7th, 13th Hexadecimal Digit
            new UsageMatcher {UseLocation = 24, MatchLocation = 51, Name = nameof(BiometricType.Iris)},

            // 8th, 14th Hexadecimal Digit
            new UsageMatcher {UseLocation = 28, MatchLocation = 52, Name = nameof(Address.PostOffice)},
            new UsageMatcher {UseLocation = 29, MatchLocation = 53, Name = nameof(Address.SubDistrict)},
            new UsageMatcher {UseLocation = 30, MatchLocation = 54, Name = nameof(Identity.DoBType)}
        });

        /// <summary>
        /// Gets or sets the encoded value.
        /// </summary>
        /// <value>The encoded value.</value>
        public string InfoValue { get; set; }

        /// <summary>
        /// Gets or sets the encoded usage data in a 60-bit hexadecimal format.
        /// </summary>
        /// <value>The encoded usage data in a 60-bit hexadecimal format.</value>
        public string UsageData { get; set; }

        /// <summary>
        /// Gets or sets the hash value of Aadhaar number in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of Aadhaar number in a hexadecimal format.</value>
        public string AadhaarNumberHash { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the demographic block in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of the demographic block in a hexadecimal format.</value>
        public string DemographicHash { get; set; }

        /// <summary>
        /// Gets or sets the timestamp specified in the PID block.
        /// </summary>
        /// <value>The timestamp specified in the PID block.</value>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the Terminal identifier in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of the Terminal identifier in a hexadecimal format.</value>
        public string TerminalHash { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the AUA code in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of the AUA code in a hexadecimal format.</value>
        public string AuaCodeHash { get; set; }

        /// <summary>
        /// Gets or sets the Sub-AUA code.
        /// </summary>
        /// <value>The Sub-AUA code.</value>
        public string SubAuaCode { get; set; }

        /// <summary>
        /// Encodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        public void Encode()
        {
            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number      [01]: SHA-256 of Demo element       [02]: Encoded Usage Data
             * [03]: pid_version                    [04]: timestamp                     [05]: fmrcount
             * [06]: fircount                       [07]: iircount                      [08]: auth_api_ver
             * [09]: SHA-256 of ASA code            [10]: SHA-256 of AUA code           [11]: SUB AUA code
             * [12]: lot                            [13]: lov                           [14]: lang
             * [15]: pi-ms                          [16]: pi-mv                         [17]: pi-lmv
             * [18]: pa-ms                          [19]: pa-mv                         [20]: pa-lmv
             * [21]: pfa-ms                         [22]: pfa-mv                        [23]: pfa-lmv
             * [24]: SHA-256 of tid
             */

            var infoArray = new string[25];
            for (var i = 0; i < infoArray.Length; i++)
                infoArray[i] = string.Empty;

            infoArray[0] = AadhaarNumberHash ?? string.Empty;
            infoArray[1] = DemographicHash ?? string.Empty;
            infoArray[2] = UsageData ?? string.Empty;
            infoArray[4] = Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
            infoArray[10] = AuaCodeHash ?? string.Empty;
            infoArray[11] = SubAuaCode ?? string.Empty;
            infoArray[24] = TerminalHash ?? string.Empty;

            InfoValue = $"{InfoVersion}{{{string.Join(",", infoArray)}}}";
        }

        /// <summary>
        /// Decodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        /// <exception cref="ArgumentException"><see cref="InfoValue"/> is empty.</exception>
        public void Decode()
        {
            ValidateEmptyString(InfoValue, nameof(InfoValue));

            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number      [01]: SHA-256 of Demo element       [02]: Encoded Usage Data
             * [03]: pid_version                    [04]: timestamp                     [05]: fmrcount
             * [06]: fircount                       [07]: iircount                      [08]: auth_api_ver
             * [09]: SHA-256 of ASA code            [10]: SHA-256 of AUA code           [11]: SUB AUA code
             * [12]: lot                            [13]: lov                           [14]: lang
             * [15]: pi-ms                          [16]: pi-mv                         [17]: pi-lmv
             * [18]: pa-ms                          [19]: pa-mv                         [20]: pa-lmv
             * [21]: pfa-ms                         [22]: pfa-mv                        [23]: pfa-lmv
             * [24]: SHA-256 of tid
             */

            var start = InfoValue.IndexOf('{');
            var end = InfoValue.LastIndexOf('}');
            if (start != -1 && end != -1)
            {
                var infoArray = InfoValue.Substring(start + 1, end - (start + 1)).Split(',');

                AadhaarNumberHash = infoArray[0];
                DemographicHash = infoArray[1];
                UsageData = infoArray[2];
                Timestamp = DateTimeOffset.ParseExact(infoArray[4], TimestampFormat, CultureInfo.InvariantCulture);
                AuaCodeHash = infoArray[10];
                SubAuaCode = infoArray[11];
                TerminalHash = infoArray[24];
            }
        }

        /// <summary>
        /// Determines whether a meta received in response is valid with the generated meta information.
        /// </summary>
        /// <param name="authInfo">The meta information to validate.</param>
        /// <returns>A string array of property names whose values do not match with <paramref name="authInfo"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="authInfo"/> is null.</exception>
        public string[] Validate(AuthInfo authInfo)
        {
            ValidateNull(authInfo, nameof(authInfo));

            var errorProperties = new List<string>(6);
            if (!AadhaarNumberHash.Equals(authInfo.AadhaarNumberHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(AadhaarNumberHash));
            if (!DemographicHash.Equals(authInfo.DemographicHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(DemographicHash));
            if (Timestamp != authInfo.Timestamp)
                errorProperties.Add(nameof(Timestamp));
            if (!AuaCodeHash.Equals(authInfo.AuaCodeHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(AuaCodeHash));
            if (!SubAuaCode.Equals(authInfo.SubAuaCode))
                errorProperties.Add(nameof(SubAuaCode));
            if (!TerminalHash.Equals(authInfo.TerminalHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(TerminalHash));

            return errorProperties.ToArray();
        }

        /// <summary>
        /// Returns <see cref="UsageData"/> wrapped in a <see cref="BitArray"/> of size 60 to allow easy comparison of usage data.
        /// </summary>
        /// <returns>An instance of <see cref="BitArray"/></returns>
        /// <exception cref="ArgumentException"><see cref="UsageData"/> is empty.</exception>
        public BitArray GetUsageData()
        {
            ValidateEmptyString(UsageData, nameof(UsageData));

            var values = Convert.ToString(Convert.ToInt64(UsageData, 16), 2)
                .PadLeft(60, '0')
                .Select(b => b == '1')
                .ToArray();

            return new BitArray(values);
        }

        /// <summary>
        /// Returns a collection of names of authentication data that caused error.
        /// CIDR servers generate errors as soon as one mismatch is found, therefore, one invalid authentication data may result in multiple invalid entries.
        /// Applications should review data as per error code in following order:
        /// <see cref="PersonalInfo.AadhaarNumber"/>,
        /// <see cref="Identity"/>,
        /// <see cref="Address"/>,
        /// <see cref="FullAddress"/>,
        /// <see cref="PinValue"/>,
        /// <see cref="Biometric"/>.
        /// </summary>
        /// <returns>A string array of invalid property names.</returns>
        public IEnumerable<string> GetMismatch()
        {
            var bitArray = GetUsageData();

            return Usage.Value.Where(u => bitArray[u.UseLocation] && !bitArray[u.MatchLocation])
                .Select(u => u.Name)
                .OrderBy(u => u);
        }

        private class UsageMatcher
        {
            public int MatchLocation, UseLocation;
            public string Name;
        }
    }
}