using System;
using System.Linq;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Bfd
    {
        private static readonly string FingerData = "Rk1SACAyMAAAAADkAAgAyQFnAMUAxQEAAAARIQBqAGsgPgCIAG0fRwC2AG2dSQBVAIUjPABuALShMgCxAL0jMAByAM6lPgCmAN2kQQBwAN8qNAB1AN8mPADJAOcgOQA8AOorNABoAOomOQC+AO2fMQDFAPqlSgCvAP8lRQB8AQuhPABwAQ4fMgB7ASqcRADAAS4iNwCkATMeMwCFATYeNwBLATYwMQBWATcoMQCkATecMQBEATwyMgBJAUciQQCkAU8cNQB9AVQWNgCEAVUVRACoAVgYOgBBAV69NgCsAWeYNwAA";

        public static AadhaarOptions Options { get; set; }

        public static ISigner Signer { get; set; }

        public static IVerifier Verifier { get; set; }

        public static async Task DetectBestFingerAsync()
        {
            #region Device Level

            // Set Best Finger Info
            var bestFingerInfo = new BestFingerInfo { AadhaarNumber = "999999990019" };
            bestFingerInfo.Fingers.Add(new TestFinger
            {
                Quality = Nfiq.Excellent,
                NumberOfAttempts = 3,
                Position = BiometricPosition.LeftIndex,
                Data = FingerData
            });
            bestFingerInfo.Fingers.Add(new TestFinger
            {
                Quality = Nfiq.Excellent,
                NumberOfAttempts = 4,
                Position = BiometricPosition.LeftMiddle,
                Data = FingerData
            });
            bestFingerInfo.Fingers.Add(new TestFinger
            {
                Quality = Nfiq.Excellent,
                NumberOfAttempts = 5,
                Position = BiometricPosition.LeftRing,
                Data = FingerData
            });

            // Set Device Info
            var deviceContext = new BfdContext
            {
                DeviceInfo = Options.DeviceInfo.Create()
            };

            // Encrypt Data
            using (var sessionKey = new SessionKey(Options.UidaiEncryptionKeyPath, false))
                await deviceContext.EncryptAsync(bestFingerInfo, sessionKey);

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
            var apiClient = new BfdClient
            {
                AgencyInfo = Options.AgencyInfo,
                Request = new BfdRequest(deviceContext) { Signer = Signer },
                Response = new BfdResponse { Verifier = Verifier }
            };
            await apiClient.GetResponseAsync();

            Console.WriteLine(apiClient.Response.Message);

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? $"Best Finger Is: {apiClient.Response.Ranks.First().Value}"
                : $"Error Code: {apiClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap BfdResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}