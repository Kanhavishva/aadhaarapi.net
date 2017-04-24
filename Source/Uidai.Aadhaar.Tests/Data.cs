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
using System.Net;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;

namespace Uidai.AadhaarTests
{
    public class Data
    {
        #region Security XML

        public static readonly string SessionKeyInfoXml = @"Data\Security\SessionKeyInfo.xml";

        #endregion

        #region Helper XML

        public static readonly string AadhaarLetterXml = @"Data\Helper\AadhaarLetter.xml";

        #endregion

        #region Resident XML

        public static readonly string AddressTxt = @"Data\Resident\Address.txt";
        public static readonly string AddressXml = @"Data\Resident\Address.xml";
        public static readonly string AuthUsageXml = @"Data\Resident\AuthUsage.xml";
        public static readonly string BestFingerInfoXml = @"Data\Resident\BestFingerInfo.xml";
        public static readonly string BiometricXml = @"Data\Resident\Biometric.xml";
        public static readonly string DemographicXml = @"Data\Resident\Demographic.xml";
        public static readonly string FullAddressXml = @"Data\Resident\FullAddress.xml";
        public static readonly string IdentityXml = @"Data\Resident\Identity.xml";
        public static readonly string PersonalInfoXml = @"Data\Resident\PersonalInfo.xml";
        public static readonly string PinValueXml = @"Data\Resident\PinValue.xml";
        public static readonly string TestFingerXml = @"Data\Resident\TestFinger.xml";

        #endregion

        #region Device XML

        public static readonly string AuthContextXml = @"Data\Device\AuthContext.xml";
        public static readonly string EncryptedDataXml = @"Data\Device\EncryptedData.xml";
        public static readonly string MetadataXml = @"Data\Device\Metadata.xml";
        public static readonly string OtpContextXml = @"Data\Device\OtpContext.xml";

        #endregion

        #region Agency XML

        public static readonly string AuthRequestXml = @"Data\Api\AuthRequest.xml";
        public static readonly string AuthResponseXml = @"Data\Api\AuthResponse.xml";
        public static readonly string BfdRequestXml = @"Data\Api\BfdRequest.xml";
        public static readonly string BfdResponseXml = @"Data\Api\BfdResponse.xml";
        public static readonly string DeviceResetRequestXml = @"Data\Api\DeviceResetRequest.xml";
        public static readonly string DeviceResetResponseXml = @"Data\Api\DeviceResetResponse.xml";
        public static readonly string KycRequestXml = @"Data\Api\KycRequest.xml";
        public static readonly string KycResponseXml = @"Data\Api\KycResponse.xml";
        public static readonly string OtpRequestXml = @"Data\Api\OtpRequest.xml";
        public static readonly string OtpResponseXml = @"Data\Api\OtpResponse.xml";

        #endregion

        #region Shared

        public static readonly string AadhaarNumber = "999999999999";
        public static readonly string ActionCode = new string('A', 10);
        public static readonly string AuaCode = "public";
        public static readonly string AuaLicenseKey = new string('L', 10);
        public static readonly string Hmac = new string('H', 10);
        public static readonly string InfoValue = new string('I', 10);
        public static readonly string Message = new string('M', 10);
        public static readonly string MobileNumber = "9876543210";
        public static readonly string Pincode = "999999";
        public static readonly string PublicTerminal = "public";
        public static readonly string RegisteredTerminal = "registered";
        public static readonly string ResponseCode = new string('R', 10);
        public static readonly string Transaction = "20150101000000000";
        public static readonly DateTimeOffset DateTime = new DateTime(2015, 1, 1);

        #endregion

        #region Resident

        public static Identity Identity => new Identity
        {
            Age = 1,
            DateOfBirth = DateTime,
            DoBType = DateOfBirthType.Declared,
            Email = "email",
            Gender = Gender.Female,
            ILName = "lname",
            ILNameMatchPercent = 60,
            Match = MatchingStrategy.Partial,
            Name = "name",
            NameMatchPercent = 50,
            Phone = "phone",
            VerifyOnlyBirthYear = false
        };

        public static Address Address => new Address
        {
            CareOf = "co",
            District = "dist",
            House = "house",
            Landmark = "lm",
            Locality = "loc",
            Pincode = Pincode,
            PostOffice = "po",
            State = "state",
            Street = "street",
            SubDistrict = "subdist",
            VillageOrCity = "vtc"
        };

