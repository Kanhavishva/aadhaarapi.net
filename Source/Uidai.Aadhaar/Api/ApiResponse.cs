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
using System.Security.Cryptography;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Provides an abstract representation of common properties specific to all type of responses. This is an abstract class.
    /// </summary>
    /// <seealso cref="AuthResponse"/>
    /// <seealso cref="BfdResponse"/>
    /// <seealso cref="OtpResponse"/>
    /// <seealso cref="KycResponse"/>
    public abstract class ApiResponse : IXml
    {
        /// <summary>
        /// Gets or sets the unique alphanumeric response code.
        /// </summary>
        /// <value>The unique alphanumeric response code.</value>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier specified during request.
        /// </summary>
        /// <value>The transaction identifier specified during request.</value>
        public Transaction Transaction { get; set; }

        /// <summary>
        /// Gets or sets the date time when the response is generated.
        /// </summary>
        /// <value>The date time when the response is generated.</value>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the error code if request fails.
        /// </summary>
        /// <value>The error code if request fails.</value>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets an instance of <see cref="IVerifier"/> to verify signature of signed XML.
        /// </summary>
        /// <value>An instance of <see cref="IVerifier"/> to verify signature of signed XML.</value>
        public IVerifier Verifier { get; set; }

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
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        /// <exception cref="CryptographicException">Signature is invalid.</exception>
        protected virtual void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            if (Verifier?.VerifySignature(element) == false)
                throw new CryptographicException(InvalidSignature);

            ResponseCode = element.Attribute("code").Value;
            Transaction = element.Attribute("txn").Value;
            Timestamp = DateTimeOffset.Parse(element.Attribute("ts").Value, CultureInfo.InvariantCulture);
            ErrorCode = element.Attribute("err")?.Value;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="ResponseCode"/> or <see cref="Transaction"/> is empty.</exception>
        protected virtual XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(ResponseCode, nameof(ResponseCode));
            ValidateEmptyString(Transaction, nameof(Transaction));

            var apiResponse = new XElement(elementName,
                new XAttribute("code", ResponseCode),
                new XAttribute("txn", Transaction),
                new XAttribute("ts", Timestamp),
                new XAttribute("err", ErrorCode ?? string.Empty));

            return apiResponse;
        }
    }
}