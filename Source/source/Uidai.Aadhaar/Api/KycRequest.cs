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
using System.IO;
using System.Xml.Linq;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represent an e-KYC request.
    /// </summary>
    public class KycRequest : AuthRequest
    {
        /// <summary>
        /// Represents the KYC version. This field is read-only.
        /// </summary>
        public static readonly string KycVersion = "1.0";

        /// <summary>
        /// Initializes a new instance of the <see cref="KycRequest"/> class.
        /// </summary>
        public KycRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KycRequest"/> class with a specified KYC data along with other details.
        /// </summary>
        /// <param name="kycContext">The authentication data received from device.</param>
        public KycRequest(KycContext kycContext) : base(kycContext)
        {
            ValidateNull(kycContext, nameof(kycContext));

            AccessILInfo = kycContext.AccessILInfo;
            AccessMobileAndEmail = kycContext.AccessMobileAndEmail;
            HasResidentConsent = kycContext.HasResidentConsent;
            Timestamp = kycContext.Timestamp;
            Transaction.Prefix = "UKC:";
        }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        public override string ApiName => "Kyc";

        /// <summary>
        /// Gets or sets the time of capturing the resident data.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether resident has consent to retrieve personal data in Indian language.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        public bool AccessILInfo { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether resident has consent to retrieve mobile and email address.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        public bool AccessMobileAndEmail { get; set; }

        /// <summary>
        /// Gets a value that indicates whether resident has consent to access data.
        /// Applications must take explicit informed resident consent and value should not be hard-coded under any circumstances.
        /// </summary>
        public bool HasResidentConsent { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the decryption is to be performed by KSA.
        /// </summary>
        public bool IsDecryptionByKsa { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            var authBytes = Convert.FromBase64String(element.Element("Rad").Value);
            using (var stream = new MemoryStream(authBytes))
            {
                var authXml = XElement.Load(stream);
                base.DeserializeXml(authXml);
            }
            Timestamp = DateTimeOffset.ParseExact(element.Attribute("ts").Value, AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture);
            HasResidentConsent = element.Attribute("rc").Value[0] == AadhaarHelper.YesUpper;

            var value = element.Attribute("mec")?.Value;
            AccessMobileAndEmail = value != null && value[0] == AadhaarHelper.YesUpper;

            value = element.Attribute("lr")?.Value;
            AccessILInfo = value != null && value[0] == AadhaarHelper.YesUpper;

            value = element.Attribute("de")?.Value;
            IsDecryptionByKsa = value != null && value[0] == AadhaarHelper.YesUpper;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            if (!((string)Transaction).StartsWith("UKC:"))
                throw new ArgumentException(InvalidTransactionPrefix, nameof(Transaction));
            if (!HasResidentConsent)
                throw new ArgumentException(RequiredConsent, nameof(HasResidentConsent));

            var authXml = base.SerializeXml(base.ApiName);
            var authBase64 = Convert.ToBase64String(authXml.ToString(SaveOptions.DisableFormatting).GetBytes());
            var ra = string.Empty;
            if (Uses.Biometrics.Contains(BiometricType.Fingerprint) || Uses.Biometrics.Contains(BiometricType.Minutiae))
                ra += 'F';
            if (Uses.Biometrics.Contains(BiometricType.Iris))
                ra += 'I';
            if (Uses.AuthUsed.HasFlag(AuthTypes.Otp))
                ra += 'O';

            var kycRequest = new XElement(elementName,
                new XAttribute("ver", KycVersion),
                new XAttribute("ts", Timestamp.ToString(AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture)),
                new XAttribute("ra", ra),
                new XAttribute("rc", AadhaarHelper.YesUpper),
                new XElement("Rad", authBase64));
            if (AccessMobileAndEmail)
                kycRequest.Add(new XAttribute("mec", AadhaarHelper.YesUpper));
            if (AccessILInfo)
                kycRequest.Add(new XAttribute("lr", AadhaarHelper.YesUpper));
            if (IsDecryptionByKsa)
                kycRequest.Add(new XAttribute("de", AadhaarHelper.YesUpper));

            Signer?.ComputeSignature(kycRequest);

            return kycRequest;
        }
    }
}