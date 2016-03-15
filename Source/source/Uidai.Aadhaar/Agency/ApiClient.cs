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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Agency
{
    /// <summary>
    /// Provides a wrapper to send a request and retrieve a response from CIDR servers.
    /// </summary>
    public class ApiClient<TRequest, TResponse> where TRequest : ApiRequest where TResponse : ApiResponse
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// Gets or sets the agency information.
        /// </summary>
        public UserAgency AgencyInfo { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        public TRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public TResponse Response { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to throw <see cref="ApiException"/> if <see cref="ApiResponse.ErrorCode"/> has value.
        /// </summary>
        public bool ThrowOnApiException { get; set; }

        /// <summary>
        /// Asynchronously sends a XML to a specified address and updates the <see cref="Response"/> property from a response XML.
        /// </summary>
        /// <returns>A task that represents the asynchronous retrieve operation.</returns>
        public async Task GetResponseAsync() => await RetrieveResponseAsync();

        /// <summary>
        /// Asynchronously sends a transformed XML to a specified address and updates the <see cref="Response"/> property from a transformed response XML.
        /// </summary>
        /// <param name="transformRequestXml">A work to transform the request XML, before sending it to CIDR servers.</param>
        /// <param name="transformResponseXml">A work to transform the response XML returned from CIDR servers, before loading in the <see cref="Response"/> property.</param>
        /// <returns>A task that represents the asynchronous retrieve operation.</returns>
        public async Task GetResponseAsync(Func<XElement, XElement> transformRequestXml, Func<XElement, XElement> transformResponseXml)
        {
            ValidateNull(Response, nameof(Response));

            ApplyAgencyInfo();

            var requestXml = Request.ToXml();
            if (transformRequestXml != null)
                requestXml = transformRequestXml(requestXml);

            var responseXml = await RetrieveResponseXmlAsync(requestXml);
            if (transformResponseXml != null)
                responseXml = transformResponseXml(responseXml);

            Response.FromXml(responseXml);
            if (ThrowOnApiException && !string.IsNullOrWhiteSpace(Response.ErrorCode))
                throw new ApiException(Response.ErrorCode);
        }

        /// <summary>
        /// When overridden in a descendant class, sets the <see cref="Address"/> property.
        /// </summary>
        protected virtual void ApplyAddress()
        {
            Address = AgencyInfo.GetAddress(Request.ApiName);
        }

        /// <summary>
        /// When overridden in a descendant class, sets agency information to the <see cref="Request"/> property.
        /// </summary>
        protected virtual void ApplyAgencyInfo()
        {
            ValidateNull(AgencyInfo, nameof(AgencyInfo));
            ValidateNull(Request, nameof(Request));

            Request.AuaCode = AgencyInfo.AuaCode;
            Request.AuaLicenseKey = AgencyInfo.AuaLicenseKey;
            Request.SubAuaCode = AgencyInfo.SubAuaCode;
        }

        /// <summary>
        /// When overridden in a descendant class, asynchronously sends a XML to a specified address and updates the <see cref="Response"/> property from a response XML.
        /// </summary>
        /// <returns>A task that represents the asynchronous retrieve operation.</returns>
        protected virtual async Task RetrieveResponseAsync()
        {
            await GetResponseAsync(null, null);
        }

        /// <summary>
        /// When overridden in a descendant class, asynchronously sends a XML to a specified address and gets a response XML.
        /// </summary>
        /// <param name="xml">The XML to send.</param>
        /// <returns>A task that represents the asynchronous retrieve operation.</returns>
        protected virtual async Task<XElement> RetrieveResponseXmlAsync(XElement xml)
        {
            ValidateNull(xml, nameof(xml));

            if (Address == null)
                ApplyAddress();
            ValidateNull(Address, nameof(Address));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                using (var content = new StringContent(xml.ToString(SaveOptions.DisableFormatting)))
                using (var response = (await client.PostAsync(Address, content)).EnsureSuccessStatusCode())
                    return XElement.Load(await response.Content.ReadAsStreamAsync());
            }
        }
    }
}