        public static FullAddress FullAddress => new FullAddress
        {
            Address = "av",
            ILAddress = "lav",
            ILMatchPercent = 60,
            MatchPercent = 50
        };

        public static Demographic Demographic => new Demographic
        {
            FullAddress = FullAddress,
            Identity = Identity,
            LanguageUsed = IndianLanguage.Hindi
        };

        public static Biometric BiometricIris => new Biometric
        {
            Data = new string('I', 10),
            Position = BiometricPosition.LeftIris,
            Type = BiometricType.Iris
        };

        public static Biometric BiometricMinutiae => new Biometric
        {
            Data = new string('M', 10),
            Position = BiometricPosition.LeftIndex,
            Type = BiometricType.Minutiae
        };

        public static PinValue PinValue => new PinValue
        {
            Otp = "012345",
            Pin = "456789"
        };

        public static PersonalInfo PersonalInfo
        {
            get
            {
                var personalInfo = new PersonalInfo
                {
                    AadhaarNumber = AadhaarNumber,
                    Demographic = Demographic,
                    PinValue = PinValue,
                    Timestamp = DateTime
                };
                personalInfo.Biometrics.Add(BiometricMinutiae);
                personalInfo.Biometrics.Add(BiometricIris);
                return personalInfo;
            }
        }

        public static AuthUsage AuthUsage
        {
            get
            {
                var authUsage = new AuthUsage
                {
                    AuthUsed = AuthTypes.Biometric | AuthTypes.FullAddress | AuthTypes.Identity | AuthTypes.Otp | AuthTypes.Pin
                };
                authUsage.Biometrics.Add(BiometricType.Minutiae);
                authUsage.Biometrics.Add(BiometricType.Iris);
                return authUsage;
            }
        }

        public static TestFinger TestFinger => new TestFinger
        {
            Data = new string('M', 10),
            NumberOfAttempts = 3,
            Position = BiometricPosition.LeftIndex,
            Quality = Nfiq.Excellent
        };

        public static BestFingerInfo BestFingerInfo
        {
            get
            {
                var bestFingerInfo = new BestFingerInfo
                {
                    AadhaarNumber = AadhaarNumber,
                    Timestamp = DateTime
                };
                bestFingerInfo.Fingers.Add(TestFinger);
                return bestFingerInfo;
            }
        }

        #endregion

        #region Security

        public static SessionKey SessionKey => new SessionKey(@"Key\UidaiEncryptAndSign.cer", false);

