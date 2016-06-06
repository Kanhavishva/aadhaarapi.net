using System;
using System.Linq;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Program
    {
        private static readonly string BiometricData = "Rk1SACAyMAAAAADkAAgAyQFnAMUAxQEAAAARIQBqAGsgPgCIAG0fRwC2AG2dSQBVAIUjPABuALShMgCxAL0jMAByAM6lPgCmAN2kQQBwAN8qNAB1AN8mPADJAOcgOQA8AOorNABoAOomOQC+AO2fMQDFAPqlSgCvAP8lRQB8AQuhPABwAQ4fMgB7ASqcRADAAS4iNwCkATMeMwCFATYeNwBLATYwMQBWATcoMQCkATecMQBEATwyMgBJAUciQQCkAU8cNQB9AVQWNgCEAVUVRACoAVgYOgBBAV69NgCsAWeYNwAA";

        private static readonly XmlSignature XmlSignature = new XmlSignature
        {
            Signer = Configuration.Current.AuaKey,
            Verifier = Configuration.Current.UidaiDigitalSignature
        };

        public static void Main(string[] args)
        {
            Console.Title = "Aadhaar API for .NET";

            AuthenticateAsync().GetAwaiter().GetResult();
            // BestFingerDetectionAsync().GetAwaiter().GetResult();
            // OneTimePasswordAsync().GetAwaiter().GetResult();
            // KnowYourCustomerAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        public static async Task AuthenticateAsync()
        {
            #region Device Level

            // Set Personal Info
            var personalInfo = new PersonalInfo
            {
                AadhaarNumber = "999999990019",
                Demographic = new Demographic
                {
                    Identity = new Identity
                    {
                        Name = "Shivshankar Choudhury",
                        DateOfBirth = new DateTime(1968, 5, 13, 0, 0, 0),
                        Gender = Gender.Male,
                        Phone = "2810806979",
                        Email = "sschoudhury@dummyemail.com"
                    },
                    Address = new Address
                    {
                        Street = "12 Maulana Azad Marg",
                        State = "New Delhi",
                        Pincode = "110002"
                    }
                }
            };
            personalInfo.Biometrics.Add(new Biometric
            {
                Type = BiometricType.Minutiae,
                Position = BiometricPosition.LeftIndex,
                Data = BiometricData
            });

            // Set Device Info
            var deviceContext = new AuthContext
            {
                DeviceInfo = Configuration.Current.DeviceInfo.Create(),
                Info = new AuthInfo()
            };

            // Encrypt Data
            var sessionKey = new SessionKey(Configuration.Current.UidaiEncryption, false);
            await deviceContext.EncryptAsync(personalInfo, sessionKey);

            #endregion

            #region Device To Agency
            // TODO: Wrap DeviceContext{T} into AUA specific protocol and send it to AUA.
            // On Device Side:
            // var deviceXml = deviceContext.ToXml();
            // var wrapped = WrapIntoAuaProtocol(deviceXml);

            // On Agency Side:
            // var auaXml = UnwrapFromAuaProtocol(wrapped);
            // var auaContext = new AuthContext();
            // auaContext.FromXml(auaXml);
            #endregion

            #region Agency Level

            // Perform Authentication
            var apiClient = new AuthClient
            {
                AgencyInfo = Configuration.Current.AgencyInfo,
                Request = new AuthRequest(deviceContext) { Signer = XmlSignature },
                Response = new AuthResponse { Verifier = XmlSignature }
            };
            await apiClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? $"Is the user authentic: {apiClient.Response.IsAuthentic}"
                : $"Error Code: {apiClient.Response.ErrorCode}");

            // Review Error
            apiClient.Response.Info.Decode();
            var mismatch = string.Join(", ", apiClient.Response.Info.GetMismatch());
            if (!string.IsNullOrEmpty(mismatch))
                Console.WriteLine($"Mismatch Attributes: {mismatch}");

            #endregion

            #region Agency To Device
            // TODO: Wrap AuthResponse into AUA specific protocol and send it to device.
            #endregion
        }

        public static async Task BestFingerDetectionAsync()
        {
            #region Device Level

            // Set Best Finger Info
            var bestFingerInfo = new BestFingerInfo
            {
                AadhaarNumber = "999999990019"
            };
            bestFingerInfo.Fingers.Add(new TestFinger { Quality = Nfiq.Excellent, NumberOfAttempts = 3, Position = BiometricPosition.LeftIndex, Data = BiometricData });
            bestFingerInfo.Fingers.Add(new TestFinger { Quality = Nfiq.Excellent, NumberOfAttempts = 4, Position = BiometricPosition.LeftMiddle, Data = BiometricData });
            bestFingerInfo.Fingers.Add(new TestFinger { Quality = Nfiq.Excellent, NumberOfAttempts = 5, Position = BiometricPosition.LeftRing, Data = BiometricData });

            // Set Device Info
            var bfdContext = new BfdContext
            {
                DeviceInfo = Configuration.Current.DeviceInfo.Create()
            };

            // Encrypt Data
            var sessionKey = new SessionKey(Configuration.Current.UidaiEncryption, false);
            await bfdContext.EncryptAsync(bestFingerInfo, sessionKey);

            #endregion

            #region Device To Agency
            // TODO: Wrap DeviceContext{T} into AUA specific protocol and send it to AUA.
            // On Device Side:
            // var deviceXml = deviceContext.ToXml();
            // var wrapped = WrapIntoAuaProtocol(deviceXml);

            // On Agency Side:
            // var auaXml = UnwrapFromAuaProtocol(wrapped);
            // var auaContext = new BfdContext();
            // auaContext.FromXml(auaXml);
            #endregion

            #region Agency Level

            // Perform Best Finger Detection
            var bfdClient = new BfdClient
            {
                AgencyInfo = Configuration.Current.AgencyInfo,
                Request = new BfdRequest(bfdContext) { Signer = XmlSignature },
                Response = new BfdResponse { Verifier = XmlSignature }
            };
            await bfdClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(bfdClient.Response.ErrorCode)
                ? $"Best Finger Is: {bfdClient.Response.Ranks.First().Value}"
                : $"Error Code: {bfdClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap BfdResponse into AUA specific protocol and send it to device.
            #endregion
        }

        public static async Task OneTimePasswordAsync()
        {
            OtpRequest.OtpVersion = "1.5";

            #region Device Level

            // Set OTP Info
            var otpContext = new OtpContext
            {
                AadhaarOrMobileNumber = "999999990019",
                Channel = OtpChannel.Sms
            };

            #endregion

            #region Device To Agency
            // TODO: Wrap DeviceContext{T} into AUA specific protocol and send it to AUA.
            // On Device Side:
            // var deviceXml = deviceContext.ToXml();
            // var wrapped = WrapIntoAuaProtocol(deviceXml);

            // On Agency Side:
            // var auaXml = UnwrapFromAuaProtocol(wrapped);
            // var auaContext = new OtpContext();
            // auaContext.FromXml(auaXml);
            #endregion

            #region Agency Level

            // Perform One Time Pin Request
            var otpClient = new OtpClient
            {
                AgencyInfo = Configuration.Current.AgencyInfo,
                Request = new OtpRequest(otpContext) { Signer = XmlSignature },
                Response = new OtpResponse()
            };
            await otpClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(otpClient.Response.ErrorCode)
                ? $"Is Sent: {otpClient.Response.IsOtpSent}"
                : $"Error Code: {otpClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap OtpResponse into AUA specific protocol and send it to device.
            #endregion
        }

        public static async Task KnowYourCustomerAsync()
        {
            #region Device Level

            // Set Personal Info
            Console.Write("Enter OTP sent to mobile: ");
            var otp = Console.ReadLine();
            var personalInfo = new PersonalInfo
            {
                AadhaarNumber = "999999990019",
                PinValue = new PinValue { Otp = otp }
            };

            // Set Device Info
            var kycContext = new KycContext
            {
                DeviceInfo = Configuration.Current.DeviceInfo.Create(),
                HasResidentConsent = true
            };

            // Ask for Resident Consent
            Console.Write("Does user have consent to access personal data? (y/n)\t");
            kycContext.HasResidentConsent = Console.ReadLine() == "y";

            // Encrypt Data
            var sessionKey = new SessionKey(Configuration.Current.UidaiEncryption, false);
            await kycContext.EncryptAsync(personalInfo, sessionKey);

            #endregion

            #region Device To Agency
            // TODO: Wrap DeviceContext{T} into AUA specific protocol and send it to AUA.
            // On Device Side:
            // var deviceXml = deviceContext.ToXml();
            // var wrapped = WrapIntoAuaProtocol(deviceXml);

            // On Agency Side:
            // var auaXml = UnwrapFromAuaProtocol(wrapped);
            // var auaContext = new KycContext();
            // auaContext.FromXml(auaXml);
            #endregion

            #region Agency Level

            // Perform e-Know Your Customer
            var kycClient = new KycClient
            {
                AgencyInfo = Configuration.Current.AgencyInfo,
                Request = new KycRequest(kycContext) { Signer = XmlSignature },
                Response = new KycResponse { Verifier = XmlSignature },
                Decrypter = new KycDecrypter { AuaKey = Configuration.Current.AuaKey }
            };
            await kycClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(kycClient.Response.ErrorCode)
                ? $"Is the user authentic: {kycClient.Response.Resident.Demographic.Identity.Name}"
                : $"Error Code: {kycClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap KycResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}