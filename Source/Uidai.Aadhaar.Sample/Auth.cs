using System;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Auth
    {
        private static readonly string BiometricData = "Rk1SACAyMAAAAADkAAgAyQFnAMUAxQEAAAARIQBqAGsgPgCIAG0fRwC2AG2dSQBVAIUjPABuALShMgCxAL0jMAByAM6lPgCmAN2kQQBwAN8qNAB1AN8mPADJAOcgOQA8AOorNABoAOomOQC+AO2fMQDFAPqlSgCvAP8lRQB8AQuhPABwAQ4fMgB7ASqcRADAAS4iNwCkATMeMwCFATYeNwBLATYwMQBWATcoMQCkATecMQBEATwyMgBJAUciQQCkAU8cNQB9AVQWNgCEAVUVRACoAVgYOgBBAV69NgCsAWeYNwAA";

        public static AadhaarOptions Options { get; set; }

        public static ISigner Signer { get; set; }

        public static IVerifier Verifier { get; set; }

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
                HasResidentConsent = true, // Should not be hardcoded in production environment.
                DeviceInfo = Options.DeviceInfo.Create()
            };

            // Encrypt Data
            using (var sessionKey = new SessionKey(Options.UidaiEncryptionKeyPath, false))
                await deviceContext.EncryptAsync(personalInfo, sessionKey);

            #endregion

            #region Device to Agency
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
                AgencyInfo = Options.AgencyInfo,
                Request = new AuthRequest(deviceContext) { Signer = Signer },
                Response = new AuthResponse { Verifier = Verifier }
            };
            await apiClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? $"Is the user authentic: {apiClient.Response.IsAuthentic}"
                : $"Error Code: {apiClient.Response.ErrorCode}");

            // Review Error, if any
            apiClient.Response.AuthInfo.Decode();
            var mismatch = string.Join(", ", apiClient.Response.AuthInfo.GetMismatch());
            if (!string.IsNullOrEmpty(mismatch))
                Console.WriteLine($"Mismatch Attributes: {mismatch}");

            #endregion

            #region Agency To Device
            // TODO: Wrap AuthResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}