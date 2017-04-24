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
using System.Threading.Tasks;
using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of finger data and related information to be used for best finger detection. 
    /// </summary>
    public class BfdContext : DeviceContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BfdContext"/> class.
        /// </summary>
        public BfdContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BfdContext"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public BfdContext(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the name of the API. The name is usually the name of the XML root sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Bfd";

        /// <summary>
        /// Encrypts finger data captured for best finger detection.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="key"/> is null.</exception>
        public void Encrypt(BestFingerInfo data, SessionKey key)
        {
            ValidateNull(data, nameof(data));
            ValidateNull(key, nameof(key));

            // Create Rbd bytes.
            var rbdBytes = data.ToXml("Rbd").ToString(SaveOptions.DisableFormatting).GetBytes();

            using (var sha = SHA256.Create())
            {
                var rbdHash = sha.ComputeHash(rbdBytes);

                // Encrypt data.
                var encryptedRbd = key.Encrypt(rbdBytes);
                var encryptedHash = key.Encrypt(rbdHash);
                KeyInfo = key.KeyInfo;

                // Set related properties.
                AadhaarNumber = data.AadhaarNumber;
                Timestamp = data.Timestamp;
                Data = new EncryptedData { Data = Convert.ToBase64String(encryptedRbd) };
                Hmac = Convert.ToBase64String(encryptedHash);

                if (DeviceInfo != null)
                    DeviceInfo.IrisDeviceCode = DeviceInfo.DeviceNotApplicable;
            }
            Array.Clear(rbdBytes, 0, rbdBytes.Length);
        }

        /// <summary>
        /// Asynchronously encrypts data captured for request.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The key to encrypt data.</param>
        /// <returns>A task that represents the asynchronous encrypt operation.</returns>
        public async Task EncryptAsync(BestFingerInfo data, SessionKey key) => await Task.Run(() => Encrypt(data, key));
    }
}