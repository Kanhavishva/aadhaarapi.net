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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Represents session key info used to encrypt data.
    /// </summary>
    public class SessionKeyInfo : IXml
    {
        /// <summary>
        /// Represents the certificate identifier format used in serialization. This field is read-only.
        /// </summary>
        public static readonly string CertificateIdentifierFormat = "yyyyMMdd";

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionKeyInfo"/> class.
        /// </summary>
        public SessionKeyInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionKeyInfo"/> class from an XML.
        /// <param name="element">The XML to deserialize.</param>
        /// </summary>
        public SessionKeyInfo(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the expiry date to identify certificate used to encrypt the session key.
        /// </summary>
        /// <value>The expiry date to identify certificate used to encrypt the session key.</value>
        public DateTimeOffset CertificateIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the encrypted session key in base64 format.
        /// </summary>
        /// <value>The encrypted session key in base64 format.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the key identifier of the synchronized session key.
        /// </summary>
        /// <value>The key identifier of the synchronized session key.</value>
        public Guid KeyIdentifier { get; set; }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            CertificateIdentifier = DateTimeOffset.ParseExact(element.Attribute("ci").Value, CertificateIdentifierFormat, CultureInfo.InvariantCulture);
            Key = element.Value;
            var ki = element.Attribute("ki")?.Value;
            KeyIdentifier = ki != null ? Guid.Parse(ki) : Guid.Empty;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            var sessionKey = new XElement(elementName,
                new XAttribute("ci", CertificateIdentifier.ToString(CertificateIdentifierFormat, CultureInfo.InvariantCulture)), Key);
            if (KeyIdentifier != Guid.Empty)
                sessionKey.Add(new XAttribute("ki", KeyIdentifier));

            return sessionKey;
        }
    }
}