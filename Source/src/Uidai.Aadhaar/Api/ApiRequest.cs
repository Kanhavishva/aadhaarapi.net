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
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Provides an abstract representation of common properties specific to all type of requests. This is an abstract class.
    /// </summary>
    /// <seealso cref="AuthRequest"/>
    /// <seealso cref="BfdRequest"/>
    /// <seealso cref="OtpRequest"/>
    /// <seealso cref="KycRequest"/>
    public abstract class ApiRequest : IXml
    {
        private string auaCode, subAuaCode, auaLicenseKey;
        private Transaction transaction = new Transaction();

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public abstract string ApiName { get; }

        /// <summary>
        /// Gets or sets the terminal identifier.
        /// </summary>
        /// <value>The terminal identifier.</value>
        public string Terminal { get; set; }

        /// <summary>
        /// Gets or sets the AUA code.
        /// Maximum length is 10 characters.
        /// </summary>
        /// <value>The AUA code.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 10 characters.</exception>
        public string AuaCode
        {
            get { return auaCode; }
            set
            {
                if (value?.Length > 10)
                    throw new ArgumentOutOfRangeException(nameof(AuaCode), OutOfRangeAuaCode);
                auaCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the Sub-AUA code.
        /// Default is <see cref="AuaCode"/>.
        /// Maximum length is 10 characters.
        /// </summary>
        /// <value>The Sub-AUA code.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 10 characters.</exception>
        public string SubAuaCode
        {
            get { return !string.IsNullOrWhiteSpace(subAuaCode) ? subAuaCode : auaCode; }
            set
            {
                if (value?.Length > 10)
                    throw new ArgumentOutOfRangeException(nameof(SubAuaCode), OutOfRangeAuaCode);
                subAuaCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the transaction identifier.
        /// Maximum length is 50 characters.
        /// </summary>
        /// <value>The transaction identifier.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 50 characters.</exception>
        public Transaction Transaction
        {
            get { return transaction; }
            set
            {
                if (((string)value)?.Length > 50)
                    throw new ArgumentOutOfRangeException(nameof(Transaction), OutOfRangeTransaction);
                transaction = value;
            }
        }

        /// <summary>
        /// Gets or sets the AUA license key.
        /// Maximum length is 64 characters.
        /// </summary>
        /// <value>The AUA license key.</value>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than 64 characters.</exception>
        public string AuaLicenseKey
        {
            get { return auaLicenseKey; }
            set
            {
                if (value?.Length > 64)
                    throw new ArgumentOutOfRangeException(nameof(AuaLicenseKey), OutOfRangeAuaLicenseKey);
                auaLicenseKey = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of <see cref="ISigner"/> to sign XML.
        /// Digital signing should always be performed by the entity that creates the final request XML.
        /// Derived class should therefore sign XML in their overridden <see cref="SerializeXml(string)"/> method.
        /// </summary>
        /// <value>An instance of <see cref="ISigner"/> to sign XML.</value>
        public ISigner Signer { get; set; }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element) => DeserializeXml(element);

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName) => SerializeXml(elementName);

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml() => ToXml(ApiName);

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        protected virtual void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            Terminal = element.Attribute("tid").Value;
            AuaCode = element.Attribute("ac").Value;
            SubAuaCode = element.Attribute("sa").Value;
            Transaction = element.Attribute("txn").Value;
            AuaLicenseKey = element.Attribute("lk").Value;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="Terminal"/>, <see cref="AuaCode"/>, <see cref="Transaction"/> or <see cref="AuaLicenseKey"/> is empty.</exception>
        protected virtual XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(Terminal, nameof(Terminal));
            ValidateEmptyString(AuaCode, nameof(AuaCode));
            ValidateEmptyString(Transaction, nameof(Transaction));
            ValidateEmptyString(AuaLicenseKey, nameof(AuaLicenseKey));

            var request = new XElement(elementName,
                new XAttribute("tid", Terminal),
                new XAttribute("ac", AuaCode),
                new XAttribute("sa", SubAuaCode),
                new XAttribute("txn", Transaction),
                new XAttribute("lk", AuaLicenseKey));

            return request;
        }
    }
}