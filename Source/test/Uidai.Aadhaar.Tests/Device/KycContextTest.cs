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
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Resident;
using Xunit;

namespace Uidai.AadhaarTests.Device
{
    public class KycContextTest
    {
        [Fact]
        public void EncryptTest()
        {
            var kycContext = new KycContext();
            var personalInfo = Data.PersonalInfo;
            var sessionKey = Data.SessionKey;

            // Test 1: HasResidentConsent = true.
            Assert.Throws<ArgumentException>(nameof(KycContext.HasResidentConsent), () => kycContext.Encrypt(personalInfo, sessionKey));

            // Test 2: Biometric or OTP is mandatory
            kycContext.HasResidentConsent = true;
            personalInfo.Biometrics.Clear();
            kycContext.Encrypt(personalInfo, sessionKey);

            personalInfo.PinValue.Otp = null;
            Assert.Throws<ArgumentException>(nameof(PersonalInfo.Uses), () => kycContext.Encrypt(personalInfo, sessionKey));
        }
    }
}