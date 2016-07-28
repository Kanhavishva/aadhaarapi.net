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
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a e-KYC response.
    /// </summary>
    public class KycResponse : AuthResponse
    {
        /// <summary>
        /// Gets or sets the resident information.
        /// </summary>
        /// <value>The resident information.</value>
        public PersonalInfo Resident { get; set; }

        /// <summary>
        /// Gets or sets the time to store resident information.
        /// </summary>
        /// <value>The time to store resident information.</value>
        public DateTimeOffset TimeToLive { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the decryption is to be performed by KSA.
        /// </summary>
        /// <value>A value that indicates whether the decryption is to be performed by KSA.</value>
        public bool IsDecryptionByKsa { get; set; }

        /// <summary>
        /// Gets or sets the digitally signed e-Aadhaar of the resident.
        /// This is useful for applications where a paper print is still needed.
        /// Applications providers are highly encouraged to move away from paper printing and store and use XML data.
        /// </summary>
        /// <value>The digitally signed e-Aadhaar of the resident.</value>
        public AadhaarDocument EAadhaar { get; set; }

        /// <summary>
        /// Gets or sets an decryptor to decrypt the response XML.
        /// Data is not decrypted if <see cref="KycRequest.IsDecryptionByKsa"/> was set to true or if the authentication fails.
        /// </summary>
        /// <value>An instance of decryptor to decrypt the response XML.</value>
        public IKycDecryptor Decryptor { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        protected override void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            IsAuthentic = element.Attribute("ret").Value[0] == AadhaarHelper.Yes;

            if (!IsAuthentic)
            {
                ResponseCode = element.Attribute("code").Value;
                Timestamp = DateTimeOffset.Parse(element.Attribute("ts").Value, CultureInfo.InvariantCulture);
                ErrorCode = element.Attribute("err")?.Value;
                return;
            }

            IsDecryptionByKsa = element.Attribute("ko").Value == "KSA";
            if (!IsDecryptionByKsa)
            {
                var encryptedKycInfo = new EncryptedKycInfo { InfoValue = Convert.FromBase64String(element.Element("kycRes").Value) };
                var decryptedKycInfo = encryptedKycInfo.Decrypt(Decryptor);
                element = decryptedKycInfo.ToXml();
            }

            var authBytes = Convert.FromBase64String(element.Element("Rar").Value);
            using (var stream = new MemoryStream(authBytes))
                base.DeserializeXml(XElement.Load(stream));

            TimeToLive = DateTimeOffset.Parse(element.Attribute("ttl").Value, CultureInfo.InvariantCulture);

            var uidData = element.Element("UidData");
            Resident = new PersonalInfo
            {
                AadhaarNumber = uidData.Attribute("uid").Value,
                Demographic = new Demographic
                {
                    Address = new Address(uidData.Element("Poa")),
                    Identity = new Identity(uidData.Element("Poi"))
                },
                Photo = Convert.FromBase64String(uidData.Element("Pht").Value)
            };
            var localAddress = uidData.Element("LData");
            if (localAddress != null)
            {
                Resident.Demographic.ILAddress = new Address(localAddress);
                Resident.Demographic.LanguageUsed = (IndianLanguage?)int.Parse(localAddress.Attribute("lang").Value);
            }

            var prn = element.Element("Prn");
            // A workaround to support version 1.0. Version checking can be removed once v1.0 becomes obsolete.
            if (prn != null)
                EAadhaar = new AadhaarDocument { Content = prn.Value };
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateNull(Resident, nameof(Resident));
            ValidateNull(Resident.Demographic, nameof(PersonalInfo.Demographic));
            ValidateNull(Resident.Demographic.Address, nameof(Demographic.Address));
            ValidateNull(Resident.Demographic.Identity, nameof(Demographic.Identity));
            ValidateNull(Resident.Photo, nameof(PersonalInfo.Photo));
            ValidateEmptyString(Resident.AadhaarNumber, nameof(PersonalInfo.AadhaarNumber));
            if (Resident.Demographic.ILAddress != null && Resident.Demographic.LanguageUsed == null)
                throw new ArgumentException(RequiredIndianLanguage, nameof(Demographic.LanguageUsed));

            // Create Rar.
            var rar = new XElement("Rar", Convert.ToBase64String(base.SerializeXml("AuthRes").ToString(SaveOptions.DisableFormatting).GetBytes()));

            // Create UidData.
            var poi = Resident.Demographic.Identity.ToXml("Poi");
            var uidData = new XElement("UidData", new XAttribute("uid", Resident.AadhaarNumber),
                poi,
                Resident.Demographic.Address.ToXml("Poa"));
            if (Resident.Demographic.ILAddress != null)
            {
                var ldata = Resident.Demographic.ILAddress.ToXml("LData");
                ldata.Add(new XAttribute("lang", ((int)Resident.Demographic.LanguageUsed).ToString("D2", CultureInfo.InvariantCulture)));
                uidData.Add(ldata);
            }
            uidData.Add(new XElement("Pht", Convert.ToBase64String(Resident.Photo)));

            // Create KycRes.
            var kycResponse = base.SerializeXml(elementName);
            kycResponse.Add(new XAttribute("ttl", TimeToLive), rar, uidData);

            // A workaround to support version 1.0. Version checking can be removed once v1.0 becomes obsolete.
            if (!string.IsNullOrWhiteSpace(EAadhaar?.Content))
                kycResponse.Add(new XElement("Prn", EAadhaar.Content));

            // Remove unnecessary attributes.
            poi.Attribute("dobt")?.Remove();
            kycResponse.Attribute("info").Remove();

            return kycResponse;
        }
    }
}