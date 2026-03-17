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

namespace Microsoft.Exchange.WebServices.Core.UnitTests.Autodiscover;

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Task = System.Threading.Tasks.Task;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

[SupportedOSPlatform("windows")]
public class AutodiscoverServiceTests
{
    #region Constructors

    [Fact]
    public void Constructor_Default_SetsExchange2010Version()
    {
        var svc = new AutodiscoverService();

        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2010);
    }

    [Fact]
    public void Constructor_Default_NullDomainAndUrl()
    {
        var svc = new AutodiscoverService();

        svc.Domain.Should().BeNull();
        svc.Url.Should().BeNull();
    }

    [Fact]
    public void Constructor_Default_IsExternalIsTrue()
    {
        var svc = new AutodiscoverService();

        svc.IsExternal.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithVersion_SetsRequestedVersion()
    {
        var svc = new AutodiscoverService(ExchangeVersion.Exchange2013);

        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2013);
    }

    [Fact]
    public void Constructor_WithDomain_SetsDomain()
    {
        var svc = new AutodiscoverService("contoso.com");

        svc.Domain.Should().Be("contoso.com");
    }

    [Fact]
    public void Constructor_WithDomain_UrlIsNull()
    {
        var svc = new AutodiscoverService("contoso.com");

        svc.Url.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithDomainAndVersion_SetsBoth()
    {
        var svc = new AutodiscoverService("contoso.com", ExchangeVersion.Exchange2016);

        svc.Domain.Should().Be("contoso.com");
        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2016);
    }

    [Fact]
    public void Constructor_WithUrl_SetsUrl()
    {
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var svc = new AutodiscoverService(url);

        svc.Url.Should().Be(url);
    }

    [Fact]
    public void Constructor_WithUrl_SetsDomainToHost()
    {
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var svc = new AutodiscoverService(url);

        svc.Domain.Should().Be("autodiscover.contoso.com");
    }

    [Fact]
    public void Constructor_WithUrlAndVersion_SetsVersionAndUrl()
    {
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var svc = new AutodiscoverService(url, ExchangeVersion.Exchange2016);

        svc.Url.Should().Be(url);
        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2016);
    }

    [Fact]
    public void Constructor_WithInvalidDomain_ThrowsArgumentException()
    {
        Action act = () => _ = new AutodiscoverService("invalid domain!");

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Domain property

    [Fact]
    public void Domain_SetValidValue_UpdatesDomain()
    {
        var svc = new AutodiscoverService
        {
            Domain = "fabrikam.com"
        };

        svc.Domain.Should().Be("fabrikam.com");
    }

    [Fact]
    public void Domain_SetNonNullValue_ClearsUrl()
    {
        var svc = new AutodiscoverService(new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc"))
        {
            Domain = "fabrikam.com"
        };

        svc.Url.Should().BeNull();
    }

    [Fact]
    public void Domain_SetNull_DoesNotClearExistingUrl()
    {
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");
        var svc = new AutodiscoverService(url)
        {
            Domain = null!
        };

        // Url stays; Domain becomes null
        svc.Url.Should().Be(url);
        svc.Domain.Should().BeNull();
    }

    [Fact]
    public void Domain_SetInvalidValue_ThrowsArgumentException()
    {
        var svc = new AutodiscoverService();

        Action act = () => svc.Domain = "invalid domain!";

        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Url property

    [Fact]
    public void Url_DefaultsToNull()
    {
        var svc = new AutodiscoverService();

        svc.Url.Should().BeNull();
    }

    [Fact]
    public void Url_SetNonNullValue_UpdatesUrl()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        svc.Url = url;

        svc.Url.Should().Be(url);
    }

    [Fact]
    public void Url_SetNonNullValue_SetsDomainToHost()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        svc.Url = url;

        svc.Domain.Should().Be("autodiscover.contoso.com");
    }

    [Fact]
    public void Url_SetNull_DoesNotChangeDomain()
    {
        var svc = new AutodiscoverService("contoso.com")
        {
            Url = null!
        };

        svc.Domain.Should().Be("contoso.com");
    }

    #endregion

    #region IsExternal property

    [Fact]
    public void IsExternal_DefaultsToTrue()
    {
        var svc = new AutodiscoverService();

        svc.IsExternal.Should().BeTrue();
    }

    [Fact]
    public void IsExternal_SetFalse_ReturnsFalse()
    {
        var svc = new AutodiscoverService
        {
            IsExternal = false
        };

        svc.IsExternal.Should().BeFalse();
    }

    [Fact]
    public void IsExternal_SetNull_ReturnsNull()
    {
        var svc = new AutodiscoverService
        {
            IsExternal = null
        };

        svc.IsExternal.Should().BeNull();
    }

    #endregion

    #region EnableScpLookup property

    [Fact]
    public void EnableScpLookup_DefaultsToTrue()
    {
        var svc = new AutodiscoverService();

        svc.EnableScpLookup.Should().BeTrue();
    }

    [Fact]
    public void EnableScpLookup_SetFalse_Persists()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        svc.EnableScpLookup.Should().BeFalse();
    }

    #endregion

    #region RedirectionUrlValidationCallback property

    [Fact]
    public void RedirectionUrlValidationCallback_DefaultsToNull()
    {
        var svc = new AutodiscoverService();

        svc.RedirectionUrlValidationCallback.Should().BeNull();
    }

    [Fact]
    public void RedirectionUrlValidationCallback_SetValue_Persists()
    {
        var svc = new AutodiscoverService();
        AutodiscoverRedirectionUrlValidationCallback callback = _ => true;

        svc.RedirectionUrlValidationCallback = callback;

        svc.RedirectionUrlValidationCallback.Should().BeSameAs(callback);
    }

    #endregion

    #region GetScpUrlsForDomainCallback property

    [Fact]
    public void GetScpUrlsForDomainCallback_DefaultsToNull()
    {
        var svc = new AutodiscoverService();

        svc.GetScpUrlsForDomainCallback.Should().BeNull();
    }

    [Fact]
    public void GetScpUrlsForDomainCallback_SetValue_Persists()
    {
        var svc = new AutodiscoverService();
        Func<string, ICollection<string>> callback = _ => [];

        svc.GetScpUrlsForDomainCallback = callback;

        svc.GetScpUrlsForDomainCallback.Should().BeSameAs(callback);
    }

    #endregion

    #region GetAutodiscoverServiceUrls

    [Fact]
    public void GetAutodiscoverServiceUrls_ScpLookupDisabled_ReturnsTwoUrls()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        var urls = svc.GetAutodiscoverServiceUrls("contoso.com", out _);

        urls.Should().HaveCount(2);
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_ScpLookupDisabled_ScpHostCountIsZero()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        svc.GetAutodiscoverServiceUrls("contoso.com", out int scpHostCount);

        scpHostCount.Should().Be(0);
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_ScpLookupDisabled_IncludesDomainFallbackUrl()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        var urls = svc.GetAutodiscoverServiceUrls("contoso.com", out _);

        urls.Should().Contain(new Uri("https://contoso.com/autodiscover/autodiscover.xml"));
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_ScpLookupDisabled_IncludesAutodiscoverSubdomainFallbackUrl()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        var urls = svc.GetAutodiscoverServiceUrls("contoso.com", out _);

        urls.Should().Contain(new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.xml"));
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_WithScpCallback_IncludesScpUrlsFirst()
    {
        var svc = new AutodiscoverService
        {
            GetScpUrlsForDomainCallback = _ =>
                ["https://scp.contoso.com/autodiscover/autodiscover.xml"]
        };

        var urls = svc.GetAutodiscoverServiceUrls("contoso.com", out int scpHostCount);

        urls[0].Should().Be(new Uri("https://scp.contoso.com/autodiscover/autodiscover.xml"));
        scpHostCount.Should().Be(1);
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_WithScpCallback_TotalCountIsScpPlusTwoFallbacks()
    {
        var svc = new AutodiscoverService
        {
            GetScpUrlsForDomainCallback = _ =>
            [
                "https://scp1.contoso.com/autodiscover/autodiscover.xml",
                "https://scp2.contoso.com/autodiscover/autodiscover.xml"
            ]
        };

        var urls = svc.GetAutodiscoverServiceUrls("contoso.com", out _);

        urls.Should().HaveCount(4); // 2 SCP + 2 fallbacks
    }

    [Fact]
    public void GetAutodiscoverServiceUrls_WithScpCallback_ScpHostCountMatchesCallbackResults()
    {
        var svc = new AutodiscoverService
        {
            GetScpUrlsForDomainCallback = _ =>
            [
                "https://scp1.contoso.com/autodiscover/autodiscover.xml",
                "https://scp2.contoso.com/autodiscover/autodiscover.xml"
            ]
        };

        svc.GetAutodiscoverServiceUrls("contoso.com", out int scpHostCount);

        scpHostCount.Should().Be(2);
    }

    #endregion

    #region GetAutodiscoverServiceHosts

    [Fact]
    public void GetAutodiscoverServiceHosts_ScpLookupDisabled_ReturnsTwoHosts()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        var hosts = svc.GetAutodiscoverServiceHosts("contoso.com", out _);

        hosts.Should().HaveCount(2);
    }

    [Fact]
    public void GetAutodiscoverServiceHosts_ScpLookupDisabled_IncludesDomainAndSubdomain()
    {
        var svc = new AutodiscoverService { EnableScpLookup = false };

        var hosts = svc.GetAutodiscoverServiceHosts("contoso.com", out _);

        hosts.Should().Contain("contoso.com");
        hosts.Should().Contain("autodiscover.contoso.com");
    }

    [Fact]
    public void GetAutodiscoverServiceHosts_WithScpCallback_IncludesScpHost()
    {
        var svc = new AutodiscoverService
        {
            GetScpUrlsForDomainCallback = _ =>
                ["https://scp.contoso.com/autodiscover/autodiscover.xml"]
        };

        var hosts = svc.GetAutodiscoverServiceHosts("contoso.com", out int scpHostCount);

        hosts.Should().Contain("scp.contoso.com");
        scpHostCount.Should().Be(1);
    }

    #endregion

    #region AutodiscoverMaxRedirections constant

    [Fact]
    public void AutodiscoverMaxRedirections_IsTen()
    {
        AutodiscoverService.AutodiscoverMaxRedirections.Should().Be(10);
    }

    #endregion

    #region GetUserSettings – input validation

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetUserSettings_NullOrEmptySmtpAddress_ThrowsServiceValidationException(string? address)
    {
        var svc = new AutodiscoverService();

        Func<Task> act = () => svc.GetUserSettings(address!, UserSettingName.ExternalEwsUrl);

        await act.Should().ThrowAsync<ServiceValidationException>();
    }

    [Fact]
    public async Task GetUserSettings_NoSettings_ThrowsServiceValidationException()
    {
        var svc = new AutodiscoverService();

        Func<Task> act = () => svc.GetUserSettings("user@contoso.com");

        await act.Should().ThrowAsync<ServiceValidationException>();
    }

    #endregion

    #region GetUsersSettings – version gate

    [Fact]
    public void GetUsersSettings_RequestedVersionBelowExchange2010_ThrowsServiceVersionException()
    {
        var svc = new AutodiscoverService(ExchangeVersion.Exchange2007_SP1);

        Action act = () => svc.GetUsersSettings(
            ["user@contoso.com"],
            UserSettingName.ExternalEwsUrl);

        act.Should().Throw<ServiceVersionException>();
    }

    #endregion

    #region TryGetPartnerAccess – input validation

    [Fact]
    public async Task TryGetPartnerAccess_EmptyTargetDomain_ThrowsArgumentException()
    {
        var svc = new AutodiscoverService(new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc"));

        Func<Task> act = () => svc.TryGetPartnerAccess("  ");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task TryGetPartnerAccess_NullUrl_ThrowsServiceValidationException()
    {
        var svc = new AutodiscoverService();

        Func<Task> act = () => svc.TryGetPartnerAccess("contoso.com");

        await act.Should().ThrowAsync<ServiceValidationException>();
    }

    [Fact]
    public async Task TryGetPartnerAccess_VersionBelowExchange2010SP1_ThrowsServiceVersionException()
    {
        var svc = new AutodiscoverService(
            new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc"),
            ExchangeVersion.Exchange2010);

        Func<Task> act = () => svc.TryGetPartnerAccess("contoso.com");

        await act.Should().ThrowAsync<ServiceVersionException>();
    }

    #endregion

    #region PrepareHttpRequestMessageForUrl

    [Fact]
    public void PrepareHttpRequestMessageForUrl_HttpsUrl_ReturnsPostRequest()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Method.Should().Be(System.Net.Http.HttpMethod.Post);
        request.RequestUri.Should().Be(url);
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_HttpUrl_ReturnsPostRequest()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("http://autodiscover.contoso.com/autodiscover/autodiscover.xml");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Method.Should().Be(System.Net.Http.HttpMethod.Post);
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_FtpUrl_ThrowsServiceLocalException()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("ftp://autodiscover.contoso.com/autodiscover/autodiscover.xml");

        Action act = () => svc.PrepareHttpRequestMessageForUrl(url);

        act.Should().Throw<ServiceLocalException>();
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_SetsAcceptTextXmlHeader()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Headers.Accept.ToString().Should().Contain("text/xml");
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_SetsUserAgentHeader()
    {
        var svc = new AutodiscoverService();
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Headers.UserAgent.ToString().Should().Contain("ExchangeServicesClient");
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_WithClientRequestId_AddsHeader()
    {
        var svc = new AutodiscoverService { ClientRequestId = "my-request-id" };
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Headers.Contains("client-request-id").Should().BeTrue();
    }

    [Fact]
    public void PrepareHttpRequestMessageForUrl_WithReturnClientRequestId_AddsReturnHeader()
    {
        var svc = new AutodiscoverService
        {
            ClientRequestId = "my-request-id",
            ReturnClientRequestId = true
        };
        var url = new Uri("https://autodiscover.contoso.com/autodiscover/autodiscover.svc");

        var request = svc.PrepareHttpRequestMessageForUrl(url);

        request.Headers.Contains("return-client-request-id").Should().BeTrue();
    }

    #endregion
}
