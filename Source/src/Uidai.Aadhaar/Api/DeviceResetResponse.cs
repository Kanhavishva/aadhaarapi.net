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
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a registered device reset response.
    /// </summary>
    public class DeviceResetResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the device reset is successful.
        /// </summary>
        public bool IsReset { get; set; }

        /// <summary>
        /// Gets or sets the device response code.
        /// The value must be passed to registered devices for decrypting and storing the content.
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            IsReset = element.Attribute("ret").Value[0] == AadhaarHelper.Yes;
            DeviceCode = element.Attribute("drd").Value;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(DeviceCode, nameof(DeviceCode));

            var deviceResetResponse = base.SerializeXml(elementName);
            deviceResetResponse.Add(new XAttribute("ret", IsReset ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("drd", DeviceCode));

            return deviceResetResponse;
        }
    }
}