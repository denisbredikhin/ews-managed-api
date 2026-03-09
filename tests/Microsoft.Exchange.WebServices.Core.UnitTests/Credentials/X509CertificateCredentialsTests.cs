/*
 * Exchange Web Services Managed API
 *
 * Copyright (c) Microsoft Corporation
 * All rights reserved.
 *
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

namespace Microsoft.Exchange.WebServices.Core.UnitTests.Credentials;

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class X509CertificateCredentialsTests : IDisposable
{
    // Self-signed cert with private key, created once per test class instance.
    private readonly X509Certificate2 _certWithKey = CreateSelfSignedCertificate();

    // Certificate exported without private key for negative-path tests.
    private readonly X509Certificate2 _certWithoutKey;

    public X509CertificateCredentialsTests()
    {
        // Export only the public portion so HasPrivateKey == false.
        _certWithoutKey = X509CertificateLoader.LoadCertificate(_certWithKey.Export(X509ContentType.Cert));
    }

    public void Dispose()
    {
        _certWithKey.Dispose();
        _certWithoutKey.Dispose();
    }

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_NullCertificate_ThrowsArgumentException()
    {
        Action act = () => _ = new X509CertificateCredentials(null!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_CertificateWithoutPrivateKey_ThrowsServiceValidationException()
    {
        Action act = () => _ = new X509CertificateCredentials(_certWithoutKey);

        act.Should().Throw<ServiceValidationException>();
    }

    [Fact]
    public void Constructor_ValidCertificate_SetsSecurityToken()
    {
        var creds = new X509CertificateCredentials(_certWithKey);

        creds.SecurityToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_ValidCertificate_SecurityTokenContainsBinarySecurityTokenElement()
    {
        var creds = new X509CertificateCredentials(_certWithKey);

        creds.SecurityToken.Should().Contain("wsse:BinarySecurityToken");
    }

    [Fact]
    public void Constructor_ValidCertificate_SecurityTokenContainsBase64EncodedCertData()
    {
        var creds = new X509CertificateCredentials(_certWithKey);

        string expected = Convert.ToBase64String(_certWithKey.GetRawCertData());
        creds.SecurityToken.Should().Contain(expected);
    }

    // -------------------------------------------------------------------------
    // NeedSignature
    // -------------------------------------------------------------------------

    [Fact]
    public void NeedSignature_AlwaysReturnsTrue()
    {
        var creds = new X509CertificateCredentials(_certWithKey);

        creds.NeedSignature.Should().BeTrue();
    }

    // -------------------------------------------------------------------------
    // AdjustUrl
    // -------------------------------------------------------------------------

    [Fact]
    public void AdjustUrl_PlainUrl_AppendsCertSuffix()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        var url = new Uri("https://mail.example.com/EWS/Exchange.asmx");

        var adjusted = creds.AdjustUrl(url);

        adjusted.AbsoluteUri.Should().Be("https://mail.example.com/EWS/Exchange.asmx/wssecurity/x509cert");
    }

    [Fact]
    public void AdjustUrl_UrlAlreadyHasWsSecuritySuffix_ReplacesWithCertSuffix()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        // URL already ends with the generic /wssecurity suffix.
        var url = new Uri("https://mail.example.com/EWS/Exchange.asmx/wssecurity");

        var adjusted = creds.AdjustUrl(url);

        // GetUriWithoutSuffix strips /wssecurity, then the cert suffix is appended.
        adjusted.AbsoluteUri.Should().Be("https://mail.example.com/EWS/Exchange.asmx/wssecurity/x509cert");
    }

    // -------------------------------------------------------------------------
    // ToString
    // -------------------------------------------------------------------------

    [Fact]
    public void ToString_ReturnsIssuerAndSubject()
    {
        var creds = new X509CertificateCredentials(_certWithKey);

        string result = creds.ToString();

        result.Should().StartWith("X509:<I>=");
        result.Should().Contain("<S>=");
        result.Should().Contain(_certWithKey.Issuer);
        result.Should().Contain(_certWithKey.Subject);
    }

    // -------------------------------------------------------------------------
    // Sign
    // -------------------------------------------------------------------------

    [Fact]
    public void Sign_ValidSoapDocument_AppendsSignatureToSecurityNode()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeStream();

        creds.Sign(stream);

        var doc = LoadXmlFromStream(stream);
        var securityNode = doc.SelectSingleNode(
            "/soap:Envelope/soap:Header/wsse:Security",
            WSSecurityBasedCredentials.NamespaceManager);

        securityNode.Should().NotBeNull();
        securityNode!.ChildNodes.Cast<XmlNode>()
            .Should().Contain(n => n.LocalName == "Signature");
    }

    [Fact]
    public void Sign_ValidSoapDocument_ToElementReceivesWsuId()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeStream();

        creds.Sign(stream);

        var doc = LoadXmlFromStream(stream);
        var toElement = (XmlElement?)doc.SelectSingleNode(
            "/soap:Envelope/soap:Header/wsa:To",
            WSSecurityBasedCredentials.NamespaceManager);

        toElement.Should().NotBeNull();
        toElement!.GetAttribute("Id", EwsUtilities.WSSecurityUtilityNamespace)
            .Should().NotBeNullOrEmpty("wsa:To must be stamped with a wsu:Id so it can be referenced in the signature");
    }

    [Fact]
    public void Sign_ValidSoapDocument_TimestampElementReceivesWsuId()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeStream();

        creds.Sign(stream);

        var doc = LoadXmlFromStream(stream);
        var tsElement = (XmlElement?)doc.SelectSingleNode(
            "/soap:Envelope/soap:Header/wsse:Security/wsu:Timestamp",
            WSSecurityBasedCredentials.NamespaceManager);

        tsElement.Should().NotBeNull();
        tsElement!.GetAttribute("Id", EwsUtilities.WSSecurityUtilityNamespace)
            .Should().NotBeNullOrEmpty("wsu:Timestamp must be stamped with a wsu:Id so it can be referenced in the signature");
    }

    [Fact]
    public void Sign_ValidSoapDocument_ProducesVerifiableSignature()
    {
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeStream();

        creds.Sign(stream);

        var doc = LoadXmlFromStream(stream);
        var signatureNode = (XmlElement?)doc
            .GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
            .Item(0);

        signatureNode.Should().NotBeNull();

        // Use a helper that resolves wsu:Id references so CheckSignature can
        // walk the signed references back to their elements.
        var verifier = new WsuIdAwareSignedXml(doc);
        verifier.LoadXml(signatureNode!);

        verifier.CheckSignature(_certWithKey, verifySignatureOnly: true)
            .Should().BeTrue("the signature must be verifiable with the signer's public key");
    }

    [Fact]
    public void Sign_SoapMissingWsaToElement_DoesNotThrow()
    {
        // wsa:To is optional — its absence is tolerated; wsu:Timestamp is still signed.
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeWithoutWsaToStream();

        Action act = () => creds.Sign(stream);

        act.Should().NotThrow();
    }

    [Fact]
    public void Sign_NonZeroInitialStreamPosition_StillProducesValidOutput()
    {
        // Sign resets the stream to position 0 before loading, so the caller's
        // initial position should have no effect on the result.
        var creds = new X509CertificateCredentials(_certWithKey);
        using var stream = BuildSoapEnvelopeStream();
        stream.Position = 10;

        creds.Sign(stream);

        // The stream must contain well-formed XML with a Signature element.
        var doc = LoadXmlFromStream(stream);
        doc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#")
            .Count.Should().Be(1);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static X509Certificate2 CreateSelfSignedCertificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=EwsTestCert",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var cert = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(1));

        // Export + re-import with the private key flag so HasPrivateKey is reliable.
        return X509CertificateLoader.LoadPkcs12(
            cert.Export(X509ContentType.Pfx),
            password: null,
            X509KeyStorageFlags.EphemeralKeySet);
    }

    /// <summary>
    /// Builds a full SOAP envelope that contains every element referenced by Sign:
    /// wsa:To, wsse:Security, and wsu:Timestamp.
    /// </summary>
    private static MemoryStream BuildSoapEnvelopeStream()
    {
        string xml =
            $"<soap:Envelope" +
            $" xmlns:soap=\"{EwsUtilities.EwsSoapNamespace}\"" +
            $" xmlns:wsa=\"{EwsUtilities.WSAddressingNamespace}\"" +
            $" xmlns:wsse=\"{EwsUtilities.WSSecuritySecExtNamespace}\"" +
            $" xmlns:wsu=\"{EwsUtilities.WSSecurityUtilityNamespace}\">" +
            "<soap:Header>" +
            "<wsa:To>https://mail.example.com/EWS/Exchange.asmx</wsa:To>" +
            "<wsse:Security>" +
            "<wsu:Timestamp>" +
            "<wsu:Created>2026-03-09T19:00:00Z</wsu:Created>" +
            "<wsu:Expires>2026-03-09T19:05:00Z</wsu:Expires>" +
            "</wsu:Timestamp>" +
            "</wsse:Security>" +
            "</soap:Header>" +
            "<soap:Body>" +
            "<GetItem xmlns=\"http://schemas.microsoft.com/exchange/services/2006/messages\"/>" +
            "</soap:Body>" +
            "</soap:Envelope>";

        return ToStream(xml);
    }

    /// <summary>
    /// SOAP envelope without wsa:To — exercises the path where one optional
    /// reference target is absent but signing still proceeds on wsu:Timestamp.
    /// </summary>
    private static MemoryStream BuildSoapEnvelopeWithoutWsaToStream()
    {
        string xml =
            $"<soap:Envelope" +
            $" xmlns:soap=\"{EwsUtilities.EwsSoapNamespace}\"" +
            $" xmlns:wsse=\"{EwsUtilities.WSSecuritySecExtNamespace}\"" +
            $" xmlns:wsu=\"{EwsUtilities.WSSecurityUtilityNamespace}\">" +
            "<soap:Header>" +
            "<wsse:Security>" +
            "<wsu:Timestamp>" +
            "<wsu:Created>2026-03-09T19:00:00Z</wsu:Created>" +
            "<wsu:Expires>2026-03-09T19:05:00Z</wsu:Expires>" +
            "</wsu:Timestamp>" +
            "</wsse:Security>" +
            "</soap:Header>" +
            "<soap:Body/>" +
            "</soap:Envelope>";

        return ToStream(xml);
    }

    private static MemoryStream ToStream(string xml)
    {
        var ms = new MemoryStream();
        var bytes = Encoding.UTF8.GetBytes(xml);
        ms.Write(bytes, 0, bytes.Length);
        ms.Position = 0;
        return ms;
    }

    private static XmlDocument LoadXmlFromStream(MemoryStream stream)
    {
        stream.Position = 0;
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.Load(stream);
        return doc;
    }

    /// <summary>
    /// A <see cref="SignedXml"/> subclass that resolves <c>wsu:Id</c> attributes
    /// so that <see cref="SignedXml.CheckSignature"/> can walk back to the
    /// elements that were signed by <see cref="X509CertificateCredentials.Sign"/>.
    /// </summary>
    private sealed class WsuIdAwareSignedXml : SignedXml
    {
        private readonly XmlDocument _doc;

        public WsuIdAwareSignedXml(XmlDocument doc) : base(doc) => _doc = doc;

        public override XmlElement? GetIdElement(XmlDocument document, string idValue)
            => base.GetIdElement(document, idValue)
               ?? (XmlElement?)_doc.SelectSingleNode(
                   $"//*[@wsu:Id='{idValue}']",
                   WSSecurityBasedCredentials.NamespaceManager);
    }
}
