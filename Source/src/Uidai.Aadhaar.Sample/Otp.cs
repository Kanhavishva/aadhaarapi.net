using System;
using System.Threading.Tasks;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Api;
using Uidai.Aadhaar.Device;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Otp
    {
        public static Configuration Configuration { get; set; }

        public static ISigner Signer { get; set; }

        public static async Task GenerateOtpAsync(string aadhaarNumber)
        {
            // A workaround to support version 1.5. Can be removed once v1.5 becomes obsolete.
            OtpRequest.OtpVersion = "1.5";

            #region Device Level

            // Set OTP Info
            var otpContext = new OtpContext
            {
                AadhaarOrMobileNumber = aadhaarNumber,
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
            var apiClient = new OtpClient
            {
                AgencyInfo = Configuration.AgencyInfo,
                Request = new OtpRequest(otpContext) { Signer = Signer },
                Response = new OtpResponse()
            };
            await apiClient.GetResponseAsync();

            Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
                ? $"Is OTP Sent: {apiClient.Response.IsOtpSent}"
                : $"Error Code: {apiClient.Response.ErrorCode}");

            #endregion

            #region Agency To Device
            // TODO: Wrap OtpResponse into AUA specific protocol and send it to device.
            #endregion
        }
    }
}