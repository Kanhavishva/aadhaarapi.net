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
using System.Threading.Tasks;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of resident data and related information. This is an abstract class.
    /// </summary>
    /// <typeparam name="T">The type of data to be encrypted.</typeparam>
    /// <seealso cref="AuthContext"/>
    /// <seealso cref="BfdContext"/>
    /// <seealso cref="KycContext"/>
    /// <seealso cref="OtpContext"/>
    public abstract class DeviceContext<T> : IXml where T : IXml
    {
        private string aadhaarNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContext{T}"/> class.
        /// </summary>
        protected DeviceContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContext{T}"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        protected DeviceContext(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public abstract string ApiName { get; }

        /// <summary>
        /// Gets or sets the Aadhaar number of the resident.
        /// </summary>
        /// <value>The Aadhaar number of the resident.</value>
        /// <exception cref="ArgumentException">Aadhaar number is invalid.</exception>
        public string AadhaarNumber
        {
            get { return aadhaarNumber; }
            set { aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber)); }
        }

        /// <summary>
        /// Gets or sets the terminal identifier.
        /// Default is <see cref="AadhaarHelper.PublicTerminal"/>.
        /// </summary>
        /// <value>The terminal identifier.</value>
        public string Terminal { get; set; } = AadhaarHelper.PublicTerminal;

        /// <summary>
        /// Gets or sets the time of capturing the resident data.
        /// </summary>
        /// <value>The timestamp of capturing the resident data.</value>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the metadata information of the device.
        /// </summary>
        /// <value>The metadata information of the device.</value>
        public Metadata DeviceInfo { get; set; }

        /// <summary>
        /// Gets or sets the session key info used to encrypt data.
        /// </summary>
        /// <value>The session key info used to encrypt data.</value>
        public SessionKeyInfo KeyInfo { get; set; }

        /// <summary>
        /// Gets or sets the encrypted data.
        /// </summary>
        /// <value>The encrypted data.</value>
        public EncryptedData Data { get; set; }

        /// <summary>
        /// Gets or sets the encrypted hash value of the data in base64 format.
        /// </summary>
        /// <value>The encrypted hash value of the data in base64 format.</value>
        public string Hmac { get; set; }

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
        /// When overridden in a descendant class, encrypts resident data.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        public abstract void Encrypt(T data, SessionKey key);

        /// <summary>
        /// Asynchronously encrypts data captured for request.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <returns>A task that represents the asynchronous encrypt operation.</returns>
        public async Task EncryptAsync(T data, SessionKey key) => await Task.Run(() => Encrypt(data, key));

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        protected virtual void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            AadhaarNumber = element.Attribute("uid").Value;
            Terminal = element.Attribute("tid").Value;
            Timestamp = DateTimeOffset.ParseExact(element.Attribute("ts").Value, AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture);
            DeviceInfo = new Metadata(element.Element("Meta"));
            KeyInfo = new SessionKeyInfo(element.Element("Skey"));
            Data = new EncryptedData(element.Element("Data"));
            Hmac = element.Element("Hmac").Value;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentNullException"><see cref="DeviceInfo"/>, <see cref="KeyInfo"/> or <see cref="Data"/> is null.</exception>
        /// <exception cref="ArgumentException"><see cref="AadhaarNumber"/> or <see cref="Hmac"/> is empty.</exception>
        protected virtual XElement SerializeXml(string elementName)
        {
            ValidateNull(DeviceInfo, nameof(DeviceInfo));
            ValidateNull(KeyInfo, nameof(KeyInfo));
            ValidateNull(Data, nameof(Data));
            ValidateEmptyString(AadhaarNumber, nameof(AadhaarNumber));
            ValidateEmptyString(Hmac, nameof(Hmac));

            var deviceContext = new XElement(elementName,
                new XAttribute("uid", AadhaarNumber),
                new XAttribute("tid", Terminal),
                new XAttribute("ts", Timestamp.ToString(AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture)),
                DeviceInfo.ToXml("Meta"),
                KeyInfo.ToXml("Skey"),
                Data.ToXml("Data"),
                new XElement("Hmac", Hmac));

            return deviceContext;
        }
    }
}