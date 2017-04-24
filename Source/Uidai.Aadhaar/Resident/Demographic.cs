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
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents demographic information of a resident.
    /// </summary>
    public class Demographic : IXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Demographic"/> class.
        /// </summary>
        public Demographic() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Demographic"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public Demographic(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets Indian language used in <see cref="Identity.ILName"/> and <see cref="FullAddress.ILAddress"/>.
        /// </summary>
        /// <value>The Indian language used.</value>
        public IndianLanguage? LanguageUsed { get; set; }

        /// <summary>
        /// Gets or sets the identity of the resident.
        /// </summary>
        /// <value>The identity of the resident.</value>
        public Identity Identity { get; set; }

        /// <summary>
        /// Gets or sets the address of the resident.
        /// <see cref="Address"/> and <see cref="FullAddress"/> cannot be used in same transaction.
        /// </summary>
        /// <value>The address of the resident.</value>
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets the address of the resident in Indian language.
        /// This property is only populated in e-KYC response and is excluded from serialization and deserialization.
        /// </summary>
        /// <value>The address of the resident in Indian language.</value>
        public Address ILAddress { get; set; }

        /// <summary>
        /// Gets or sets the full address of the resident.
        /// <see cref="Address"/> and <see cref="FullAddress"/> cannot be used in same transaction.
        /// </summary>
        /// <value>The full address of the resident.</value>
        public FullAddress FullAddress { get; set; }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            var xml = element.Element("Pa");
            Address = xml != null ? new Address(xml) : null;

            xml = element.Element("Pfa");
            FullAddress = xml != null ? new FullAddress(xml) : null;

            xml = element.Element("Pi");
            Identity = xml != null ? new Identity(xml) : null;

            var lang = element.Attribute("lang")?.Value;
            LanguageUsed = lang != null ? (IndianLanguage?)int.Parse(lang) : null;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Identity.ILName"/> or <see cref="FullAddress.ILAddress"/> has value but <see cref="LanguageUsed"/> is not set.
        /// Or <see cref="Address"/> and <see cref="FullAddress"/> are both set.
        /// </exception>
        public XElement ToXml(string elementName)
        {
            var isILUsed = !(string.IsNullOrWhiteSpace(Identity?.ILName) && string.IsNullOrWhiteSpace(FullAddress?.ILAddress));

            if (LanguageUsed == null && isILUsed)
                throw new ArgumentException(RequiredIndianLanguage, nameof(LanguageUsed));
            if (Address != null && FullAddress != null)
                throw new ArgumentException(XorAddresses);

            var demographic = new XElement(elementName);
            if (isILUsed)
                demographic.Add(new XAttribute("lang", ((int)LanguageUsed).ToString("D2", CultureInfo.InvariantCulture)));
            if (Identity != null)
                demographic.Add(Identity.ToXml("Pi"));
            if (Address != null)
                demographic.Add(Address.ToXml("Pa"));
            if (FullAddress != null)
                demographic.Add(FullAddress.ToXml("Pfa"));

            return demographic;
        }
    }
}