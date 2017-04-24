#region Copyright
/********************************************************************************
 * Aadhaar API for .NET Copyright © 2015 Souvik Dey Chowdhury
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
 * along with Aadhaar API for .NET. If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************************/
#endregion

namespace Uidai.Aadhaar.Internal
{
    internal static class ErrorMessage
    {
        // Invalid
        public static readonly string InvalidAadhaarNumber = "Aadhaar number is invalid.";
        public static readonly string InvalidBiometricPosition = "Biometric position is invalid.";
        public static readonly string InvalidHeader = "Header data is invalid.";
        public static readonly string InvalidHmac = "Data hash is wrong.";
        public static readonly string InvalidPincode = "Pincode must be 6 digits.";
        public static readonly string InvalidSignature = "Digital signature verification failed.";
        public static readonly string InvalidTransactionPrefix = "Transaction prefix is invalid.";

        // Out of range
        public static readonly string OutOfRangeAge = "Age must be within 0 - 150.";
        public static readonly string OutOfRangeAuaCode = "AUA or Sub-AUA code must be within 10 characters.";
        public static readonly string OutOfRangeAuaLicenseKey = "AUA license key must be within 64 characters.";
        public static readonly string OutOfRangeDeviceCode = "Device code length exceeded.";
        public static readonly string OutOfRangeLatitude = "Latitude must be within -90 - +90.";
        public static readonly string OutOfRangeLongitude = "Longitude must be within -180 - +180.";
        public static readonly string OutOfRangeAltitude = "Altitude must be within -999 to +999";
        public static readonly string OutOfRangeMatchPercent = "Match percentage must be within 1 - 100.";
        public static readonly string OutOfRangeName = "Name must be within 60 characters.";
        public static readonly string OutOfRangeNumberOfAttempts = "Minimum attempts must be greater than 1.";
        public static readonly string OutOfRangeTransaction = "Transaction identifier must be within 50 characters.";

        // Required
        public static readonly string RequiredBiometricOrOtp = "Biometric or OTP authentication required.";
        public static readonly string RequiredBiometricsUsed = "Biometrics used need to be specified.";
        public static readonly string RequiredConsent = "Resident consent required.";
        public static readonly string RequiredIndianLanguage = "Indian language type used required.";
        public static readonly string RequiredNonEmptyString = "String cannot be null, empty or whitespaces.";
        public static readonly string RequiredSomeData = "At least one resident data needs to be specified.";

        // Not Found
        public static readonly string NoHostName = "Host name not found.";
        public static readonly string NoPrivateKey = "No private key found.";
        public static readonly string NoPublicKey = "No public key found.";
        public static readonly string NoSignature = "No signature found.";

        // XOR
        public static readonly string XorAddresses = "Address & FullAddress cannot be used in same transaction.";
        public static readonly string XorFirFmr = "Fingerprint and Minutiae cannot be used in same transaction.";

        // Miscellany
        public static readonly string NotSupportedXmlSignature = "XML signing not supported in platform.";
        public static readonly string ExpiredSynchronizedKey = "Synchronized key expired.";
    }
}