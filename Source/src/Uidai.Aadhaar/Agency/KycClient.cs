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
using System.Xml.Linq;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Provides a wrapper to send a e-KYC request and retrieve a response from CIDR servers.
    /// </summary>
    public class KycClient : ApiClient<KycRequest, KycResponse>
    {
        /// <summary>
        /// When overridden in a descendant class, sets the address of the host and addtional properties for request and validation.
        /// </summary>
        protected override void ApplyInfo()
        {
            base.ApplyInfo();
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
            Address = AgencyInfo.GetAddress(Request.ApiName, Request.AadhaarNumber);
        }

        /// <summary>
        /// When overridden in a descendant class, deserializes the response XML.
        /// </summary>
        /// <param name="responseXml">The XML to deserialize.</param>
        /// <exception cref="ApiException">API error occurred.</exception>
        protected override void DeserializeResponseXml(XElement responseXml)
        {
            ValidateNull(responseXml, nameof(responseXml));

            using (var stream = new MemoryStream(Convert.FromBase64String(responseXml.Element("kycRes").Value)))
                base.DeserializeResponseXml(XElement.Load(stream));
        }
    }
}