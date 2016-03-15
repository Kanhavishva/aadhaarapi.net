using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Uidai.Aadhaar.Agency;
using Uidai.Aadhaar.Device;

namespace Uidai.Aadhaar.Sample
{
    public class Configuration
    {
        public Configuration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json").Build();
            AgencyInfo = configuration.Get<UserAgency>("AgencyInfo");
            DeviceInfo = configuration.Get<Metadata>("DeviceInfo");
            UidaiEncryption = new X509Certificate2(configuration.Get<string>("UIDAIEncryption"));
            UidaiDigitalSignature = new X509Certificate2(configuration.Get<string>("UidaiDigitalSignature"));
        }

        public static Configuration Current { get; } = new Configuration();

        public UserAgency AgencyInfo { get; set; }

        public Metadata DeviceInfo { get; set; }

        public X509Certificate2 AuaKey { get; set; } = new X509Certificate2(@"Key\AuaKey.p12", "public");

        public X509Certificate2 UidaiEncryption { get; set; }

        public X509Certificate2 UidaiDigitalSignature { get; set; }
    }
}