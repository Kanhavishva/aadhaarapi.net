using System;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Helper;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Otp
    {
        public static AadhaarOptions Options { get; set; }

        public static ISigner Signer { get; set; }

        public static async Task GenerateOtpAsync(string aadhaarNumber)
        {
            #region Device Level

            // Set OTP Info
            var otpContext = new OtpContext
            {
                AadhaarNumber = aadhaarNumber,
                Channel = OtpChannel.Sms,
                OtpInfo = new OtpInfo()
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
            var apiClient = new OtpClient
            {
                AgencyInfo = Options.AgencyInfo,
                Request = new OtpRequest(otpContext) { Signer = Signer },
                Response = new OtpResponse()
            };
            await apiClient.GetResponseAsync(null, b =>
            {
                return b;
            });

            apiClient.Request.OtpInfo.Encode();
            apiClient.Response.OtpInfo.Decode();

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? "OTP sent to mobile."
                : $"Error Code: {apiClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap OtpResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}