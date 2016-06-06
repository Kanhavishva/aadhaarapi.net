# Aadhaar API for .NET

[![Join the chat at https://gitter.im/souvikdc9/aadhaarapi.net](https://badges.gitter.im/souvikdc9/aadhaarapi.net.svg)](https://gitter.im/souvikdc9/aadhaarapi.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
Aadhaar API for .NET is a client library that provides an easy way to interact with the UIDAI Aadhaar services.

[Download NuGet Package](https://www.nuget.org/packages/Uidai.Aadhaar/)

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
personalInfo.Biometrics.Add(new Biometric
{
    Type = BiometricType.Minutiae,
    Position = BiometricPosition.LeftIndex,
    Data = BiometricData
});
```
### Encrypt Data
```csharp
var deviceContext = new AuthContext
{
    DeviceInfo = Configuration.Current.DeviceInfo.Create(),
};
var sessionKey = new SessionKey(Configuration.Current.UidaiEncryption, false);
await deviceContext.EncryptAsync(personalInfo, sessionKey);

// Send Data to AUA
// var wrapped = WrapIntoAuaProtocol(deviceContext.ToXml());
```

### Perform Authentication
```csharp
var apiClient = new AuthClient
{
    AgencyInfo = Configuration.Current.AgencyInfo,
    Request = new AuthRequest(deviceContext) { Signer = XmlSignature },
    Response = new AuthResponse { Verifier = XmlSignature }
};
await apiClient.GetResponseAsync();

Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
    ? $"Is the user authentic: {apiClient.Response.IsAuthentic}"
    : $"Error Code: {apiClient.Response.ErrorCode}");

// Review Error
apiClient.Response.Info.Decode();
var mismatch = string.Join(", ", apiClient.Response.Info.GetMismatch());
if (!string.IsNullOrEmpty(mismatch))
    Console.WriteLine($"Mismatch Attributes: {mismatch}");
```

### Configuration
```json
{
    "DeviceInfo": {
        "UniqueDeviceCode": "ABCDE.20150101.00001",
        "FingerprintDeviceCode": "NC",
        "IrisDeviceCode": "NC",
        "Location": "21,78,0",
        "LocationType": "GeoCoordinate"
    },
    "UIDAIEncryption": "Key/UidaiEncryptAndSign.cer",
    
    "AgencyInfo": {
        "AuaLicenseKey": "<AUA License Key>",
        "AuaCode": "public",
        "Hosts": {
            "Auth": "http://auth.uidai.gov.in/1.6/",
            "Bfd": "http://developer.uidai.gov.in/bfd/1.6/",
            "Kyc": "http://developer.uidai.gov.in/kyc/1.0/",
            "Otp": "http://developer.uidai.gov.in/otp/1.6/"
        }
    },
    "UIDAIDigitalSignature": "Key/UidaiEncryptAndSign.cer"
}
```

## BFD, OTP, e-KYC, Device Reset
Other UIDAI services can also be accessed in similar manner using their corresponding classes.

**See [Sample](/Source/source/Uidai.Aadhaar.Sample) project, for more details.**

## Issues
e-KYC is currently, not working.

## Copyright
Copyright © 2015 Souvik Dey Chowdhury

Licensed under GNU Lesser General Public License v3 or later.

## Contact Me
Souvik Dey Chowdhury (souvikdc@hotmail.com)
