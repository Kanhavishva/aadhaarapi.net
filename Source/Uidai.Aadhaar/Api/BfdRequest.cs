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
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a best finger detection request.
    /// </summary>
    public class BfdRequest : ApiRequest
    {
        /// <summary>
        /// Represents the Bfd version.
        /// </summary>
        public static string BfdVersion = "1.6";

        /// <summary>
        /// Represents the Bfd XML namespace. This field is read-only.
        /// </summary>
        public static readonly XNamespace BfdXmlNamespace = "http://www.uidai.gov.in/authentication/uid-bfd-request/1.0";

        private string aadhaarNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="BfdRequest"/> class.
        /// </summary>
        public BfdRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfdRequest"/> class with a specified best finger detection data received from device.
        /// </summary>
        /// <param name="bfdContext">The finger data received from device.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="bfdContext"/> is null.</exception>
        public BfdRequest(BfdContext bfdContext)
        {
            ValidateNull(bfdContext, nameof(bfdContext));

            AadhaarNumber = bfdContext.AadhaarNumber;
            Data = bfdContext.Data;
            DeviceInfo = bfdContext.DeviceInfo;
            Hmac = bfdContext.Hmac;
            KeyInfo = bfdContext.KeyInfo;
            Terminal = bfdContext.Terminal;
        }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Bfd";

        /// <summary>
        /// Gets or sets the Aadhaar number.
        /// </summary>
        /// <value>The Aadhaar number.</value>
        public string AadhaarNumber
        {
            get { return aadhaarNumber; }
            set { aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber)); }
        }

        /// <summary>
        /// Gets or sets the metadata information of the device.
        /// </summary>
        /// <value>The metadata information of the device.</value>
        public DeviceInfo DeviceInfo { get; set; }

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
        /// Gets or sets the encrypted hash value of the data.
        /// </summary>
        /// <value>The encrypted hash value of the data.</value>
        public string Hmac { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);

            AadhaarNumber = element.Attribute("uid").Value;
            KeyInfo = new SessionKeyInfo(element.Element("Skey"));
            Data = new EncryptedData(element.Element("Data"));
            Hmac = element.Element("Hmac").Value;

            var meta = element.Element("Meta");
            meta.Add(new XAttribute("idc", DeviceInfo.DeviceNotApplicable));
            DeviceInfo = new DeviceInfo(meta);
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><see cref="DeviceInfo"/>, <see cref="KeyInfo"/> or <see cref="Data"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><see cref="AadhaarNumber"/> or <see cref="Hmac"/> is empty.</exception>
        protected override XElement SerializeXml(XName name)
        {
            ValidateNull(DeviceInfo, nameof(DeviceInfo));
            ValidateNull(KeyInfo, nameof(KeyInfo));
            ValidateNull(Data, nameof(Data));
            ValidateEmptyString(AadhaarNumber, nameof(AadhaarNumber));
            ValidateEmptyString(Hmac, nameof(Hmac));

            var bfdRequest = base.SerializeXml(BfdXmlNamespace + name.LocalName);
            var meta = DeviceInfo.ToXml("Meta");
            meta.Attribute("idc").Remove();
            bfdRequest.Add(new XAttribute("uid", AadhaarNumber),
                new XAttribute("ver", BfdVersion),
                meta,
                KeyInfo.ToXml("Skey"),
                Data.ToXml("Data"),
                new XElement("Hmac", Hmac));

            Signer?.ComputeSignature(bfdRequest);

            return bfdRequest;
        }
    }
}