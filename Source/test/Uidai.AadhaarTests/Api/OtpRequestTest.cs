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
using System.Linq;
using System.Xml.Linq;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Xunit;

namespace Uidai.AadhaarTests.Api
{
    public class OtpRequestTest
    {
        [Fact]
        public void FromXmlTest()
        {
            /*
            Assume:     ToXml(string) is correct.
            */
            var otpRequest = new OtpRequest();
            var xml = XElement.Parse(File.ReadAllText(Data.OtpRequestXml)).Elements().ToArray();

            Assert.Throws<ArgumentNullException>("element", () => otpRequest.FromXml(null));

            foreach (var element in xml)
            {
                otpRequest.FromXml(element);
                Assert.True(XNode.DeepEquals(element, otpRequest.ToXml("Otp")));
            }
        }

        [Fact]
        public void ToXmlTest()
        {
            var otpRequest = Data.OtpRequest;
            var xml = XElement.Parse(File.ReadAllText(Data.OtpRequestXml)).Elements().ToArray();

            // Set: All
            Assert.True(XNode.DeepEquals(xml[0], otpRequest.ToXml("Otp")));

            // Set: RequestType = AadhaarNumber
            otpRequest.RequestType =  OtpRequestType.AadhaarNumber;
            Assert.Throws<ArgumentException>(nameof(OtpRequest.AadhaarOrMobileNumber), () => otpRequest.ToXml("Otp"));

            // Set: AadhaarOrMobileNumber = 999999999999
            otpRequest.AadhaarOrMobileNumber = "999999999999";
            Assert.True(XNode.DeepEquals(xml[1], otpRequest.ToXml("Otp")));

            // Set: Channel = SmsAndEmail
            otpRequest.Channel =  OtpChannel.SmsAndEmail;
            Assert.True(XNode.DeepEquals(xml[2], otpRequest.ToXml("Otp")));

            // Remove: AadhaarOrMobileNumber
            otpRequest.AadhaarOrMobileNumber = null;
            Assert.Throws<ArgumentException>(nameof(OtpContext.AadhaarOrMobileNumber), () => otpRequest.ToXml("Otp"));
        }
    }
}