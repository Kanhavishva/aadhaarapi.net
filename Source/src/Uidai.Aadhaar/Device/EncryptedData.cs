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

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents encrypted data and its encoding type to be sent in request.
    /// </summary>
    public class EncryptedData : IXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedData"/> class.
        /// </summary>
        public EncryptedData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedData"/> class from an XML.
        /// <param name="element">The XML to deserialize.</param>
        /// </summary>
        public EncryptedData(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the encoding type of the encrypted data.
        /// </summary>
        public EncodingType EncodingType => EncodingType.Xml;

        /// <summary>
        /// Gets or sets the encrypted data in base64 format.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element)
        {
            Data = ValidateNull(element, nameof(element)).Value;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the parent element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            ValidateEmptyString(Data, nameof(Data));

            var encryptedData = new XElement(elementName,
                new XAttribute("type", (char)EncodingType), Data);

            return encryptedData;
        }
    }
}