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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents personal information of a resident.
    /// </summary>
    public class PersonalInfo : IXml
    {
        /// <summary>
        /// Represents Pid version. This field is read-only.
        /// </summary>
        public static readonly string PidVersion = "1.0";

        private string aadhaarNumber;

        /// <summary>
        /// Gets or sets the Aadhaar number.
        /// </summary>
        public string AadhaarNumber
        {
            get { return aadhaarNumber; }
            set { aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber)); }
        }

        /// <summary>
        /// Gets or sets the time of capturing the resident data.
        /// Default is <see cref="DateTimeOffset.Now"/>
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the demographic data of the resident.
        /// </summary>
        public Demographic Demographic { get; set; }

        /// <summary>
        /// Gets a collection of biometric data of the resident.
        /// <see cref="BiometricType.Fingerprint"/> and <see cref="BiometricType.Minutiae"/> cannot be used in same transaction.
        /// </summary>
        public ICollection<Biometric> Biometrics { get; } = new HashSet<Biometric>();

        /// <summary>
        /// Gets or sets the OTP received by the resident.
        /// </summary>
        public PinValue PinValue { get; set; }

        /// <summary>
        /// Gets or sets the photo of the resident.
        /// This property is only used in e-KYC response.
        /// </summary>
        public byte[] Photo { get; set; }

        /// <summary>
        /// Gets the authentication factors captured.
        /// </summary>
        public AuthUsage Uses
        {
            get
            {
                var uses = new AuthUsage { AuthUsed = AuthTypesUsed };
                var biometricTypes = Biometrics.Select(b => b.Type).Distinct();
                foreach (var biometricType in biometricTypes)
                    uses.Biometrics.Add(biometricType);
                return uses;
            }
        }

        private AuthTypes AuthTypesUsed
        {
            get
            {
                var authTypes = AuthTypes.None;
                if (Demographic != null)
                {
                    if (Demographic.Identity?.IsUsed() == true)
                        authTypes |= AuthTypes.Identity;
                    if (Demographic.Address?.IsUsed() == true)
                        authTypes |= AuthTypes.Address;
                    if (Demographic.FullAddress?.IsUsed() == true)
                        authTypes |= AuthTypes.FullAddress;
                }
                if (Biometrics.Count > 0 && Biometrics.Any(b => b.IsUsed()))
                    authTypes |= AuthTypes.Biometric;
                if (PinValue != null)
                {
                    if (!string.IsNullOrWhiteSpace(PinValue.Pin))
                        authTypes |= AuthTypes.Pin;
                    if (!string.IsNullOrWhiteSpace(PinValue.Otp))
                        authTypes |= AuthTypes.Otp;
                }
                return authTypes;
            }
        }

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="NotSupportedException"></exception>
        void IXml.FromXml(XElement element)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            if (Uses.AuthUsed == AuthTypes.None)
                throw new ArgumentException(RequiredSomeData);
            if (Biometrics.Any(b => b.Type == BiometricType.Fingerprint) && Biometrics.Any(b => b.Type == BiometricType.Minutiae))
                throw new ArgumentException(XorFirFmr, nameof(Biometrics));

            var personalInfo = new XElement(elementName,
                new XAttribute("ts", Timestamp.ToString(AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture)),
                new XAttribute("ver", PidVersion));
            if (Demographic != null)
                personalInfo.Add(Demographic.ToXml("Demo"));
            if (Biometrics.Count > 0)
            {
                var biometrics = new XElement("Bios");
                foreach (var biometric in Biometrics)
                    biometrics.Add(biometric.ToXml("Bio"));
                personalInfo.Add(biometrics);
            }
            if (PinValue != null)
                personalInfo.Add(PinValue.ToXml("Pv"));

            return personalInfo;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml() => ToXml("Pid");
    }
}