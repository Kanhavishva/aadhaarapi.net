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
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Security;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Provides a wrapper to send a e-KYC request and retrieve a response from CIDR servers.
    /// </summary>
    public class KycClient : ApiClient<KycRequest, KycResponse>
    {
        /// <summary>
        /// Gets or sets an instance of <see cref="IDecrypter"/> to decrypt the response XML.
        /// </summary>
        public IDecrypter Decrypter { get; set; }

        /// <summary>
        /// When overridden in a descendant class, sets the <see cref="ApiClient{TRequest, TResponse}.Address"/> property.
        /// </summary>
        protected override void ApplyAddress()
        {
            ValidateNull(AgencyInfo, nameof(AgencyInfo));
            ValidateNull(Request, nameof(Request));
            ValidateEmptyString(Request.AadhaarNumber, nameof(KycRequest.AadhaarNumber));

            Address = AgencyInfo.GetAddress(Request.ApiName, Request.AadhaarNumber);
        }

        /// <summary>
        /// When overridden in a descendant class, sets agency information to the <see cref="ApiClient{TRequest, TResponse}.Request"/> property.
        /// </summary>
        protected override void ApplyAgencyInfo()
        {
            base.ApplyAgencyInfo();
            if (Request.Info != null)
            {
                using (var sha = SHA256.Create())
                {
                    Request.Info.AadhaarNumberHash = sha.ComputeHash(Request.AadhaarNumber.GetBytes()).ToHex();
                    Request.Info.TerminalHash = sha.ComputeHash(Request.Terminal.GetBytes()).ToHex();
                    Request.Info.AuaCodeHash = sha.ComputeHash(Request.AuaCode.GetBytes()).ToHex();
                }
                Request.Info.SubAuaCode = Request.SubAuaCode;
                Request.Info.Encode();
            }
        }

        /// <summary>
        /// When overridden in a descendant class, asynchronously sends a XML to a specified address and updates the <see cref="ApiClient{TRequest, TResponse}.Response"/> property from a response XML.
        /// </summary>
        /// <returns>A task that represents the asynchronous retrieve operation.</returns>
        protected override async Task RetrieveResponseAsync()
        {
            ValidateNull(Decrypter, nameof(Decrypter));

            await GetResponseAsync(null, element =>
            {
                ValidateNull(element, nameof(element));
                if (element.Attribute("status").Value == "-1")
                    return element;

                var encrypted = Convert.FromBase64String(element.Element("kycRes").Value);
                var decrypted = Decrypter.Decrypt(encrypted);
                using (var stream = new MemoryStream(decrypted))
                    return XElement.Load(stream);
            });
        }
    }
}