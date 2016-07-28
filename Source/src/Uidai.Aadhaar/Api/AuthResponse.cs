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

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents an authentication response.
    /// </summary>
    public class AuthResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the authentication passed.
        /// </summary>
        /// <value>A value that indicates whether the authentication passed.</value>
        public bool IsAuthentic { get; set; }

        /// <summary>
        /// Gets or sets the action code which are published from time to time meant to be shown to resident/operator.
        /// </summary>
        /// <value>The action code which are published from time to time meant to be shown to resident/operator.</value>
        public string ActionCode { get; set; }

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>The meta information.</value>
        public AuthInfo Info { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
            base.DeserializeXml(element);
            IsAuthentic = element.Attribute("ret").Value[0] == AadhaarHelper.Yes;
            ActionCode = element.Attribute("actn")?.Value;
            Info = new AuthInfo { InfoValue = element.Attribute("info").Value };
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="System.ArgumentException"><see cref="AuthInfo.InfoValue"/> is empty.</exception>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(Info?.InfoValue, nameof(AuthInfo.InfoValue));

            var authResponse = base.SerializeXml(elementName);
            authResponse.Add(new XAttribute("ret", IsAuthentic ? AadhaarHelper.Yes : AadhaarHelper.No),
                new XAttribute("info", Info.InfoValue));
            if (!string.IsNullOrWhiteSpace(ActionCode))
                authResponse.Add(new XAttribute("actn", ActionCode));

            return authResponse;
        }
    }
}