        public static SessionKeyInfo SessionKeyInfo => new SessionKeyInfo
        {
            CertificateIdentifier = new DateTime(2015, 1, 1),
            Key = new string('S', 10),
            KeyIdentifier = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        #endregion

        #region Device

        public static Metadata Metadata => new Metadata
        {
            FingerprintDeviceCode = "F0001",
            IrisDeviceCode = "I0001",
            Location = "21,78,0",
            LocationType = LocationType.GeoCoordinate,
            PublicAddress = IPAddress.Parse("127.0.0.1"),
            UniqueDeviceCode = "ABCDE.20150101.0001"
        };

        public static EncryptedData EncryptedData => new EncryptedData
        {
            Data = new string('X', 10)
        };

        public static AuthContext AuthContext => new AuthContext
        {
            AadhaarNumber = AadhaarNumber,
            Data = EncryptedData,
            DeviceInfo = Metadata,
            Hmac = Hmac,
            KeyInfo = SessionKeyInfo,
            Terminal = PublicTerminal,
            Timestamp = PersonalInfo.Timestamp,
            Uses = AuthUsage
        };

        public static KycContext KycContext => new KycContext
        {
            AadhaarNumber = AadhaarNumber,
            AccessILInfo = true,
            AccessMobileAndEmail = true,
            Data = EncryptedData,
            DeviceInfo = Metadata,
            HasResidentConsent = true,
            Hmac = Hmac,
            KeyInfo = SessionKeyInfo,
            Terminal = PublicTerminal,
            Timestamp = PersonalInfo.Timestamp,
            Uses = AuthUsage
        };

        public static BfdContext BfdContext
        {
            get
            {
                var bfdContext = new BfdContext
                {
                    AadhaarNumber = AadhaarNumber,
                    Data = EncryptedData,
                    DeviceInfo = Metadata,
                    Hmac = Hmac,
                    KeyInfo = SessionKeyInfo,
                    Terminal = PublicTerminal,
                    Timestamp = BestFingerInfo.Timestamp
                };
                bfdContext.DeviceInfo.IrisDeviceCode = Metadata.DeviceNotApplicable;
                return bfdContext;
            }
        }

        public static OtpContext OtpContext => new OtpContext
        {
            AadhaarOrMobileNumber = MobileNumber,
            Channel = OtpChannel.Sms,
            RequestType = OtpRequestType.MobileNumber,
            Terminal = PublicTerminal
        };

        #endregion

        #region Agency

        public static AuthRequest AuthRequest => new AuthRequest(AuthContext)
        {
            AuaCode = AuaCode,
            AuaLicenseKey = AuaLicenseKey,
            Token = new Token { Value = "9876543210" },
            Transaction = "Auth:" + Transaction
        };

        public static AuthResponse AuthResponse => new AuthResponse
        {
            ActionCode = ActionCode,
            ErrorCode = "999",
            IsAuthentic = true,
            ResponseCode = ResponseCode,
            Timestamp = DateTime,
            Transaction = "Auth:" + Transaction,
            Info = new AuthInfo { InfoValue = InfoValue }
        };

        public static KycRequest KycRequest => new KycRequest(KycContext)
        {
            AuaCode = AuaCode,
            AuaLicenseKey = AuaLicenseKey,
            IsDecryptionByKsa = true,
            Transaction = "UKC:KYC:" + Transaction
        };

        public static KycResponse KycResponse => new KycResponse
        {
            ActionCode = ActionCode,
            IsAuthentic = true,
            ResponseCode = ResponseCode,
            Timestamp = DateTime,
            Transaction = "UKC:KYC:" + Transaction,
            Info = new AuthInfo { InfoValue = InfoValue },
            Resident = new PersonalInfo
            {
                AadhaarNumber = AadhaarNumber,
                Demographic = new Demographic
                {
                    LanguageUsed = IndianLanguage.Hindi,
                    Address = Address,
                    ILAddress = Address,
                    Identity = new Identity
                    {
                        Name = "name",
                        Phone = "phone",
                        Email = "email",
                        Gender = Gender.Female,
                        DateOfBirth = new DateTime(2015, 1, 1)
                    }
                },
                Photo = new string('P', 10).GetBytes()
            },
            TimeToLive = DateTime
        };

        public static BfdRequest BfdRequest => new BfdRequest(BfdContext)
        {
            AuaCode = AuaCode,
            AuaLicenseKey = AuaLicenseKey,
            Transaction = "BFD:" + Transaction
        };

        public static BfdResponse BfdResponse
        {
            get
            {
                var bfdResponse = new BfdResponse
                {
                    ActionCode = ActionCode,
                    Message = Message,
                    ResponseCode = ResponseCode,
                    Timestamp = DateTime,
                    Transaction = "BFD:" + Transaction
                };
                bfdResponse.Ranks.Add(1, BiometricPosition.LeftIndex);
                bfdResponse.Ranks.Add(2, BiometricPosition.LeftMiddle);
                bfdResponse.Ranks.Add(3, BiometricPosition.LeftRing);
                bfdResponse.Ranks.Add(4, BiometricPosition.LeftLittle);
                return bfdResponse;
            }
        }

        public static OtpRequest OtpRequest => new OtpRequest(OtpContext)
        {
            AuaCode = AuaCode,
            AuaLicenseKey = AuaLicenseKey,
            Transaction = "OTP:" + Transaction
        };

        public static OtpResponse OtpResponse => new OtpResponse
        {
            IsOtpSent = true,
            ResponseCode = ResponseCode,
            Timestamp = DateTime,
            Transaction = "OTP:" + Transaction,
            Info = new OtpInfo { InfoValue = InfoValue }
        };

        public static DeviceResetRequest DeviceResetRequest => new DeviceResetRequest
        {
            AuaCode = AuaCode,
            AuaLicenseKey = AuaLicenseKey,
            Data = new string('D', 10),
            Terminal = RegisteredTerminal,
            Transaction = "Reset:" + Transaction
        };

        public static DeviceResetResponse DeviceResetResponse => new DeviceResetResponse
        {
            DeviceCode = new string('D', 10),
            IsReset = true,
            ResponseCode = ResponseCode,
            Timestamp = DateTime,
            Transaction = "Reset:" + Transaction
        };

        #endregion
    }
}