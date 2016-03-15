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

using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Provides a wrapper to send a best finger detection request and retrieve a response from CIDR servers.
    /// </summary>
    public class BfdClient : ApiClient<BfdRequest, BfdResponse>
    {
        /// <summary>
        /// When overridden in a descendant class, sets the <see cref="ApiClient{TRequest, TResponse}.Address"/> property.
        /// </summary>
        protected override void ApplyAddress()
        {
            ValidateNull(AgencyInfo, nameof(AgencyInfo));
            ValidateNull(Request, nameof(Request));
            ValidateEmptyString(Request.AadhaarNumber, nameof(BfdRequest.AadhaarNumber));

            Address = AgencyInfo.GetAddress(Request.ApiName, Request.AadhaarNumber);
        }
    }
}