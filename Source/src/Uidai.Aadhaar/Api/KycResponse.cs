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
        public PersonalInfo Resident { get; set; }

        /// <summary>
        /// Gets or sets the time to store resident information.
        /// </summary>
        public DateTimeOffset TimeToLive { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            var authBytes = Convert.FromBase64String(element.Element("Rar").Value);
            using (var stream = new MemoryStream(authBytes))
            {
                var authXml = XElement.Load(stream);
                base.DeserializeXml(authXml);
            }
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
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateNull(Resident?.Demographic?.Address, nameof(Demographic.Address));
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

            // Remove unnecessary attributes.
            poi.Attribute("dobt")?.Remove();
            kycResponse.Attribute("info").Remove();

            return kycResponse;
        }
    }
}