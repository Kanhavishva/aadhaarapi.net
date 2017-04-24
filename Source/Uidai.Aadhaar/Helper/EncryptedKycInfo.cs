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
using System.Linq;
using System.Text;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Represents the encrypted e-KYC response data.
    /// </summary>
    public class EncryptedKycInfo
    {
        /// <summary>
        /// Represents the version of the data. This field is read-only.
        /// </summary>
        public static readonly string Version = "VERSION_1.0";

        private static readonly byte[] HeaderData = Encoding.UTF8.GetBytes(Version);

        /// <summary>
        /// Gets or sets a byte array that contains the encrypted data.
        /// </summary>
        /// <value>A byte array that contains the encrypted data.</value>
        public byte[] InfoValue { get; set; }

        /// <summary>
        /// Gets or sets the header segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The header segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> Header { get; set; }

        /// <summary>
        /// Gets or sets the public key segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The public key segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the OAEP encoding label segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The OAEP encoding label segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> OaepLabel { get; set; }

        /// <summary>
        /// Gets or sets the encrypted key segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The encrypted key segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> EncryptedKey { get; set; }

        /// <summary>
        /// Gets or sets the encrypted data segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The encrypted data segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> EncryptedData { get; set; }

        /// <summary>
        /// Encodes the array segments into the <see cref="InfoValue"/>.
        /// </summary>
        public void Encode()
        {
            InfoValue = new byte[Header.Count + PublicKey.Count + OaepLabel.Count + EncryptedKey.Count + EncryptedData.Count];

            var offset = 0;
            Buffer.BlockCopy(Header.Array, Header.Offset, InfoValue, offset, 11);
            offset += Header.Count;

            Buffer.BlockCopy(PublicKey.Array, PublicKey.Offset, InfoValue, offset, 294);
            offset += PublicKey.Count;

            Buffer.BlockCopy(OaepLabel.Array, OaepLabel.Offset, InfoValue, offset, 32);
            offset += OaepLabel.Count;

            Buffer.BlockCopy(EncryptedKey.Array, EncryptedKey.Offset, InfoValue, offset, 256);
            offset += EncryptedKey.Count;

            Buffer.BlockCopy(EncryptedData.Array, EncryptedData.Offset, InfoValue, offset, EncryptedData.Count);
        }

        /// <summary>
        /// Decodes the <see cref="InfoValue"/> into array segments.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="InfoValue"/> is null.</exception>
        /// <exception cref="ArgumentException"><see cref="Header"/> is invalid.</exception>
        public void Decode()
        {
            ValidateNull(InfoValue, nameof(InfoValue));
            if (HeaderData.Where((t, i) => t != InfoValue[i]).Any())
                throw new ArgumentException(InvalidHeader);

            /*
             * Data             Length
             * ------------------------------
             * Version          11
             * PublicKey        294
             * Label            32
             * EncryptedKey     256
             * EncryptedData    Remaining Bytes
             */

            var offset = 0;

            Header = new ArraySegment<byte>(InfoValue, offset, 11);
            offset += 11;

            PublicKey = new ArraySegment<byte>(InfoValue, offset, 294);
            offset += 294;

            OaepLabel = new ArraySegment<byte>(InfoValue, offset, 32);
            offset += 32;

            EncryptedKey = new ArraySegment<byte>(InfoValue, offset, 256);
            offset += 256;

            EncryptedData = new ArraySegment<byte>(InfoValue, offset, InfoValue.Length - offset);
        }

        /// <summary>
        /// Decrypts the encrypted data using a specified decryptor.
        /// </summary>
        /// <param name="decryptor">An instance of <see cref="IKycDecryptor"/>.</param>
        /// <returns>An instance of <see cref="DecryptedKycInfo"/>.</returns>
        public DecryptedKycInfo Decrypt(IKycDecryptor decryptor)
        {
            ValidateNull(decryptor, nameof(decryptor));

            Decode();

            return decryptor.Decrypt(this);
        }
    }
}