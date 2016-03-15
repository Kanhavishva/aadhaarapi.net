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

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Specifies encoding type of resident data.
    /// </summary>
    public enum EncodingType
    {
        /// <summary>
        /// Indicates encrypted data format is in XML.
        /// </summary>
        Xml = 'X',
        /// <summary>
        /// Indicates encrypted data format is in Google Protocol Buffer. Currently, not supported in .NET API version.
        /// </summary>
        ProtocolBuffer = 'P'
    }

    /// <summary>
    /// Specifies location type.
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// Indicates geographic coordinate.
        /// </summary>
        GeoCoordinate = 'G',
        /// <summary>
        /// Indicates pincode.
        /// </summary>
        Pincode = 'P'
    }

    /// <summary>
    /// Specifies method of sending OTP.
    /// </summary>
    public enum OtpChannel
    {
        /// <summary>
        /// Indicates SMS and email both.
        /// </summary>
        SmsAndEmail,
        /// <summary>
        /// Indicates SMS only.
        /// </summary>
        Sms,
        /// <summary>
        /// Indicates email only.
        /// </summary>
        Email
    }

    /// <summary>
    /// Specifies the type of OTP request.
    /// </summary>
    public enum OtpRequestType
    {
        /// <summary>
        /// Specifies Aadhaar number.
        /// </summary>
        AadhaarNumber = 'A',
        /// <summary>
        /// Specifies mobile number.
        /// </summary>
        MobileNumber = 'M'
    }
}