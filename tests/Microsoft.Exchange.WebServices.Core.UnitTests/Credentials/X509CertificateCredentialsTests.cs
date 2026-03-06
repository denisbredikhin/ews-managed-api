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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

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
}
