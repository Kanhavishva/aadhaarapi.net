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

namespace Uidai.Aadhaar.Resident
{
    /// <summary>
    /// Specifies the authentication factors.
    /// </summary>
    [Flags]
    public enum AuthTypes
    {
        /// <summary>
        /// Indicates no authentication factors used.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates identity data used.
        /// </summary>
        Identity = 1,
        /// <summary>
        /// Indicates address in parts used.
        /// </summary>
        Address = 2,
        /// <summary>
        /// Indicates full address used.
        /// </summary>
        FullAddress = 4,
        /// <summary>
        /// Indicates biometric data used.
        /// </summary>
        Biometric = 8,
        /// <summary>
        /// Indicates pin used.
        /// </summary>
        Pin = 16,
        /// <summary>
        /// Indicates one time pin used.
        /// </summary>
        Otp = 32
    }

    /// <summary>
    /// Specifies biometric position.
    /// </summary>
    public enum BiometricPosition
    {
        /// <summary>
        /// Indicates left iris.
        /// </summary>
        LeftIris,
        /// <summary>
        /// Indicates right iris.
        /// </summary>
        RightIris,
        /// <summary>
        /// Indicates left index finger.
        /// </summary>
        LeftIndex,
        /// <summary>
        /// Indicates left little finger.
        /// </summary>
        LeftLittle,
        /// <summary>
        /// Indicates left middle finger.
        /// </summary>
        LeftMiddle,
        /// <summary>
        /// Indicates left ring finger.
        /// </summary>
        LeftRing,
        /// <summary>
        /// Indicates left thumb finger.
        /// </summary>
        LeftThumb,
        /// <summary>
        /// Indicates right index finger.
        /// </summary>
        RightIndex,
        /// <summary>
        /// Indicates right little finger.
        /// </summary>
        RightLittle,
        /// <summary>
        /// Indicates right middle finger.
        /// </summary>
        RightMiddle,
        /// <summary>
        /// Indicates right ring finger.
        /// </summary>
        RightRing,
        /// <summary>
        /// Indicates right thumb finger.
        /// </summary>
        RightThumb,
        /// <summary>
        /// Indicates unknown biometric position.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Specifies biometric type.
    /// </summary>
    public enum BiometricType
    {
        /// <summary>
        /// Indicates fingerprint.
        /// </summary>
        Fingerprint,
        /// <summary>
        /// Indicates fingerprint minutiae.
        /// </summary>
        Minutiae,
        /// <summary>
        /// Indicates iris.
        /// </summary>
        Iris
    }

    /// <summary>
    /// Specifies the date of birth type of a resident.
    /// </summary>
    public enum DateOfBirthType
    {
        /// <summary>
        /// Indicates approximate date of birth.
        /// </summary>
        Approximate = 'A',
        /// <summary>
        /// Indicates declared date of birth.
        /// </summary>
        Declared = 'D',
        /// <summary>
        /// Indicates verified date of birth.
        /// </summary>
        Verified = 'V'
    }

    /// <summary>
    /// Specifies the gender of a resident.
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Indicates female.
        /// </summary>
        Female = 'F',
        /// <summary>
        /// Indicates male.
        /// </summary>
        Male = 'M',
        /// <summary>
        /// Indicates transgender.
        /// </summary>
        Transgender = 'T'
    }

    /// <summary>
    /// Specifies the Indian language used in capturing resident data.
    /// </summary>
    public enum IndianLanguage
    {
        /// <summary>
        /// Indicates Assamese.
        /// </summary>
        Assamese = 1,
        /// <summary>
        /// Indicates Bengali.
        /// </summary>
        Bengali = 2,
        /// <summary>
        /// Indicates Gujarati.
        /// </summary>
        Gujarati = 5,
        /// <summary>
        /// Indicates Hindi.
        /// </summary>
        Hindi = 6,
        /// <summary>
        /// Indicates Kannada.
        /// </summary>
        Kannada = 7,
        /// <summary>
        /// Indicates Malayalam.
        /// </summary>
        Malayalam = 11,
        /// <summary>
        /// Indicates Manipuri.
        /// </summary>
        Manipuri = 12,
        /// <summary>
        /// Indicates Marathi.
        /// </summary>
        Marathi = 13,
        /// <summary>
        /// Indicates Oriya.
        /// </summary>
        Oriya = 15,
        /// <summary>
        /// Indicates Punjabi.
        /// </summary>
        Punjabi = 16,
        /// <summary>
        /// Indicates Tamil.
        /// </summary>
        Tamil = 20,
        /// <summary>
        /// Indicates Telugu.
        /// </summary>
        Telugu = 21,
        /// <summary>
        /// Indicates Urdu.
        /// </summary>
        Urdu = 22
    }

    /// <summary>
    /// Specifies the matching strategy of name and address.
    /// </summary>
    public enum MatchingStrategy
    {
        /// <summary>
        /// Indicates exact matching strategy.
        /// </summary>
        Exact = 'E',
        /// <summary>
        /// Indicates partial matching strategy specified through a match percentage.
        /// </summary>
        Partial = 'P'
    }

    /// <summary>
    /// Specifies NIST Fingerprint Image Quality.
    /// </summary>
    public enum Nfiq
    {
        /// <summary>
        /// Indicates excellent.
        /// </summary>
        Excellent = 1,
        /// <summary>
        /// Indicates very good.
        /// </summary>
        VeryGood = 2,
        /// <summary>
        /// Indicates good.
        /// </summary>
        Good = 3,
        /// <summary>
        /// Indicates fair.
        /// </summary>
        Fair = 4,
        /// <summary>
        /// Indicates poor.
        /// </summary>
        Poor = 5
    }
}