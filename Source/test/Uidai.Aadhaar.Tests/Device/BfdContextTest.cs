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
using Xunit;

namespace Uidai.AadhaarTests.Device
{
    public class BfdContextTest
    {
        [Fact]
        public void EncryptTest()
        {
            var bfdContext = new BfdContext
            {
                DeviceInfo = Data.Metadata,
                Terminal = Data.PublicTerminal
            };

            // Test 1: Validate null argument.
            Assert.Throws<ArgumentNullException>("key", () => bfdContext.Encrypt(Data.BestFingerInfo, null));
            Assert.Throws<ArgumentNullException>("data", () => bfdContext.Encrypt(null, Data.SessionKey));

            // Test 2: All fields are set after call.
            bfdContext.Encrypt(Data.BestFingerInfo, Data.SessionKey);
            Assert.NotNull(bfdContext.AadhaarNumber);
            Assert.NotNull(bfdContext.Data);
            Assert.NotNull(bfdContext.DeviceInfo);
            Assert.NotNull(bfdContext.Hmac);
            Assert.NotNull(bfdContext.KeyInfo);
            Assert.Equal(Data.PersonalInfo.Timestamp, bfdContext.Timestamp);

            // Test 3: DeviceInfo device value are set to NA.
            Assert.Equal(Metadata.DeviceNotApplicable, bfdContext.DeviceInfo.IrisDeviceCode);
        }
    }
}