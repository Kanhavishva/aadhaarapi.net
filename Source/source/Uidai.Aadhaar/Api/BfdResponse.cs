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
using Uidai.Aadhaar.Resident;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a best finger detection response.
    /// </summary>
    public class BfdResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets an actionable feedback message in English in case resident or operator needs to take specific actions.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the action code which are published from time to time meant to be shown to resident/operator.
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// Gets a collection of ranks associated with each finger that was part of input. 
        /// </summary>
        public SortedList<int, BiometricPosition> Ranks { get; } = new SortedList<int, BiometricPosition>();

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            ActionCode = element.Attribute("actn").Value;
            Message = element.Attribute("msg").Value;
            foreach (var rank in element.Element("Ranks").Elements())
            {
                var value = int.Parse(rank.Attribute("val").Value, CultureInfo.InvariantCulture);
                var index = Array.IndexOf(Biometric.BiometricPositionNames, rank.Attribute("pos").Value);
                Ranks.Add(value, (BiometricPosition)index);
            }
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(ActionCode, nameof(ActionCode));
            ValidateEmptyString(Message, nameof(Message));
            if (Ranks.Any(r => r.Value == BiometricPosition.LeftIris || r.Value == BiometricPosition.RightIris || r.Value == BiometricPosition.Unknown))
                throw new ArgumentException(InvalidBiometricPosition, nameof(Ranks));

            var bfdResponse = base.SerializeXml(elementName);
            bfdResponse.Add(new XAttribute("actn", ActionCode),
                new XAttribute("msg", Message));
            if (Ranks.Count > 0)
            {
                var ranks = new XElement("Ranks");
                foreach (var rank in Ranks)
                    ranks.Add(new XElement("Rank", new XAttribute("pos", Biometric.BiometricPositionNames[(int)rank.Value]), new XAttribute("val", rank.Key)));
                bfdResponse.Add(ranks);
            }

            return bfdResponse;
        }
    }
}