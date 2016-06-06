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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents pin values received by resident for authentication.
    /// </summary>
    public class PinValue : IUsed, IXml
    {
        /// <summary>
        /// Gets or sets the most recently generated one time pin.
        /// </summary>
        public string Otp { get; set; }

        /// <summary>
        /// Gets or sets the actual pin as set by the resident.
        /// This option is not available for AUAs and is restricted to internal UIDAI usage only.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Determines whether a particular resident data is used or not.
        /// </summary>
        /// <returns>true if the data is used; otherwise, false.</returns>
        public bool IsUsed() => !(string.IsNullOrWhiteSpace(Otp) &&
                                  string.IsNullOrWhiteSpace(Pin));

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="NotSupportedException"></exception>
        void IXml.FromXml(XElement element)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            var pinValue = new XElement(elementName,
                new XAttribute("otp", Otp ?? string.Empty),
                new XAttribute("pin", Pin ?? string.Empty));

            pinValue.RemoveEmptyAttributes();

            return pinValue;
        }
    }
}