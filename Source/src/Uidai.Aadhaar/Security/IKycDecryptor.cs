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

using Uidai.Aadhaar.Helper;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides an interface to decrypt e-KYC response data.
    /// </summary>
    public interface IKycDecryptor
    {
        /// <summary>
        /// Decrypts the specified encrypted e-KYC response data received from UIDAI.
        /// </summary>
        /// <param name="kycInfo">The encrypted e-KYC data.</param>
        /// <returns>The decrypted e-KYC data.</returns>
        DecryptedKycInfo Decrypt(EncryptedKycInfo kycInfo);
    }
}