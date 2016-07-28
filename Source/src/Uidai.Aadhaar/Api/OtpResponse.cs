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

using System.Xml.Linq;
using Uidai.Aadhaar.Helper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a one time pin response.
    /// </summary>
    public class OtpResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value that indicates whether OTP is sent to resident successfully.
        /// </summary>
        /// <value>A value that indicates whether OTP is sent to resident successfully.</value>
        public bool IsOtpSent { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>The meta information.</value>
        public OtpInfo Info { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            IsOtpSent = element.Attribute("ret").Value[0] == AadhaarHelper.Yes;

            var info = element.Attribute("info")?.Value;
            Info = info != null ? new OtpInfo { InfoValue = info } : null;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            var otpResponse = base.SerializeXml(elementName);
            otpResponse.Add(new XAttribute("ret", IsOtpSent ? AadhaarHelper.Yes : AadhaarHelper.No));
            if (!string.IsNullOrWhiteSpace(Info?.InfoValue))
                otpResponse.Add(new XAttribute("info", Info.InfoValue));

            return otpResponse;
        }
    }
}