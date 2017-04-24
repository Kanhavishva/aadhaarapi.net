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
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of resident data and related information to be used for authentication. 
    /// </summary>
    public class AuthContext : DeviceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthContext"/> class.
        /// </summary>
        public AuthContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthContext"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public AuthContext(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Auth";

        /// <summary>
        /// Gets a value that indicates whether resident has consent to access data.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        /// <value>A value that indicates whether resident has consent to access data.</value>
        public bool HasResidentConsent { get; set; }

        /// <summary>
        /// Gets or sets the authentication factors captured.
        /// </summary>
        /// <value>The authentication factors captured.</value>
        public AuthUsage Uses { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>The meta information.</value>
        public AuthInfo AuthInfo { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public Token Token { get; set; }

        /// <summary>
        /// Encrypts data captured for authentication.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException"><see cref="HasResidentConsent"/> is set to false.</exception>
        public virtual void Encrypt(PersonalInfo data, SessionKey key)
        {
            ValidateNull(data, nameof(data));
            ValidateNull(key, nameof(key));
            if (!HasResidentConsent)
                throw new ArgumentException(RequiredConsent, nameof(HasResidentConsent));

            // Create Pid bytes.
            var pidXml = data.ToXml("Pid").ToString(SaveOptions.DisableFormatting);
            var pidBytes = pidXml.GetBytes();

            using (var sha = SHA256.Create())
            {
                // Encrypt data.
                var encryptedPid = key.Encrypt(pidBytes);
                var encryptedPidHash = key.Encrypt(sha.ComputeHash(pidBytes));
                KeyInfo = key.KeyInfo;

                // Set related properties.
                AadhaarNumber = data.AadhaarNumber;
                Timestamp = data.Timestamp;
                Uses = data.Uses;
                Data = new EncryptedData { Data = Convert.ToBase64String(encryptedPid) };
                Hmac = Convert.ToBase64String(encryptedPidHash);
                if (DeviceInfo != null)
                {
                    if (data.Biometrics.All(b => b.Type != BiometricType.Iris))
                        DeviceInfo.IrisDeviceCode = DeviceInfo.DeviceNotApplicable;
                    if (!data.Biometrics.Any(b => b.Type == BiometricType.Fingerprint || b.Type == BiometricType.Minutiae))
                        DeviceInfo.FingerprintDeviceCode = DeviceInfo.DeviceNotApplicable;
                }

                // Set meta information.
                if (AuthInfo == null)
                    AuthInfo = new AuthInfo();
                AuthInfo.Timestamp = data.Timestamp;

                const string demoStart = "<Demo", demoEnd = "</Demo";
                var startOfDemoStart = pidXml.LastIndexOf(demoStart, StringComparison.Ordinal);
                if (startOfDemoStart >= 0)
                {
                    var startOfDemoEnd = pidXml.IndexOf(demoEnd, startOfDemoStart + demoStart.Length, StringComparison.Ordinal);
                    var realEnd = pidXml.IndexOf(">", startOfDemoEnd + demoEnd.Length, StringComparison.Ordinal);
                    var demoXml = pidXml.Substring(startOfDemoStart, realEnd - startOfDemoStart + 1);
                    var demoBytes = demoXml.GetBytes();
                    var demoHash = sha.ComputeHash(demoBytes);
                    AuthInfo.DemographicHash = demoHash.ToHex();
                    Array.Clear(demoBytes, 0, demoBytes.Length);
                }
            }
            Array.Clear(pidBytes, 0, pidBytes.Length);
        }

        /// <summary>
        /// Asynchronously encrypts data captured for request.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <returns>A task that represents the asynchronous encrypt operation.</returns>
        public async virtual Task EncryptAsync(PersonalInfo data, SessionKey key) => await Task.Run(() => Encrypt(data, key));

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            HasResidentConsent = element.Attribute("rc").Value[0] == AadhaarHelper.YesUpper;
            Uses = new AuthUsage(element.Element("Uses"));

            var info = element.Attribute("info")?.Value;
            AuthInfo = info != null ? new AuthInfo { InfoValue = info } : null;

            var tkn = element.Element("Tkn");
            Token = tkn != null ? new Token(tkn) : null;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentNullException"><see cref="Uses"/> is null.</exception>
        protected override XElement SerializeXml(XName name)
        {
            ValidateNull(Uses, nameof(Uses));

            var authContext = base.SerializeXml(name);
            authContext.Add(new XAttribute("rc", AadhaarHelper.YesUpper),
                Uses.ToXml("Uses"));

            if (AuthInfo != null)
                authContext.Add(new XAttribute("info", AuthInfo.Encode()));
            if (Token != null)
                authContext.Add(Token.ToXml("Tkn"));

            return authContext;
        }
    }
}