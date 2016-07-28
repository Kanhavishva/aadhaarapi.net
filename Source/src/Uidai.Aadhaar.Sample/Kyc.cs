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
        public static Configuration Configuration { get; set; }

        public static ISigner Signer { get; set; }

        public static IVerifier Verifier { get; set; }

        public static IKycDecryptor Decryptor { get; set; }

        public static async Task KnowYourCustomerAsync()
        {
            // A workaround to support version 1.0. Can be removed once v1.0 becomes obsolete.
            KycRequest.KycVersion = "1.0";

            #region Device Level

            // Generate OTP
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
                DeviceInfo = Configuration.DeviceInfo.Create()
            };

            // Ask for Resident Consent
            Console.Write("Access personal information? (y/n)\t\t");
            kycContext.HasResidentConsent = Console.ReadLine() == "y";

            Console.Write("Access information in Indian language? (y/n)\t");
            kycContext.AccessILInfo = Console.ReadLine() == "y";

            Console.Write("Access mobile and email information? (y/n)\t");
            kycContext.AccessMobileAndEmail = Console.ReadLine() == "y";

            // Encrypt Data
            using (var sessionKey = new SessionKey(Configuration.UidaiEncryptionKeyPath, true))
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
                AgencyInfo = Configuration.AgencyInfo,
                Request = new KycRequest(kycContext) { Signer = Signer },
                Response = new KycResponse { Decryptor = Decryptor, Verifier = Verifier }
            };
            await apiClient.GetResponseAsync();

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