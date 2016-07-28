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
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents full address of a resident.
    /// </summary>
    public class FullAddress : IUsed, IXml
    {
        private int matchPercent = AadhaarHelper.MaxMatchPercent, ilMatchPercent = AadhaarHelper.MaxMatchPercent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FullAddress"/> class.
        /// </summary>
        public FullAddress() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullAddress"/> class from an XML.
        /// <param name="element">The XML to deserialize.</param>
        /// </summary>
        public FullAddress(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the full address.
        /// </summary>
        /// <value>The full address.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the full address in local Indian language.
        /// </summary>
        /// <value>The full address in local Indian language.</value>
        public string ILAddress { get; set; }

        /// <summary>
        /// Gets or sets the matching strategy of the full addresses. 
        /// Default is <see cref="MatchingStrategy.Partial"/>.
        /// </summary>
        /// <value>The matching strategy of the full addresses.</value>
        public MatchingStrategy Match { get; set; } = MatchingStrategy.Partial;

        /// <summary>
        /// Gets or sets the partial match value of the full address.
        /// Used only when <see cref="Match"/> is set to <see cref="MatchingStrategy.Partial"/>.
        /// Valid values are in the range 1 - 100.
        /// Default is 100.
        /// </summary>
        /// <value>The partial match value of the full address.</value>
        public int MatchPercent
        {
            get { return matchPercent; }
            set { matchPercent = ValidateMatchPercent(value, nameof(MatchPercent)); }
        }

        /// <summary>
        /// Gets or sets the partial match value of address in Indian language.
        /// Used only when <see cref="Match"/> is set to <see cref="MatchingStrategy.Partial"/>.
        /// Valid values are in the range 1 - 100.
        /// Default is 100.
        /// </summary>
        /// <value>The partial match value of the full address in Indian language.</value>
        public int ILMatchPercent
        {
            get { return ilMatchPercent; }
            set { ilMatchPercent = ValidateMatchPercent(value, nameof(ILMatchPercent)); }
        }

        /// <summary>
        /// Determines whether a particular resident data is used or not.
        /// </summary>
        /// <returns>true if the data is used; otherwise, false.</returns>
        public bool IsUsed() => !(string.IsNullOrWhiteSpace(Address) &&
                                  string.IsNullOrWhiteSpace(ILAddress));

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="element"/> is null.</exception>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            Address = element.Attribute("av")?.Value;
            ILAddress = element.Attribute("lav")?.Value;

            var value = element.Attribute("ms")?.Value;
            Match = value != null ? (MatchingStrategy)value[0] : MatchingStrategy.Exact;
            if (Match == MatchingStrategy.Partial)
            {
                MatchPercent = int.Parse(element.Attribute("mv").Value);
                ILMatchPercent = int.Parse(element.Attribute("lmv").Value);
            }
            else
                MatchPercent = ILMatchPercent = AadhaarHelper.MaxMatchPercent;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            var fullAddress = new XElement(elementName,
                new XAttribute("av", Address ?? string.Empty),
                new XAttribute("lav", ILAddress ?? string.Empty));
            if (Match == MatchingStrategy.Partial && !(string.IsNullOrWhiteSpace(Address) && string.IsNullOrWhiteSpace(ILAddress)))
                fullAddress.Add(new XAttribute("ms", (char)Match),
                    new XAttribute("mv", MatchPercent),
                    new XAttribute("lmv", ILMatchPercent));

            fullAddress.RemoveEmptyAttributes();

            return fullAddress;
        }

        /// <summary>
        /// Returns a string that represents the full address.
        /// The full address in Indian language is returned if address in English is empty.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Address))
                return Address;
            if (!string.IsNullOrWhiteSpace(ILAddress))
                return ILAddress;
            return base.ToString();
        }
    }
}