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
    /// <typeparam name="TRequest">The type of the request to send.</typeparam>
    /// <typeparam name="TResponse">The type of the response to receive.</typeparam>
    public class ApiClient<TRequest, TResponse> where TRequest : ApiRequest where TResponse : ApiResponse
    {
        private static readonly string ContentType = "application/xml";

        /// <summary>
        /// Gets or sets the address of the API service.
        /// </summary>
        /// <value>The address of the API service..</value>
        public Uri Address { get; set; }

        /// <summary>
        /// Gets or sets the agency information.
        /// </summary>
        /// <value>The agency information.</value>
        public UserAgency AgencyInfo { get; set; }

        /// <summary>
        /// Gets or sets the API request.
        /// </summary>
        /// <value>The API request.</value>
        public TRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the API response.
        /// </summary>
        /// <value>The API response.</value>
        public TResponse Response { get; set; }

        /// <summary>
        /// Asynchronously sends an XML to a specified address and updates the <see cref="Response"/> property from a response XML.
        /// </summary>
        /// <returns>A task that represents the asynchronous response operation.</returns>
        public async Task GetResponseAsync() => await GetResponseAsync(null, null);

        /// <summary>
        /// Asynchronously sends a transformed XML to a specified address and updates the <see cref="Response"/> property from a transformed response XML.
        /// </summary>
        /// <param name="requestXmlTransformer">A work to transform the request XML.</param>
        /// <param name="responseXmlTransformer">A work to transform the response XML.</param>
        /// <returns>A task that represents the asynchronous response operation.</returns>
        public async Task GetResponseAsync(Func<XElement, XElement> requestXmlTransformer, Func<XElement, XElement> responseXmlTransformer)
        {
            ApplyInfo();

            var requestXml = SerializeRequestXml();
            if (requestXmlTransformer != null)
                requestXml = requestXmlTransformer(requestXml);

            var responseXml = await GetResponseXmlAsync(requestXml);
            if (responseXmlTransformer != null)
                responseXml = responseXmlTransformer(responseXml);

            DeserializeResponseXml(responseXml);
        }

        /// <summary>
        /// When overridden in a descendant class, sets the address of the host and addtional properties for request and validation.
        /// </summary>
        protected virtual void ApplyInfo()
        {
            ValidateNull(Request, nameof(Request));
            ValidateNull(Response, nameof(Response));
            ValidateNull(AgencyInfo, nameof(AgencyInfo));

            Request.AuaCode = AgencyInfo.AuaCode;
            Request.AuaLicenseKey = AgencyInfo.AuaLicenseKey;
            Request.SubAuaCode = AgencyInfo.SubAuaCode;

            Address = AgencyInfo.GetAddress(Request.ApiName);
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the request XML.
        /// </summary>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        protected virtual XElement SerializeRequestXml()
        {
            return Request.ToXml();
        }

        /// <summary>
        /// When overridden in a descendant class, asynchronously sends a XML to a specified address and gets a response XML.
        /// </summary>
        /// <param name="requestXml">The XML to send.</param>
        /// <returns>A task that represents the asynchronous response operation.</returns>
        protected virtual async Task<XElement> GetResponseXmlAsync(XElement requestXml)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                using (var content = new StringContent(requestXml.ToString(SaveOptions.DisableFormatting)))
                using (var response = (await client.PostAsync(Address, content)).EnsureSuccessStatusCode())
                    return XElement.Load(await response.Content.ReadAsStreamAsync());
            }
        }

        /// <summary>
        /// When overridden in a descendant class, deserializes the response XML.
        /// </summary>
        /// <param name="responseXml">The XML to deserialize.</param>
        /// <exception cref="ApiException">API error occurred.</exception>
        protected virtual void DeserializeResponseXml(XElement responseXml)
        {
            ValidateNull(Response, nameof(responseXml));

            Response.ErrorCode = responseXml.Attribute("err")?.Value;

            // Catch all exceptions arising from API error condition due to absence of mandatory XML elements and attributes.
            try { Response.FromXml(responseXml); }
            finally
            {
                if (!string.IsNullOrWhiteSpace(Response.ErrorCode))
                    throw new ApiException(Response.ErrorCode);
            }
        }
    }
}