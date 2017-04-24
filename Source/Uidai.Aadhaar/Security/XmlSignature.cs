#region Copyright
/********************************************************************************
 * Aadhaar API for .NET
 * Copyright © 2015 Souvik Dey Chowdhury
 * 
 * This file is part of Aadhaar API for .NET.
 * 
 * Aadhaar API for .NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 * 
 * Aadhaar API for .NET is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with Aadhaar API for .NET. If not, see http://www.gnu.org/licenses.
 ********************************************************************************/
#endregion

// XML Signing is not supported in .NET Core. Therefore, we have to target full .NET framework for signing.
// GitHub Issue: https://github.com/dotnet/corefx/issues/4278
#if NET46
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using static Uidai.Aadhaar.Internal.ErrorMessage;
using static Uidai.Aadhaar.Internal.ExceptionHelper;

namespace Uidai.Aadhaar.Security
{
    /// <summary>
    /// Provides a wrapper to sign or verify XML signature.
    /// </summary>
    /// <seealso cref="ISigner"/>
    /// <seealso cref="IVerifier"/>
    public class XmlSignature : ISigner, IVerifier, IDisposable
    {
        /// <summary>
        /// Gets or sets the X.509 certificate containing private key to sign the XML.
        /// </summary>
        /// <value>The X.509 certificate containing private key to sign the XML.</value>
        public X509Certificate2 Signer { get; set; }

        /// <summary>
        /// Gets or sets the X.509 certificate containing public key to verify the signed the XML.
        /// </summary>
        /// <value>The X.509 certificate containing public key to verify the signed the XML.</value>
        public X509Certificate2 Verifier { get; set; }

        /// <summary>
        /// Computes digital signature and adds it to the specified XML element.
        /// </summary>
        /// <param name="xml">The XML on which the signature is to be computed.</param>
        /// <returns>The Signature element of the XML.</returns>
        public XElement ComputeSignature(XElement xml)
        {
            ValidateNull(xml, nameof(xml));
            ValidateNull(Signer, nameof(Signer));
            if (!Signer.HasPrivateKey)
                throw new CryptographicException(NoPrivateKey);

            var signedXml = new SignedXml(GetXmlDocument(xml)) { SigningKey = Signer.PrivateKey };

            // Add Reference.
            var reference = new Reference(string.Empty);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            // Add Key Info.
            var keyInfo = new KeyInfo();
            var clause = new KeyInfoX509Data(Signer);
            clause.AddSubjectName(Signer.Subject);
            keyInfo.AddClause(clause);
            signedXml.KeyInfo = keyInfo;

            // Compute Signature.
            signedXml.ComputeSignature();
            var signatureXml = GetXDocument(signedXml.GetXml()).Root;
            xml.Add(signatureXml);

            return signatureXml;
        }

        /// <summary>
        /// Determines whether the digital signature of an XML is valid.
        /// </summary>
        /// <param name="xml">The signed XML.</param>
        /// <returns>true if the signature is valid; otherwise, false.</returns>
        public bool VerifySignature(XElement xml)
        {
            ValidateNull(xml, nameof(xml));
            ValidateNull(Verifier, nameof(Verifier));

            var document = GetXmlDocument(xml);
            var nodeList = document.GetElementsByTagName("Signature");
            if (nodeList.Count == 0)
                throw new CryptographicException(NoSignature);
            var signedXml = new SignedXml(document);
            signedXml.LoadXml((XmlElement)nodeList[0]);

            return signedXml.CheckSignature(Verifier.PublicKey.Key);
        }

        private static XDocument GetXDocument(XmlNode node)
        {
            var document = new XDocument();
            using (var writer = document.CreateWriter())
                node.WriteTo(writer);
            return document;
        }

        private static XmlDocument GetXmlDocument(XNode node)
        {
            var document = new XmlDocument { PreserveWhitespace = false };
            using (var reader = node.CreateReader())
                document.Load(reader);
            return document;
        }

        #region IDisposable Support

        private bool disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true if the method call comes from a <see cref="Dispose(bool)"/>; otherwise false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Signer?.Dispose();
                    Verifier?.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => Dispose(true);

        #endregion
    }
}
#endif