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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Helper
{
    /// <summary>
    /// Represents the decrypted e-KYC response data.
    /// </summary>
    public class DecryptedKycInfo
    {
        /// <summary>
        /// Gets or sets a byte array that contains the decrypted data.
        /// </summary>
        /// <value>A byte array that contains the decrypted data.</value>
        public byte[] InfoValue { get; set; }

        /// <summary>
        /// Gets or sets the data segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The data segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> Data { get; set; }

        /// <summary>
        /// Gets or sets the Hmac segment from the <see cref="InfoValue"/> array.
        /// </summary>
        /// <value>The Hmac segment from the <see cref="InfoValue"/> array.</value>
        public ArraySegment<byte> Hmac { get; set; }

        /// <summary>
        /// Encodes the array segments into the <see cref="InfoValue"/>.
        /// </summary>
        public void Encode()
        {
            InfoValue = new byte[Hmac.Count + Data.Count];

            var offset = 0;

            Buffer.BlockCopy(Hmac.Array, Hmac.Offset, InfoValue, offset, Hmac.Count);
            offset += Hmac.Count;

            Buffer.BlockCopy(Data.Array, Data.Offset, InfoValue, offset, Data.Count);
        }

        /// <summary>
        /// Decodes the <see cref="InfoValue"/> into array segments.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="InfoValue"/> is null.</exception>
        /// <exception cref="CryptographicException"><see cref="Hmac"/> is wrong.</exception>
        public void Decode()
        {
            ValidateNull(InfoValue, nameof(InfoValue));

            /*
             * Data             Length
             * ------------------------------
             * Hmac             32
             * Data             Remaining Bytes
             */

            var offset = 0;

            Hmac = new ArraySegment<byte>(InfoValue, offset, 32);
            offset += 32;

            Data = new ArraySegment<byte>(InfoValue, offset, InfoValue.Length - offset);

            // Validate Hmac
            using (var sha = SHA256.Create())
            {
                var newHmac = sha.ComputeHash(Data.Array, Data.Offset, Data.Count);
                var oldHmac = new byte[Hmac.Count];
                Buffer.BlockCopy(Hmac.Array, Hmac.Offset, oldHmac, 0, Hmac.Count);
                if (!oldHmac.SequenceEqual(newHmac))
                    throw new CryptographicException(InvalidHmac);
            }
        }

        /// <summary>
        /// Serializes the data segment into XML.
        /// </summary>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml()
        {
            Decode();
            using (var stream = new MemoryStream(Data.Array, Data.Offset, Data.Count))
                return XElement.Load(stream);
        }
    }
}