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

using System.Xml.Linq;
using Uidai.Aadhaar.Helper;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Device
{
    /// <summary>
    /// Represents a combination of one time pin data and related information to be used to OTP request.
    /// </summary>
    public class OtpContext : DeviceContext
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
        /// <value>The name of the API.</value>
        public override string ApiName => "Otp";

        /// <summary>
        /// Gets or sets the mobile number.
        /// <see cref="RequestType"/> must be set to <see cref="OtpRequestType.MobileNumber"/> if mobile number is used. 
        /// </summary>
        /// <value>The mobile number.</value>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the channel through which OTP should be sent.
        /// </summary>
        /// <value>The channel through which OTP should be sent.</value>
        public OtpChannel Channel { get; set; } = OtpChannel.SmsAndEmail;

        /// <summary>
        /// Gets or sets the type of number specified.
        /// Default is <see cref="OtpRequestType.AadhaarNumber"/>.
        /// </summary>
        /// <value>The type of number specified.</value>
        public OtpRequestType RequestType { get; set; } = OtpRequestType.AadhaarNumber;

        /// <summary>
        /// Gets or sets the meta information.
        /// Meta information is only calculated in <see cref="Encode(string)"/> if this property is initialized.
        /// </summary>
        /// <value>The meta information.</value>
        public OtpInfo OtpInfo { get; set; }

        /// <summary>
        /// When overridden in a descendant class, deserializes the object from an XML according to Aadhaar API specification.
        /// </summary>
        /// <param name="element">An instance of <see cref="XElement"/>.</param>
        protected override void DeserializeXml(XElement element)
        {
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
        /// <exception cref="ArgumentNullException"><see cref="Uses"/> is null.</exception>
        protected override XElement SerializeXml(XName name)
        {
            if (RequestType == OtpRequestType.AadhaarNumber)
                ValidateEmptyString(AadhaarNumber, nameof(AadhaarNumber));
            else
                ValidateEmptyString(MobileNumber, nameof(MobileNumber));

            var otpContext = base.SerializeXml(name);
            otpContext.Add(new XAttribute("uid", AadhaarNumber ?? MobileNumber));

            return otpContext;
        }
    }
}