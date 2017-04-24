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

// .NET Standard Library doesn't support specifying OAEP label for RSA-OAEP decryption and AES decryption with no padding.
// Therefore, we have to use BouncyCastle to perform those tasks.
#if NET46
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography.X509Certificates;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides an implementation to decrypt e-KYC data.
    /// </summary>
    public class KycDecryptor : IKycDecryptor
    {
        private static readonly string SymmetricAlgorithm = "AES/CFB/NoPadding";

        /// <summary>
        /// Gets or sets the X.509 certificate that contains the private key of the user agency.
        /// The <see cref="X509Certificate2"/> must be created with <see cref="X509KeyStorageFlags.Exportable"/> flag.
        /// </summary>
        /// <value>The X.509 certificate that contains the private key of the user agency.</value>
        public X509Certificate2 KuaKey { get; set; }

        /// <summary>
        /// Decrypts the specified encrypted e-KYC response data received from UIDAI.
        /// </summary>
        /// <param name="kycInfo">The encrypted e-KYC data.</param>
        /// <returns>The decrypted e-KYC data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="kycInfo"/> or <see cref="EncryptedKycInfo.InfoValue"/> is null.</exception>
        public DecryptedKycInfo Decrypt(EncryptedKycInfo kycInfo)
        {
            ValidateNull(kycInfo, nameof(kycInfo));
            ValidateNull(kycInfo.InfoValue, nameof(EncryptedKycInfo.InfoValue));

            var iv = new byte[kycInfo.OaepLabel.Count];
            Buffer.BlockCopy(kycInfo.OaepLabel.Array, kycInfo.OaepLabel.Offset, iv, 0, iv.Length);

            // Decrypt key.
            var oaep = new OaepEncoding(new RsaEngine(), new Sha256Digest(), iv);
            oaep.Init(false, DotNetUtilities.GetRsaKeyPair(KuaKey.GetRSAPrivateKey()).Private);
            var key = oaep.ProcessBlock(kycInfo.EncryptedKey.Array, kycInfo.EncryptedKey.Offset, kycInfo.EncryptedKey.Count);

            // Decrypt data.
            var cipher = CipherUtilities.GetCipher(SymmetricAlgorithm);
            var parameter = new ParametersWithIV(new KeyParameter(key), iv, 0, 16);
            cipher.Init(false, parameter);
            var data = cipher.DoFinal(kycInfo.EncryptedData.Array, kycInfo.EncryptedData.Offset, kycInfo.EncryptedData.Count);

            return new DecryptedKycInfo { InfoValue = data };
        }
    }
}
#endif