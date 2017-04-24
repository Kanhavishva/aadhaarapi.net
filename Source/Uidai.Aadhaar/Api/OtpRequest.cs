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
        /// Represents the OTP version.
        /// </summary>
        /// <value>The OTP version.</value>
        public static string OtpVersion = "1.6";

        /// <summary>
        /// Represents the Otp XML namespace. This field is read-only.
        /// </summary>
        public static readonly XNamespace OtpXmlNamespace = "http://www.uidai.gov.in/authentication/otp/1.0";

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
            AadhaarNumber = otpContext.AadhaarNumber;
            MobileNumber = otpContext.MobileNumber;
            Channel = otpContext.Channel;
            RequestType = otpContext.RequestType;
            Terminal = otpContext.Terminal;
            OtpInfo = otpContext.OtpInfo;
        }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        /// <value>The name of the API.</value>
        public override string ApiName => "Otp";

        /// <summary>
        /// Gets or sets the Aadhaar number.
        /// </summary>
        /// <value>The Aadhaar number.</value>
        public string AadhaarNumber { get; set; }

        /// <summary>
        /// Gets or sets the mobile number.
        /// </summary>
        /// <value>The mobile number.</value>
        public string MobileNumber { get; set; }

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
        /// Gets or sets the time of capturing the resident data.
        /// Default is <see cref="DateTimeOffset.Now"/>
        /// </summary>
        /// <value>The time of capturing the resident data.</value>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets any meta information received from device.
        /// This property is excluded from serialization and deserialization.
        /// </summary>
        /// <value>The meta information received from device.</value>
        public OtpInfo OtpInfo { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/> is null.</exception>
        protected override void DeserializeXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            base.DeserializeXml(element);

            var value = element.Attribute("type")?.Value;
            RequestType = value != null ? (OtpRequestType)value[0] : OtpRequestType.AadhaarNumber;

            if (RequestType == OtpRequestType.AadhaarNumber)
                AadhaarNumber = element.Attribute("uid").Value;
            else
                MobileNumber = element.Attribute("uid").Value;

            value = element.Element("Opts")?.Attribute("ch")?.Value;
            Channel = value != null ? (OtpChannel)int.Parse(value) : OtpChannel.SmsAndEmail;
        }

        /// <summary>
        /// When overridden in a descendant class, serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="AadhaarNumber"/> or <see cref="MobileNumber"/> is empty. Or, Aadhaar number is invalid.</exception>
        protected override XElement SerializeXml(XName name)
        {
            if (RequestType == OtpRequestType.AadhaarNumber)
                ValidateAadhaarNumber(AadhaarNumber, nameof(AadhaarNumber));
            else
                ValidateEmptyString(MobileNumber, nameof(MobileNumber));

            var otpRequest = base.SerializeXml(OtpXmlNamespace + name.LocalName);
            otpRequest.Add(new XAttribute("uid", AadhaarNumber ?? MobileNumber),
                new XAttribute("ver", OtpVersion),
                new XAttribute("ts", Timestamp.ToString(AadhaarHelper.TimestampFormat, CultureInfo.InvariantCulture)));

            if (RequestType != OtpRequestType.AadhaarNumber)
                otpRequest.Add(new XAttribute("type", (char)RequestType));
            if (RequestType != OtpRequestType.MobileNumber && Channel != OtpChannel.SmsAndEmail)
                otpRequest.Add(new XElement("Opts", new XAttribute("ch", ((int)Channel).ToString("D2", CultureInfo.InvariantCulture))));

            Signer?.ComputeSignature(otpRequest);

            return otpRequest;
        }
    }
}