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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides a wrapper to decrypt e-KYC response.
    /// </summary>
    /// <seealso cref="IDecrypter"/>
    public class KycDecrypter : IDecrypter
    {
        private static readonly byte[] HeaderData = Encoding.UTF8.GetBytes("VERSION_1.0");

        /// <summary>
        /// Gets or sets an X.509 certificate to decrypt the XML.
        /// </summary>
        public X509Certificate2 AuaKey { get; set; }

        /// <summary>
        /// Decrypts the specified encrypted byte array.
        /// </summary>
        /// <param name="dataToDecrypt">The data to decrypt.</param>
        /// <returns>A byte array that contains the decrypted data.</returns>
        public byte[] Decrypt(byte[] dataToDecrypt)
        {
            var offset = 0;

            var header = new byte[HeaderData.Length];
            Array.Copy(dataToDecrypt, offset, header, 0, header.Length);
            offset += header.Length;

            var publicKey = new byte[294];
            Array.Copy(dataToDecrypt, offset, publicKey, 0, publicKey.Length);
            offset += publicKey.Length;

            var iv = new byte[32];
            Array.Copy(dataToDecrypt, offset, iv, 0, iv.Length);
            offset += iv.Length;

            var encryptedSecretKey = new byte[256];
            Array.Copy(dataToDecrypt, offset, encryptedSecretKey, 0, encryptedSecretKey.Length);
            offset += encryptedSecretKey.Length;

            var encryptedData = new byte[dataToDecrypt.Length - offset];
            Array.Copy(dataToDecrypt, offset, encryptedData, 0, encryptedData.Length);

            var key = AuaKey.GetRSAPrivateKey().Decrypt(encryptedSecretKey, RSAEncryptionPadding.OaepSHA256);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously decrypts the specified encrypted byte array.
        /// </summary>
        /// <param name="dataToDecrypt">The data to decrypt.</param>
        /// <returns>A task that represents the asynchronous decrypt operation.</returns>
        public async Task<byte[]> DecryptAsync(byte[] dataToDecrypt) => await Task.Run(() => Decrypt(dataToDecrypt));
    }
}