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
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents authentication factors captured from a resident.
    /// </summary>
    public class AuthUsage : IXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthUsage"/> class.
        /// </summary>
        public AuthUsage() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthUsage"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public AuthUsage(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the authentication factors captured.
        /// </summary>
        public AuthTypes AuthUsed { get; set; }

        /// <summary>
        /// Gets a collection of biometric types captured.
        /// Collection cannot be empty if <see cref="AuthTypes.Biometric"/> flag is set in <see cref="AuthUsed"/>.
        /// </summary>
        public ICollection<BiometricType> Biometrics { get; } = new HashSet<BiometricType>();

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            AuthUsed = AuthTypes.None;
            Biometrics.Clear();

            if (element.Attribute("pi").Value[0] == AadhaarHelper.Yes)
                AuthUsed |= AuthTypes.Identity;
            if (element.Attribute("pa").Value[0] == AadhaarHelper.Yes)
                AuthUsed |= AuthTypes.Address;
            if (element.Attribute("pfa").Value[0] == AadhaarHelper.Yes)
                AuthUsed |= AuthTypes.FullAddress;
            if (element.Attribute("otp").Value[0] == AadhaarHelper.Yes)
                AuthUsed |= AuthTypes.Otp;
            if (element.Attribute("pin").Value[0] == AadhaarHelper.Yes)
                AuthUsed |= AuthTypes.Pin;
            if (element.Attribute("bio").Value[0] == AadhaarHelper.Yes)
            {
                AuthUsed |= AuthTypes.Biometric;
                foreach (var index in element.Attribute("bt").Value.Split(',').Select(b => Array.IndexOf(Biometric.BiometricTypeNames, b)))
                    Biometrics.Add((BiometricType)index);
            }
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            if (AuthUsed.HasFlag(AuthTypes.Biometric) && Biometrics.Count == 0)
                throw new ArgumentException(RequiredBiometricsUsed, nameof(Biometrics));

            var authUsage = new XElement(elementName,
                new XAttribute("pi", AuthUsed.HasFlag(AuthTypes.Identity) ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("pa", AuthUsed.HasFlag(AuthTypes.Address) ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("pfa", AuthUsed.HasFlag(AuthTypes.FullAddress) ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("bio", AuthUsed.HasFlag(AuthTypes.Biometric) ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("otp", AuthUsed.HasFlag(AuthTypes.Otp) ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("pin", AuthUsed.HasFlag(AuthTypes.Pin) ? AadhaarHelper.Yes : AadhaarHelper.No));
            if (AuthUsed.HasFlag(AuthTypes.Biometric))
            {
                var bt = string.Join(",", Biometrics.Select(b => Biometric.BiometricTypeNames[(int)b]));
                authUsage.Add(new XAttribute("bt", bt));
            }

            return authUsage;
        }
    }
}