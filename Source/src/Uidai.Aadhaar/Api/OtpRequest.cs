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

using System.Globalization;
using System.Xml.Linq;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Api
{
    /// <summary>
    /// Represents a One Time Pin request.
    /// </summary>
    public class OtpRequest : ApiRequest
    {
        /// <summary>
        /// Represents the OTP version. This field is currently configurable.
        /// </summary>
        /// <value>The OTP version.</value>
        public static string OtpVersion = "1.6";

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpRequest"/> class.
        /// </summary>
        public OtpRequest() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpRequest"/> class with a specified OTP data received from device.
        /// </summary>
        /// <param name="otpContext">The OTP data received from device.</param>
        public OtpRequest(OtpContext otpContext)
        {
            AadhaarOrMobileNumber = otpContext.AadhaarOrMobileNumber;
            Channel = otpContext.Channel;
            RequestType = otpContext.RequestType;
            Terminal = otpContext.Terminal;
        }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Otp";

        /// <summary>
        /// Gets or sets the Aadhaar or mobile number.
        /// </summary>
        /// <value>The Aadhaar or mobile number.</value>
        public string AadhaarOrMobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of number specified.
        /// </summary>
        /// <value>The type of number specified.</value>
        public OtpRequestType RequestType { get; set; }

        /// <summary>
        /// Gets or sets the channel through which OTP should be sent.
        /// </summary>
        /// <value>The channel through which OTP should be sent.</value>
        public OtpChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets any meta information received from device.
        /// This property is excluded from serialization and deserialization.
        /// </summary>
        /// <value>The meta information received from device.</value>
        public OtpInfo Info { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="element"/> is null.</exception>
        protected override void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            base.DeserializeXml(element);
            AadhaarOrMobileNumber = element.Attribute("uid").Value;

            var value = element.Attribute("type")?.Value;
            RequestType = value != null ? (OtpRequestType)value[0] : OtpRequestType.AadhaarNumber;

            value = element.Element("Opts")?.Attribute("ch")?.Value;
            Channel = value != null ? (OtpChannel)int.Parse(value) : OtpChannel.SmsAndEmail;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="System.ArgumentException"><see cref="AadhaarOrMobileNumber"/> is empty. Or, Aadhaar number is invalid.</exception>
        protected override XElement SerializeXml(string elementName)
        {
            ValidateEmptyString(AadhaarOrMobileNumber, nameof(AadhaarOrMobileNumber));
            if (RequestType == OtpRequestType.AadhaarNumber)
                ValidateAadhaarNumber(AadhaarOrMobileNumber, nameof(AadhaarOrMobileNumber));

            var otpRequest = base.SerializeXml(elementName);
            otpRequest.Add(new XAttribute("uid", AadhaarOrMobileNumber),
                new XAttribute("ver", OtpVersion));

            // A workaround to support version 1.5. Version checking can be removed once v1.5 becomes obsolete.
            if (OtpVersion == "1.6" && RequestType != OtpRequestType.AadhaarNumber)
                otpRequest.Add(new XAttribute("type", (char)RequestType));
            if (RequestType != OtpRequestType.MobileNumber && Channel != OtpChannel.SmsAndEmail)
                otpRequest.Add(new XElement("Opts", new XAttribute("ch", ((int)Channel).ToString("D2", CultureInfo.InvariantCulture))));

            Signer?.ComputeSignature(otpRequest);

            return otpRequest;
        }
    }
}