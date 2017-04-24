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
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents an authentication request.
    /// </summary>
    public class AuthRequest : ApiRequest
    {
        /// <summary>
        /// Represents the Auth version.
        /// </summary>
        /// <value>The Auth version.</value>
        public static string AuthVersion = "1.6";

        /// <summary>
        /// Represents the Auth XML namespace. This field is read-only.
        /// </summary>
        public static readonly XNamespace AuthXmlNamespace = "http://www.uidai.gov.in/authentication/uid-auth-request/1.0";

        private string aadhaarNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRequest"/> class.
        /// </summary>
        public AuthRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRequest"/> class with a specified authentication data received from device.
        /// </summary>
        /// <param name="authContext">The authentication data received from device.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="authContext"/> is null.</exception>
        public AuthRequest(AuthContext authContext)
        {
            ValidateNull(authContext, nameof(authContext));

            AadhaarNumber = authContext.AadhaarNumber;
            HasResidentConsent = authContext.HasResidentConsent;
            Data = authContext.Data;
            DeviceInfo = authContext.DeviceInfo;
            Hmac = authContext.Hmac;
            KeyInfo = authContext.KeyInfo;
            Terminal = authContext.Terminal;
            Uses = authContext.Uses;
            AuthInfo = authContext.AuthInfo;
        }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Auth";

        /// <summary>
        /// Gets or sets the Aadhaar number.
        /// </summary>
        /// <value>The Aadhaar number.</value>
        /// <exception cref="System.ArgumentException">Aadhaar number is invalid.</exception>
        public string AadhaarNumber
        {
            get { return aadhaarNumber; }
            set { aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber)); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether resident has consent to access data.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        /// <value>A value that indicates whether resident has consent to access data.</value>
        public bool HasResidentConsent { get; set; }

        /// <summary>
        /// Gets or sets the authentication factors captured by device.
        /// </summary>
        /// <value>The authentication factors captured by device.</value>
        public AuthUsage Uses { get; set; }

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
        /// Gets or sets the encrypted hash value of the data in base64 format.
        /// </summary>
        /// <value>The encrypted hash value of the data in base64 format.</value>
        public string Hmac { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public Token Token { get; set; }

        /// <summary>
        /// Gets or sets any meta information received from device.
        /// This property is excluded from serialization and deserialization.
        /// </summary>
        /// <value>The meta information received from device.</value>
        public AuthInfo AuthInfo { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);

            AadhaarNumber = element.Attribute("uid").Value;
            HasResidentConsent = element.Attribute("rc").Value[0] == AadhaarHelper.YesUpper;
            Uses = new AuthUsage(element.Element("Uses"));
            DeviceInfo = new DeviceInfo(element.Element("Meta"));
            KeyInfo = new SessionKeyInfo(element.Element("Skey"));
            Data = new EncryptedData(element.Element("Data"));
            Hmac = element.Element("Hmac").Value;

            var tkn = element.Element("Tkn");
            Token = tkn != null ? new Token(tkn) : null;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><see cref="Uses"/>, <see cref="DeviceInfo"/>, <see cref="KeyInfo"/> or <see cref="Data"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><see cref="AadhaarNumber"/> or <see cref="Hmac"/> is empty.</exception>
        protected override XElement SerializeXml(XName name)
        {
            ValidateNull(Uses, nameof(Uses));
            ValidateNull(DeviceInfo, nameof(DeviceInfo));
            ValidateNull(KeyInfo, nameof(KeyInfo));
            ValidateNull(Data, nameof(Data));
            ValidateEmptyString(AadhaarNumber, nameof(AadhaarNumber));
            ValidateEmptyString(Hmac, nameof(Hmac));

            var authRequest = base.SerializeXml(AuthXmlNamespace + name.LocalName);
            authRequest.Add(new XAttribute("uid", AadhaarNumber),
                new XAttribute("ver", AuthVersion),
                new XAttribute("rc", AadhaarHelper.YesUpper),
                Uses.ToXml("Uses"),
                DeviceInfo.ToXml("Meta"),
                KeyInfo.ToXml("Skey"),
                Data.ToXml("Data"),
                new XElement("Hmac", Hmac));
            if (Token != null)
                authRequest.Add(Token.ToXml("Tkn"));

            Signer?.ComputeSignature(authRequest);

            return authRequest;
        }
    }
}