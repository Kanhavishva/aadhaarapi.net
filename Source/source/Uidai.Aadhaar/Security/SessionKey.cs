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
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    ///  Provides a wrapper to encrypt data using 256-bit AES algorithm.
    /// </summary>
    public class SessionKey : IDisposable
    {
        /// <summary>
        /// Represents the time span a synchronized key is valid. This field is configurable.
        /// </summary>
        public static TimeSpan SynchronizedKeyTimeout = new TimeSpan(3, 45, 0);

        private readonly bool leaveOpen;
        private readonly Aes aes;
        private readonly DateTimeOffset? seedCreationTime;
        private readonly RandomNumberGenerator random;
        private readonly byte[] seedKey;

        private byte[] syncKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionKey"/> class with a specified file name of UIDAI public key.
        /// </summary>
        /// <param name="fileName">The file name of the X.509 certificate.</param>
        /// <param name="isSynchronized">true to use synchronized session key; otherwise false.</param>
        public SessionKey(string fileName, bool isSynchronized) : this(new X509Certificate2(fileName), isSynchronized)
        {
            leaveOpen = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionKey"/> class with a specified UIDAI public key.
        /// Keeps the certificate open after the <see cref="SessionKey"/> object is disposed.
        /// </summary>
        /// <param name="uidaiKey">The X.509 certificate.</param>
        /// <param name="isSynchronized">true to use synchronized session key; otherwise false.</param>
        public SessionKey(X509Certificate2 uidaiKey, bool isSynchronized)
        {
            ValidateNull(uidaiKey, nameof(uidaiKey));

            UidaiKey = uidaiKey;
            leaveOpen = true;
            aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            if (isSynchronized)
            {
                IsSynchronized = true;
                random = RandomNumberGenerator.Create();
                seedKey = new byte[32];
                aes.Key.CopyTo(seedKey, 0);
                KeyIdentifier = Guid.NewGuid();
                seedCreationTime = DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Gets the encrypted key info used to encrypt data.
        /// A new key is generated after <see cref="KeyInfo"/> is accessed.
        /// </summary>
        public SessionKeyInfo KeyInfo => EncryptKey();

        /// <summary>
        /// Gets the UIDAI public key.
        /// </summary>
        public X509Certificate2 UidaiKey { get; }

        /// <summary>
        /// Gets a value that indicates whether the session key is synchronized.
        /// </summary>
        public bool IsSynchronized { get; }

        /// <summary>
        /// Gets the key identifier of the synchronized session key.
        /// </summary>
        public Guid KeyIdentifier { get; }

        /// <summary>
        /// Gets a value that indicates whether a synchronized session key has expired. 
        /// </summary>
        public bool HasExpired => IsSynchronized && seedCreationTime - DateTimeOffset.Now > SynchronizedKeyTimeout;

        /// <summary>
        /// Encrypts an input byte array and returns the encrypted array.
        /// </summary>
        /// <param name="dataToEncrypt">The data to encrypt.</param>
        /// <returns>A byte array that contains the decrypted data.</returns>
        public byte[] Encrypt(byte[] dataToEncrypt)
        {
            ValidateNull(dataToEncrypt, nameof(dataToEncrypt));
            if (HasExpired)
                throw new InvalidOperationException(ExpiredSynchronizedKey);

            using (var transform = aes.CreateEncryptor())
                return transform.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
        }

        private void GenerateKey()
        {
            if (IsSynchronized)
            {
                if (syncKey == null)
                    syncKey = new byte[20];
                random.GetBytes(syncKey);
                using (var transform = aes.CreateEncryptor(seedKey, null))
                {
                    var encryptionKey = transform.TransformFinalBlock(syncKey, 0, syncKey.Length);
                    Array.Resize(ref encryptionKey, 32);
                    aes.Key = encryptionKey;
                }
            }
            else
                aes.GenerateKey();
        }

        private SessionKeyInfo EncryptKey()
        {
            var rsa = UidaiKey.GetRSAPublicKey();
            if (rsa == null)
                throw new ArgumentNullException(nameof(UidaiKey), NoPublicKey);

            // For normal session key, syncKey is always null.
            var key = Convert.ToBase64String(syncKey ?? rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1));
            GenerateKey();

            return new SessionKeyInfo
            {
                CertificateIdentifier = UidaiKey.NotAfter,
                Key = key,
                KeyIdentifier = KeyIdentifier
            };
        }

        #region IDisposable Support

        private bool disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if the method call comes from a <see cref="Dispose(bool)"/>; otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    aes.Dispose();
                    random?.Dispose();
                    if (!leaveOpen)
                        UidaiKey.Dispose();
                    if (seedKey != null)
                        Array.Clear(seedKey, 0, seedKey.Length);
                    if (syncKey != null)
                        Array.Clear(syncKey, 0, syncKey.Length);
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Dispose(true);

        #endregion
    }
}