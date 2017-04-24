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
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of resident data and related information to be used for e-KYC. 
    /// </summary>
    public class KycContext : AuthContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KycContext"/> class.
        /// </summary>
        public KycContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KycContext"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public KycContext(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Kyc";

        /// <summary>
        /// Gets or sets a value that indicates whether resident has consent to access personal data in Indian language.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        /// <value>A value that indicates whether resident has consent to access personal data in Indian language.</value>
        public bool AccessILInfo { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether resident has consent to access mobile and email address.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        /// <value>A value that indicates whether resident has consent to access mobile and email address.</value>
        public bool AccessMobileAndEmail { get; set; }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public new XElement ToXml() => SerializeXml("Kyc");

        /// <summary>
        /// Encrypts data captured for authentication.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <exception cref="ArgumentException">Biometric or OTP data is not used.</exception>
        public override void Encrypt(PersonalInfo data, SessionKey key)
        {
            if (!(data.Uses.AuthUsed.HasFlag(AuthTypes.Biometric) || data.Uses.AuthUsed.HasFlag(AuthTypes.Otp)))
                throw new ArgumentException(RequiredBiometricOrOtp, nameof(data.Uses));

            base.Encrypt(data, key);
        }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);

            var value = element.Attribute("mec")?.Value;
            AccessMobileAndEmail = value != null && value[0] == AadhaarHelper.YesUpper;

            value = element.Attribute("lr")?.Value;
            AccessILInfo = value != null && value[0] == AadhaarHelper.YesUpper;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="HasResidentConsent"/> is set to false.</exception>
        protected override XElement SerializeXml(XName name)
        {
            if (!HasResidentConsent)
                throw new ArgumentException(RequiredConsent, nameof(HasResidentConsent));

            var kycContext = base.SerializeXml(name);
            if (AccessMobileAndEmail)
                kycContext.Add(new XAttribute("mec", AadhaarHelper.YesUpper));
            if (AccessILInfo)
                kycContext.Add(new XAttribute("lr", AadhaarHelper.YesUpper));

            return kycContext;
        }
    }
}