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
using System.Globalization;
using Uidai.Aadhaar.Device;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Represents the meta information of OTP data.
    /// </summary>
    public class OtpInfo
    {
        /// <summary>
        /// Represents info version. This field is read-only.
        /// </summary>
        public static readonly string InfoVersion = "01";

        private static readonly string TimestampFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Gets or sets the encoded value.
        /// </summary>
        /// <value>The encoded value.</value>
        public string InfoValue { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the Aadhaar number in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of the Aadhaar number in a hexadecimal format.</value>
        public string AadhaarNumberHash { get; set; }

        /// <summary>
        /// Gets or sets the type of request specified.
        /// </summary>
        /// <value>The type of request specified.</value>
        public OtpRequestType RequestType { get; set; }

        /// <summary>
        /// Gets or sets the timestamp specified in the PID block.
        /// </summary>
        /// <value>The timestamp specified in the PID block.</value>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the AUA code in a hexadecimal format.
        /// </summary>
        /// <value>The hash value of the AUA code in a hexadecimal format.</value>
        public string AuaCodeHash { get; set; }

        /// <summary>
        /// Gets or sets the Sub-AUA code.
        /// </summary>
        /// <value>The Sub-AUA code.</value>
        public string SubAuaCodeHash { get; set; }

        /// <summary>
        /// Encodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        /// <returns>The <see cref="InfoValue"/>.</returns>
        public string Encode()
        {
            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number  [01]: request-type          [02]: timestamp
             * [03]: OTP_api_ver                [04]: SHA-256 of ASA code   [05]: SHA-256 of AUA code
             * [06]: Sub-AUA code               [07]: masked-mobile         [08]: masked-email
             */

            var infoArray = new string[9];
            for (var i = 0; i < infoArray.Length; i++)
                infoArray[i] = string.Empty;

            infoArray[0] = AadhaarNumberHash ?? string.Empty;
            infoArray[1] = ((char)RequestType).ToString();
            infoArray[2] = Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
            infoArray[5] = AuaCodeHash ?? string.Empty;
            infoArray[6] = SubAuaCodeHash ?? string.Empty;

            InfoValue = $"{InfoVersion}{{{string.Join(",", infoArray)}}}";

            return InfoValue;
        }

        /// <summary>
        /// Decodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        /// <returns>The current instance of <see cref="OtpInfo"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="InfoValue"/> is empty.</exception>
        public OtpInfo Decode()
        {
            ValidateEmptyString(InfoValue, nameof(InfoValue));

            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number  [01]: request-type          [02]: timestamp
             * [03]: OTP_api_ver                [04]: SHA-256 of ASA code   [05]: SHA-256 of AUA code
             * [06]: Sub-AUA code               [07]: masked-mobile         [08]: masked-email
             */

            var start = InfoValue.IndexOf('{');
            var end = InfoValue.LastIndexOf('}');
            if (start != -1 && end != -1)
            {
                var infoArray = InfoValue.Substring(start + 1, end - (start + 1)).Split(',');

                AadhaarNumberHash = infoArray[0];
                RequestType = (OtpRequestType)infoArray[1][0];
                // Timestamp = DateTimeOffset.ParseExact(infoArray[2], TimestampFormat, CultureInfo.InvariantCulture);
                AuaCodeHash = infoArray[5];
                SubAuaCodeHash = infoArray[6];
            }

            return this;
        }

        /// <summary>
        /// Determines whether a meta received in response is valid with the generated meta information.
        /// </summary>
        /// <param name="result">The meta information to validate.</param>
        /// <returns>A string array of invalid property names.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        public string[] Validate(OtpInfo result)
        {
            ValidateNull(result, nameof(result));

            var errorProperties = new List<string>(5);
            if (!AadhaarNumberHash.Equals(result.AadhaarNumberHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(AadhaarNumberHash));
            if (RequestType != result.RequestType)
                errorProperties.Add(nameof(RequestType));
            if (Timestamp != result.Timestamp)
                errorProperties.Add(nameof(Timestamp));
            if (!AuaCodeHash.Equals(result.AuaCodeHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(AuaCodeHash));
            if (!SubAuaCodeHash.Equals(result.SubAuaCodeHash, StringComparison.OrdinalIgnoreCase))
                errorProperties.Add(nameof(SubAuaCodeHash));

            return errorProperties.ToArray();
        }
    }
}