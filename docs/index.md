---
layout: post
title: Aadhaar API for .NET
---

## Authentication Service
Aadhaar authentication is the process wherein Aadhaar Number, along with other attributes, including biometrics, are submitted online to the CIDR for its verification on the basis of information available with it.

### Collect Resident Data

```csharp
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
```

### Encrypt Data
```csharp
var deviceContext = new AuthContext
{
  // Load device metadata from configuration
  DeviceInfo = Configuration.Current.DeviceInfo.Create(),
};
var sessionKey = new SessionKey(Configuration.Current.UidaiEncryption, false);
await deviceContext.EncryptAsync(personalInfo, sessionKey);
```

### Perform Authentication
```csharp
var apiClient = new AuthClient
{
  // Load agency codes and certificates from configuration.
  AgencyInfo = Configuration.Current.AgencyInfo,
  Request = new AuthRequest(deviceContext) { Signer = XmlSignature },
  Response = new AuthResponse { Verifier = XmlSignature }
};
await apiClient.GetResponseAsync();

Console.WriteLine($"Is the user authentic: {apiClient.Response.IsAuthentic}");

// Review error, if any
apiClient.Response.Info.Decode();
var mismatch = string.Join(", ", apiClient.Response.Info.GetMismatch());
if (!string.IsNullOrEmpty(mismatch))
  Console.WriteLine($"Mismatch Attributes: {mismatch}");
```

### Configuration
```json
{
  "Aadhaar": {
  "DeviceInfo": {
    "UniqueDeviceCode": "ABCDE.20150101.00001",
    "FingerprintDeviceCode": "NC",
    "IrisDeviceCode": "NC",
    "Location": "21,78,0",
    "LocationType": "GeoCoordinate"
  },
  "UIDAIEncryptionKeyPath": "Key/Uidai.cer",
  "AgencyInfo": {
    "AuaLicenseKey": "MEaMX8fkRa6PqsqK6wGMrEXcXFl_oXHA-YuknI2uf0gKgZ80HaZgG3A",
    "AsaLicenseKey": "MG41KIrkk5moCkcO8w-2fc01-P7I5S-6X2-X7luVcDgZyOa2LXs3ELI",
    "AuaCode": "public",
    "Hosts": {
    "Auth": "http://auth.uidai.gov.in/1.6/",
    "Bfd": "http://developer.uidai.gov.in/bfd/1.6/",
    "Kyc": "http://developer.uidai.gov.in/kyc/1.0/",
    "Otp": "http://developer.uidai.gov.in/otp/1.6/"
    }
  },
  "UIDAISignatureKeyPath": "Key/Uidai.cer",
  "AUASignatureKeyPath": "Key/Aua.p12",
  "AUADecryptionKeyPath": "Key/Aua.p12"
  }
}
```

## BFD, OTP, e-KYC Services
Other UIDAI services can also be accessed in similar manner using their corresponding classes.

See [Sample](https://github.com/souvikdc9/aadhaarapi.net/tree/master/Source/Uidai.Aadhaar.Sample) project, for more details.