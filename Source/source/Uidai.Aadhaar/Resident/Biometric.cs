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
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Represents biometric data of a resident.
    /// Biometric data should never be should never be stored anywhere.
    /// </summary>
    public class Biometric : IEquatable<Biometric>, IUsed, IXml
    {
        internal static readonly string[] BiometricTypeNames = { "FIR", "FMR", "IIR" };
        internal static readonly string[] BiometricPositionNames = { "LEFT_IRIS", "RIGHT_IRIS", "LEFT_INDEX", "LEFT_LITTLE", "LEFT_MIDDLE", "LEFT_RING", "LEFT_THUMB", "RIGHT_INDEX", "RIGHT_LITTLE", "RIGHT_MIDDLE", "RIGHT_RING", "RIGHT_THUMB", "UNKNOWN" };

        /// <summary>
        /// Gets or sets the type of biometric data.
        /// </summary>
        public BiometricType Type { get; set; }

        /// <summary>
        /// Gets or sets the position of biometric data. 
        /// </summary>
        public BiometricPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the biometric data in base64 format.
        /// In case of registered device, this contains encrypted biometric record. 
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type by comparing <see cref="Type"/> and <see cref="Position"/>.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(Biometric other)
        {
            return other != null && Type == other.Type && Position == other.Position;
        }

        /// <summary>
        /// Determines whether a particular resident data is used or not.
        /// </summary>
        /// <returns>true if the data is used; otherwise, false.</returns>
        public bool IsUsed()
        {
            return !string.IsNullOrWhiteSpace(Data);
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
            if ((Type == BiometricType.Iris && Position != BiometricPosition.LeftIris && Position != BiometricPosition.RightIris && Position != BiometricPosition.Unknown) ||
                (Type != BiometricType.Iris && (Position == BiometricPosition.LeftIris || Position == BiometricPosition.RightIris)))
                throw new ArgumentException(InvalidBiometricPosition, nameof(Position));
            ValidateEmptyString(Data, nameof(Data));

            var biometric = new XElement(elementName,
                new XAttribute("type", BiometricTypeNames[(int)Type]),
                new XAttribute("posh", BiometricPositionNames[(int)Position]), Data);

            return biometric;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && Equals((Biometric)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Position.GetHashCode();
        }
    }
}