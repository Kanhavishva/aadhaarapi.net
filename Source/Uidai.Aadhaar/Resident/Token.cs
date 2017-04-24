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

using System.Globalization;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents information used as a valid token which is provided to resident such as mobile phone, NFC token, Smart Card, etc.
    /// </summary>
    public class Token : IXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public Token(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public TokenType TokenType { get; set; } = TokenType.MobileNumber;

        /// <summary>
        /// Gets or sets the value of the token.
        /// </summary>
        /// <value>The value of the token.</value>
        public string Value { get; set; }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element)
        {
            Value = ValidateNull(element, nameof(element)).Attribute("value").Value;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="System.ArgumentException"><see cref="Value"/> is empty.</exception>
        public XElement ToXml(string elementName)
        {
            ValidateEmptyString(Value, nameof(Value));

            var token = new XElement(elementName,
                new XAttribute("type", ((int)TokenType).ToString("D3", CultureInfo.InvariantCulture)),
                new XAttribute("value", Value));

            return token;
        }
    }
}