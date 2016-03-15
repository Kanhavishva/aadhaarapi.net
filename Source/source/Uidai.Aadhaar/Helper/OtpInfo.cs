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
        public string InfoValue { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the Aadhaar number in a hexadecimal format.
        /// </summary>
        public string AadhaarNumberHash { get; set; }

        /// <summary>
        /// Gets or sets the timestamp specified in the PID block.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the hash value of the AUA code in a hexadecimal format.
        /// </summary>
        public string AuaCodeHash { get; set; }

        /// <summary>
        /// Gets or sets the Sub-AUA code.
        /// </summary>
        public string SubAuaCode { get; set; }

        /// <summary>
        /// Gets or sets the masked mobile.
        /// </summary>
        public string MaskedMobile { get; set; }

        /// <summary>
        /// Gets or sets the masked email.
        /// </summary>
        public string MaskedEmail { get; set; }

        /// <summary>
        /// Encodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        public void Encode()
        {
            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number      [01]: timestamp                     [02]: OTP_api_ver,
             * [03]: SHA-256 of ASA code            [04]: SHA-256 of AUA code           [05]: Sub-AUA code,
             * [06]: masked-mobile                  [07]: masked-email
             */

            var infoArray = new string[8];
            for (var i = 0; i < infoArray.Length; i++)
                infoArray[i] = string.Empty;

            infoArray[0] = AadhaarNumberHash ?? string.Empty;
            infoArray[1] = Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
            infoArray[4] = AuaCodeHash ?? string.Empty;
            infoArray[5] = SubAuaCode ?? string.Empty;
            infoArray[6] = MaskedMobile ?? string.Empty;
            infoArray[7] = MaskedMobile ?? string.Empty;

            InfoValue = $"{InfoVersion}{{{string.Join(",", infoArray)}}}";
        }

        /// <summary>
        /// Decodes meta information of authentication data according to Aadhaar API specification.
        /// </summary>
        public void Decode()
        {
            ValidateEmptyString(InfoValue, nameof(InfoValue));

            /*
             * <Version>
             * [00]: SHA-256 of Aadhaar Number      [01]: timestamp                     [02]: OTP_api_ver,
             * [03]: SHA-256 of ASA code            [04]: SHA-256 of AUA code           [05]: Sub-AUA code,
             * [06]: masked-mobile                  [07]: masked-email
             */

            var start = InfoValue.IndexOf('{');
            var end = InfoValue.LastIndexOf('}');
            if (start != -1 && end != -1)
            {
                var infoArray = InfoValue.Substring(start + 1, end - (start + 1)).Split(',');

                AadhaarNumberHash = infoArray[0];
                Timestamp = DateTimeOffset.ParseExact(infoArray[1], TimestampFormat, CultureInfo.InvariantCulture);
                AuaCodeHash = infoArray[4];
                SubAuaCode = infoArray[5];
                MaskedMobile = infoArray[6];
                MaskedEmail = infoArray[7];
            }
        }

        /// <summary>
        /// Determines whether a meta received in response is valid with the generated meta information.
        /// </summary>
        /// <param name="result">The meta information to validate.</param>
        /// <returns>A string array of invalid property names.</returns>
        public string[] Validate(OtpInfo result)
        {
            ValidateNull(result, nameof(result));

            var errorProperties = new List<string>(6);
            if (!AadhaarNumberHash.Equals(result.AadhaarNumberHash))
                errorProperties.Add(nameof(AadhaarNumberHash));
            if (Timestamp != result.Timestamp)
                errorProperties.Add(nameof(Timestamp));
            if (!AuaCodeHash.Equals(result.AuaCodeHash))
                errorProperties.Add(nameof(AuaCodeHash));
            if (!SubAuaCode.Equals(result.SubAuaCode))
                errorProperties.Add(nameof(SubAuaCode));
            if (!MaskedMobile.Equals(result.MaskedMobile))
                errorProperties.Add(nameof(MaskedMobile));
            if (!MaskedEmail.Equals(result.MaskedEmail))
                errorProperties.Add(nameof(MaskedEmail));

            return errorProperties.ToArray();
        }
    }
}