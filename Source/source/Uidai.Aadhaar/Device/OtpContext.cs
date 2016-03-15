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
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of one time pin data and related information to be used to OTP request.
    /// </summary>
    public class OtpContext : IXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtpContext"/> class.
        /// </summary>
        public OtpContext() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtpContext"/> class from an XML.
        /// </summary>
        /// <param name="element">The XML to deserialize.</param>
        public OtpContext(XElement element) { FromXml(element); }

        /// <summary>
        /// Gets the name of the API. The name is usually the XML root name sent in request.
        /// </summary>
        public string ApiName => "Otp";

        /// <summary>
        /// Gets or sets the Aadhaar or mobile number.
        /// <see cref="RequestType"/> must be set to <see cref="OtpRequestType.MobileNumber"/> if mobile number is used. 
        /// </summary>
        public string AadhaarOrMobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the channel through which OTP should be sent.
        /// </summary>
        public OtpChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets the type of number specified.
        /// Default is <see cref="OtpRequestType.AadhaarNumber"/>.
        /// </summary>
        public OtpRequestType RequestType { get; set; } = OtpRequestType.AadhaarNumber;

        /// <summary>
        /// Gets or sets the registered terminal ID, when using registered device.
        /// Default is <see cref="AadhaarHelper.PublicTerminal"/>.
        /// </summary>
        public string Terminal { get; set; } = AadhaarHelper.PublicTerminal;

        /// <summary>
        /// Deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        public void FromXml(XElement element)
        {
            ValidateNull(element, nameof(element));

            Terminal = element.Attribute("tid").Value;
            AadhaarOrMobileNumber = element.Attribute("uid").Value;

            var value = element.Attribute("type")?.Value;
            RequestType = value != null ? (OtpRequestType)value[0] : OtpRequestType.AadhaarNumber;

            value = element.Element("Opts")?.Attribute("ch")?.Value;
            Channel = value != null ? (OtpChannel)int.Parse(value) : OtpChannel.SmsAndEmail;
        }

        /// <summary>
        /// Serializes the object into XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>An instance of <see cref="XElement"/>.</returns>
        public XElement ToXml(string elementName)
        {
            ValidateEmptyString(Terminal, nameof(Terminal));
            ValidateEmptyString(AadhaarOrMobileNumber, nameof(AadhaarOrMobileNumber));
            if (RequestType == OtpRequestType.AadhaarNumber)
                ValidateAadhaarNumber(AadhaarOrMobileNumber, nameof(AadhaarOrMobileNumber));

            var otpContext = new XElement(elementName,
                new XAttribute("tid", Terminal),
                new XAttribute("uid", AadhaarOrMobileNumber));
            if (RequestType != OtpRequestType.AadhaarNumber)
                otpContext.Add(new XAttribute("type", (char)RequestType));
            if (RequestType != OtpRequestType.MobileNumber && Channel != OtpChannel.SmsAndEmail)
                otpContext.Add(new XElement("Opts", new XAttribute("ch", ((int)Channel).ToString("D2", CultureInfo.InvariantCulture))));

            return otpContext;
        }
    }
}