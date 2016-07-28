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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents personal identity of a resident.
    /// </summary>
    public class Identity : IUsed, IXml
    {
        /// <summary>
        /// Represents the birth date format used in serialization. This field is read-only.
        /// </summary>
        /// <value>The birth date format used in serialization.</value>
        public static readonly string BirthDateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Represents the birth year format used in serialization. This field is read-only.
        /// </summary>
        /// <value>The birth year format used in serialization</value>
        public static readonly string BirthYearFormat = "yyyy";

        private int age, ilNameMatchPercent = AadhaarHelper.MaxMatchPercent, nameMatchPercent = AadhaarHelper.MaxMatchPercent;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Identity"/> class.
        /// </summary>
        public Identity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identity"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public Identity(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets or sets the name of the resident.
        /// Maximum length is 60 characters.
        /// </summary>
        /// <value>The name of the resident.</value>
        public string Name
        {
            get { return name; }
            set
            {
                if (value?.Length > 60)
                    throw new ArgumentOutOfRangeException(nameof(Name), OutOfRangeName);
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the resident in local Indian language.
        /// </summary>
        /// <value>The name of the resident in local Indian language.</value>
        public string ILName { get; set; }

        /// <summary>
        /// Gets or sets the gender of the resident.
        /// </summary>
        /// <value>The gender of the resident.</value>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the resident.
        /// </summary>
        /// <value>The date of birth of the resident.</value>
        public DateTimeOffset? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the age of the resident.
        /// Valid values are in the range 1 - 150; otherwise 0.
        /// Authentication will pass if resident's age is equal to or greater than the input age.
        /// </summary>
        /// <value>The age of the resident.</value>
        public int Age
        {
            get { return age; }
            set
            {
                if (value < 0 || value > 150)
                    throw new ArgumentOutOfRangeException(nameof(Age), OutOfRangeAge);
                age = value;
            }
        }

        /// <summary>
        /// Gets or sets the email of the resident.
        /// </summary>
        /// <value>The email of the resident.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the resident.
        /// </summary>
        /// <value>The phone number of the resident.</value>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the matching strategy of names.
        /// Default is <see cref="MatchingStrategy.Exact"/>.
        /// </summary>
        /// <value>The matching strategy of names.</value>
        public MatchingStrategy Match { get; set; } = MatchingStrategy.Exact;

        /// <summary>
        /// Gets or sets the partial match percent of name.
        /// Used only when <see cref="Match"/> is set to <see cref="MatchingStrategy.Partial"/>.
        /// Valid values are in the range 1 - 100.
        /// Default is 100.
        /// </summary>
        /// <value>The match percent of the name.</value>
        public int NameMatchPercent
        {
            get { return nameMatchPercent; }
            set { nameMatchPercent = ValidateMatchPercent(value, nameof(NameMatchPercent)); }
        }

        /// <summary>
        /// Gets or sets the partial match percent of name in Indian language.
        /// Used only when <see cref="Match"/> is set to <see cref="MatchingStrategy.Partial"/>.
        /// Valid values are in the range 1 - 100.
        /// Default is 100.
        /// </summary>
        /// <value>The match percent of name in Indian language.</value>
        public int ILNameMatchPercent
        {
            get { return ilNameMatchPercent; }
            set { ilNameMatchPercent = ValidateMatchPercent(value, nameof(ILNameMatchPercent)); }
        }

        /// <summary>
        /// Gets or sets the date of birth type specified during registration.
        /// </summary>
        /// <value>The date of birth type.</value>
        public DateOfBirthType? DoBType { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to verify only birth year.
        /// </summary>
        /// <value>A value that indicates whether to verify only birth year.</value>
        public bool VerifyOnlyBirthYear { get; set; }

        /// <summary>
        /// Determines whether a particular resident data is used or not.
        /// </summary>
        /// <returns>true if the data is used; otherwise, false.</returns>
        public bool IsUsed() => !(string.IsNullOrWhiteSpace(Name) &&
                                  string.IsNullOrWhiteSpace(ILName) &&
                                  string.IsNullOrWhiteSpace(Phone) &&
                                  string.IsNullOrWhiteSpace(Email) &&
                                  Gender == null &&
                                  DateOfBirth == null &&
                                  Age == 0);

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            Name = element.Attribute("name")?.Value;
            ILName = element.Attribute("lname")?.Value;
            Phone = element.Attribute("phone")?.Value;
            Email = element.Attribute("email")?.Value;
            Gender = (Gender?)element.Attribute("gender")?.Value[0];

            var value = element.Attribute("dob")?.Value;
            if (value != null)
            {
                VerifyOnlyBirthYear = value.Length == BirthYearFormat.Length;
                DateOfBirth = DateTimeOffset.ParseExact(value, VerifyOnlyBirthYear ? BirthYearFormat : BirthDateFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                VerifyOnlyBirthYear = false;
                DateOfBirth = null;
            }

            value = element.Attribute("age")?.Value;
            Age = value != null ? int.Parse(value) : 0;

            value = element.Attribute("ms")?.Value;
            Match = value != null ? (MatchingStrategy)value[0] : MatchingStrategy.Exact;
            if (Match == MatchingStrategy.Partial)
            {
                NameMatchPercent = int.Parse(element.Attribute("mv").Value);
                ILNameMatchPercent = int.Parse(element.Attribute("lmv").Value);
            }
            else
                NameMatchPercent = ILNameMatchPercent = AadhaarHelper.MaxMatchPercent;
            DoBType = (DateOfBirthType?)element.Attribute("dobt")?.Value[0];
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            var identity = new XElement(elementName,
                new XAttribute("name", Name ?? string.Empty),
                new XAttribute("lname", ILName ?? string.Empty),
                new XAttribute("phone", Phone ?? string.Empty),
                new XAttribute("email", Email ?? string.Empty));
            if (Gender != null)
                identity.Add(new XAttribute("gender", (char)Gender));
            if (DateOfBirth != null)
                identity.Add(new XAttribute("dob", DateOfBirth.Value.ToString(VerifyOnlyBirthYear ? BirthYearFormat : BirthDateFormat, CultureInfo.InvariantCulture)));
            if (Age != 0)
                identity.Add(new XAttribute("age", Age));

            if (Match == MatchingStrategy.Partial && !(string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(ILName)))
            {
                identity.Add(new XAttribute("ms", (char)Match),
                    new XAttribute("mv", NameMatchPercent),
                    new XAttribute("lmv", ILNameMatchPercent));
            }
            if (DoBType != null && (DateOfBirth != null || Age != 0))
                identity.Add(new XAttribute("dobt", (char)DoBType));

            identity.RemoveEmptyAttributes();

            return identity;
        }
    }
}