using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Remove personal info before commit.

            // Load configuration.
            var options = new AadhaarOptions();
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Aadhaar")
                .Bind(options);

            var signerAndVerifier = new XmlSignature
            {
                Signer = new X509Certificate2(options.AuaSignatureKeyPath, "public", X509KeyStorageFlags.Exportable),
                Verifier = new X509Certificate2(options.UidaiSignatureKeyPath)
            };

            Auth.Options = Bfd.Options = Otp.Options = Kyc.Options = options;
            Auth.Signer = Bfd.Signer = Otp.Signer = Kyc.Signer = signerAndVerifier;
            Auth.Verifier = Bfd.Verifier = Kyc.Verifier = signerAndVerifier;
            Kyc.Decryptor = new KycDecryptor { KuaKey = signerAndVerifier.Signer };

            Auth.AuthenticateAsync().GetAwaiter().GetResult();
            // Bfd.DetectBestFingerAsync().GetAwaiter().GetResult();
            Kyc.KnowYourCustomerAsync().GetAwaiter().GetResult();

            Console.ReadLine();

            signerAndVerifier.Dispose();
        }
    }
}