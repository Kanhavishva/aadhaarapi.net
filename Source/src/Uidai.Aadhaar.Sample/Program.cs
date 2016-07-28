using System;
using System.Security.Cryptography.X509Certificates;
using Uidai.Aadhaar.Security;

namespace Uidai.Aadhaar.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = Configuration.GetConfiguration();
            Auth.Configuration = Bfd.Configuration = Otp.Configuration = Kyc.Configuration = configuration;

            var signerAndVerifier = new XmlSignature
            {
                Signer = new X509Certificate2(configuration.AuaSignatureKeyPath, "public", X509KeyStorageFlags.Exportable),
                Verifier = new X509Certificate2(configuration.UidaiSignatureKeyPath)
            };

            Auth.Signer = Bfd.Signer = Otp.Signer = Kyc.Signer = signerAndVerifier;
            Auth.Verifier = Bfd.Verifier = Kyc.Verifier = signerAndVerifier;

            // Just reference the signer key, as the signer and decryption key point to the same X.509 certificate.
            Kyc.Decryptor = new KycDecryptor { KuaKey = signerAndVerifier.Signer };

            Auth.AuthenticateAsync().GetAwaiter().GetResult();
            // Bfd.DetectBestFingerAsync().GetAwaiter().GetResult();
            // Kyc.KnowYourCustomerAsync().GetAwaiter().GetResult();

            Console.ReadLine();

            signerAndVerifier.Dispose();
        }
    }
}