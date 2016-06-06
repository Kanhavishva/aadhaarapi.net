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

using System.Security.Cryptography;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Provides a wrapper to send a OTP request and retrieve a response from CIDR servers.
    /// </summary>
    public class OtpClient : ApiClient<OtpRequest, OtpResponse>
    {
        /// <summary>
        /// When overridden in a descendant class, sets the <see cref="ApiClient{TRequest, TResponse}.Address"/> property.
        /// </summary>
        protected override void ApplyAddress()
        {
            ValidateNull(AgencyInfo, nameof(AgencyInfo));
            ValidateNull(Request, nameof(Request));
            ValidateEmptyString(Request.AadhaarOrMobileNumber, nameof(OtpRequest.AadhaarOrMobileNumber));

            // Don't use Mobile number as part of the URL.
            Address = AgencyInfo.GetAddress(Request.ApiName, Request.RequestType == OtpRequestType.AadhaarNumber ? Request.AadhaarOrMobileNumber : null);
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
                    Request.Info.AadhaarNumberHash = sha.ComputeHash(Request.AadhaarOrMobileNumber.GetBytes()).ToHex();
                    Request.Info.AuaCodeHash = sha.ComputeHash(Request.AuaCode.GetBytes()).ToHex();
                }
                Request.Info.SubAuaCode = Request.SubAuaCode;
                Request.Info.Encode();
            }
        }
    }
}