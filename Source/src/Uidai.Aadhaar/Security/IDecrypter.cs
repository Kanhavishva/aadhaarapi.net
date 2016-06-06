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

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides an interface to decrypt data.
    /// </summary>
    public interface IDecrypter
    {
        /// <summary>
        /// Decrypts the specified encrypted byte array.
        /// </summary>
        /// <param name="dataToDecrypt">The data to decrypt.</param>
        /// <returns>A byte array that contains the decrypted data.</returns>
        byte[] Decrypt(byte[] dataToDecrypt);
    }
}