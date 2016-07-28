using Microsoft.Extensions.Configuration;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Device;

namespace Uidai.Aadhaar.Sample
{
    /// <summary>
    /// Represents the configuration data of device and user agency.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets the agency information.
        /// </summary>
        /// <value>The agency information.</value>
        public UserAgency AgencyInfo { get; set; }

        /// <summary>
        /// Gets or sets the device information.
        /// </summary>
        /// <value>The device information.</value>
        public Metadata DeviceInfo { get; set; }

        /// <summary>
        /// Gets or sets the path to the X.509 certificate to encrypt session key.
        /// </summary>
        /// <value>The path to the X.509 certificate to encrypt session key.</value>
        public string UidaiEncryptionKeyPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the X.509 certificate to sign request XML.
        /// </summary>
        /// <value>The path to the X.509 certificate to sign request XML.</value>
        public string UidaiSignatureKeyPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the X.509 certificate to decrypt e-KYC response data.
        /// </summary>
        /// <value>The path to the X.509 certificate to decrypt e-KYC response data.</value>
        public string AuaDecryptionKeyPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the X.509 certificate to verify signature of response XML.
        /// </summary>
        /// <value>The path to the X.509 certificate to verify signature of response XML.</value>
        public string AuaSignatureKeyPath { get; set; }

        /// <summary>
        /// Reads a JSON configuration file and binds its value to <see cref="Configuration"/> instance.
        /// </summary>
        /// <returns>An instance of <see cref="Configuration"/>.</returns>
        public static Configuration GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("aadhaar.json").Build();
            var configuration = new Configuration();
            builder.Bind(configuration);

            return configuration;
        }
    }
}