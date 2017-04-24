using System;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Resident;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Kyc
    {
        public static AadhaarOptions Options { get; set; }

        public static ISigner Signer { get; set; }

        public static IVerifier Verifier { get; set; }

        public static IKycDecryptor Decryptor { get; set; }

        public static async Task KnowYourCustomerAsync()
        {
            #region Device Level

            // Generate OTP 999999990019
            await Otp.GenerateOtpAsync("999999990019");

            // Set Personal Info
            Console.Write("Enter OTP sent to mobile: ");
            var personalInfo = new PersonalInfo
            {
                AadhaarNumber = "999999990019",
                PinValue = new PinValue { Otp = Console.ReadLine() }
            };

            // Set Device Info
            var kycContext = new KycContext
            {
                DeviceInfo = Options.DeviceInfo.Create(),

                // Should not be hardcoded in production environment.
                HasResidentConsent = true,
                AccessILInfo = true,
                AccessMobileAndEmail = true,
            };

            // Encrypt Data
            using (var sessionKey = new SessionKey(Options.UidaiEncryptionKeyPath, true))
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
            var apiClient = new KycClient
            {
                AgencyInfo = Options.AgencyInfo,
                Request = new KycRequest(kycContext) { Signer = Signer },
                Response = new KycResponse { Decryptor = Decryptor, Verifier = Verifier }
            };
            await apiClient.GetResponseAsync(null, b =>
            {
                return b;
            });

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? $"Customer Name: {apiClient.Response.Resident.Demographic.Identity.Name}"
                : $"Error Code: {apiClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap KycResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}