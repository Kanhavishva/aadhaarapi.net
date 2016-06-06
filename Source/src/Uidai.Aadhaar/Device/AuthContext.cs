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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of resident data and related information to be used for authentication. 
    /// </summary>
    public class AuthContext : DeviceContext<PersonalInfo>
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
        public override string ApiName => "Auth";

        /// <summary>
        /// Gets or sets the authentication factors captured.
        /// </summary>
        public AuthUsage Uses { get; set; }

        /// <summary>
        /// Gets or sets meta information.
        /// Meta information is only calculated in <see cref="Encrypt(PersonalInfo, SessionKey)"/> if this property is initialized.
        /// </summary>
        public AuthInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// Encrypts data captured for authentication.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        public override void Encrypt(PersonalInfo data, SessionKey key)
        {
            ValidateNull(data, nameof(data));
            ValidateNull(key, nameof(key));

            // Create Pid bytes.
            var pidXml = data.ToXml().ToString(SaveOptions.DisableFormatting);
            var pidBytes = pidXml.GetBytes();

            using (var sha = SHA256.Create())
            {
                // Encrypt data.
                var encryptedPid = key.Encrypt(pidBytes);
                var encryptedPidHash = key.Encrypt(sha.ComputeHash(pidBytes));
                KeyInfo = key.KeyInfo;

                // Set related properties.
                AadhaarNumber = data.AadhaarNumber;
                Uses = data.Uses;
                Data = new EncryptedData { Data = Convert.ToBase64String(encryptedPid) };
                Hmac = Convert.ToBase64String(encryptedPidHash);
                Timestamp = data.Timestamp;
                if (DeviceInfo != null)
                {
                    if (data.Biometrics.All(b => b.Type != BiometricType.Iris))
                        DeviceInfo.IrisDeviceCode = Metadata.DeviceNotApplicable;
                    if (!data.Biometrics.Any(b => b.Type == BiometricType.Fingerprint || b.Type == BiometricType.Minutiae))
                        DeviceInfo.FingerprintDeviceCode = Metadata.DeviceNotApplicable;
                }

                // Set meta information.
                if (Info != null)
                {
                    Info.AadhaarNumberHash = sha.ComputeHash(AadhaarNumber.GetBytes()).ToHex();
                    Info.Timestamp = data.Timestamp;
                    Info.TerminalHash = sha.ComputeHash(Terminal.GetBytes()).ToHex();

                    const string demoStart = "<Demo", demoEnd = "</Demo";
                    var startOfDemoStart = pidXml.LastIndexOf(demoStart, StringComparison.Ordinal);
                    if (startOfDemoStart >= 0)
                    {
                        var startOfDemoEnd = pidXml.IndexOf(demoEnd, startOfDemoStart + demoStart.Length, StringComparison.Ordinal);
                        var realEnd = pidXml.IndexOf(">", startOfDemoEnd + demoEnd.Length, StringComparison.Ordinal);
                        var demoXml = pidXml.Substring(startOfDemoStart, realEnd - startOfDemoStart + 1);
                        var demoBytes = demoXml.GetBytes();
                        var demoHash = sha.ComputeHash(demoBytes);
                        Info.DemographicHash = demoHash.ToHex();
                        Array.Clear(demoBytes, 0, demoBytes.Length);
                    }
                    Info.Encode();
                }
            }
            Array.Clear(pidBytes, 0, pidBytes.Length);
        }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            Uses = new AuthUsage(element.Element("Uses"));

            var info = element.Attribute("info")?.Value;
            Info = info != null ? new AuthInfo { InfoValue = info } : null;

            var tkn = element.Element("Tkn");
            Token = tkn != null ? new Token(tkn) : null;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateNull(Uses, nameof(Uses));

            var authContext = base.SerializeXml(elementName);
            authContext.Add(Uses.ToXml("Uses"));
            if (Info != null)
                authContext.Add(new XAttribute("info", Info.InfoValue));
            if (Token != null)
                authContext.Add(Token.ToXml("Tkn"));

            return authContext;
        }
    }
}