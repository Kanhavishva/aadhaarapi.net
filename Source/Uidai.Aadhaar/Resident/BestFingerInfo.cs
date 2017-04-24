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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents the best finger detection data of a resident.
    /// </summary>
    public class BestFingerInfo : IXml
    {
        /// <summary>
        /// Represents the Rbd version.
        /// </summary>
        /// <value>The Rbd version.</value>
        public static string RbdVersion = "1.0";

        /// <summary>
        /// Represents the Pid XML namespace. This field is read-only.
        /// </summary>
        public static readonly XNamespace RbdXmlNamespace = "http://www.uidai.gov.in/authentication/uid-bfd-request-data/1.0";

        private string aadhaarNumber;

        /// <summary>
        /// Gets or sets the Aadhaar number of the resident.
        /// </summary>
        /// <value>The Aadhaar number of the resident.</value>
        /// <exception cref="ArgumentException">Aadhaar number is invalid.</exception>
        public string AadhaarNumber
        {
            get { return aadhaarNumber; }
            set { aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber)); }
        }

        /// <summary>
        /// Gets or sets the time of capturing the resident data.
        /// Default is <see cref="DateTimeOffset.Now"/>
        /// </summary>
        /// <value>The time of capturing the resident data.</value>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets a collection of finger data of the resident.
        /// </summary>
        /// <value>A collection of finger data.</value>
        public ICollection<TestFinger> Fingers { get; } = new HashSet<TestFinger>();

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="NotSupportedException">The method is not supported.</exception>
        void IXml.FromXml(XElement element)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="Fingers"/> collection cannot be empty. At least one test finger is required.</exception>
        public XElement ToXml(string elementName)
        {
            if (Fingers.Count == 0)
                throw new ArgumentException(RequiredSomeData, nameof(Fingers));

            var bestFingerInfo = new XElement(RbdXmlNamespace + elementName,
                new XAttribute("ts", Timestamp.ToString(AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture)),
                new XAttribute("ver", RbdVersion));
            var testFingers = new XElement("Bios");
            foreach (var testFinger in Fingers)
                testFingers.Add(testFinger.ToXml("Bio"));
            bestFingerInfo.Add(testFingers);

            return bestFingerInfo;
        }
    }
}