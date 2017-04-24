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
    /// Represents test finger data of a resident.
    /// </summary>
    public class TestFinger : IEquatable<TestFinger>, IXml
    {
        private int numberOfAttempts = 1;

        /// <summary>
        /// Gets or sets the NIST Fingerprint Image Quality.
        /// Default is <see cref="Nfiq.Excellent"/>.
        /// </summary>
        /// <value>The NIST Fingerprint Image Quality.</value>
        public Nfiq Quality { get; set; } = Nfiq.Excellent;

        /// <summary>
        /// Gets or sets the number of attempts.
        /// Default is 1.
        /// </summary>
        /// <value>The number of attempts.</value>
        public int NumberOfAttempts
        {
            get { return numberOfAttempts; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(NumberOfAttempts), OutOfRangeNumberOfAttempts);
                numberOfAttempts = value;
            }
        }

        /// <summary>
        /// Gets or sets the finger position.
        /// </summary>
        /// <value>The finger position.</value>
        public BiometricPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BiometricType.Minutiae"/> data in base64 format.
        /// </summary>
        /// <value>The biometric data in base64 format.</value>
        public string Data { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(TestFinger other) => other != null && Position == other.Position;

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
        /// <exception cref="ArgumentException">
        /// <see cref="Position"/> is not one of the fingers.
        /// Or, <see cref="Data"/> is empty.
        /// </exception>
        public XElement ToXml(string elementName)
        {
            if (Position == BiometricPosition.Unknown || Position == BiometricPosition.LeftIris || Position == BiometricPosition.RightIris)
                throw new ArgumentException(InvalidBiometricPosition, nameof(Position));
            ValidateEmptyString(Data, nameof(Data));

            var bestFinger = new XElement(elementName,
                new XAttribute("nfiq", (int)Quality),
                new XAttribute("na", NumberOfAttempts),
                new XAttribute("pos", Biometric.BiometricPositionNames[(int)Position]), Data);

            return bestFinger;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => obj != null && GetType() == obj.GetType() && Equals((TestFinger)obj);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => Position.GetHashCode();
    }
}