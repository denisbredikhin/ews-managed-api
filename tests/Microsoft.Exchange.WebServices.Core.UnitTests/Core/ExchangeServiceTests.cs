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

namespace Microsoft.Exchange.WebServices.Core.UnitTests.Core;

using System;
using System.Globalization;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class ExchangeServiceTests
{
    #region Constructors

    [Fact]
    public void Constructor_Default_SetsExchange2013SP1Version()
    {
        var svc = new ExchangeService();

        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2013_SP1);
    }

    [Fact]
    public void Constructor_Default_UsesLocalTimeZone()
    {
        var svc = new ExchangeService();

        svc.TimeZone.Should().Be(TimeZoneInfo.Local);
    }

    [Fact]
    public void Constructor_WithVersion_SetsRequestedVersion()
    {
        var svc = new ExchangeService(ExchangeVersion.Exchange2010);

        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2010);
    }

    [Fact]
    public void Constructor_WithTimeZone_SetsTimeZone()
    {
        var svc = new ExchangeService(TimeZoneInfo.Utc);

        svc.TimeZone.Should().Be(TimeZoneInfo.Utc);
    }

    [Fact]
    public void Constructor_WithVersionAndTimeZone_SetsBoth()
    {
        var svc = new ExchangeService(ExchangeVersion.Exchange2016, TimeZoneInfo.Utc);

        svc.RequestedServerVersion.Should().Be(ExchangeVersion.Exchange2016);
        svc.TimeZone.Should().Be(TimeZoneInfo.Utc);
    }

    #endregion

    #region ExchangeService properties

    [Fact]
    public void Url_DefaultsToNull()
    {
        var svc = new ExchangeService();

        svc.Url.Should().BeNull();
    }

    [Fact]
    public void Url_GetSet_RoundTrips()
    {
        var svc = new ExchangeService();
        var uri = new Uri("https://mail.example.com/ews/exchange.asmx");

        svc.Url = uri;

        svc.Url.Should().Be(uri);
    }

    [Fact]
    public void ImpersonatedUserId_DefaultsToNull()
    {
        var svc = new ExchangeService();

        svc.ImpersonatedUserId.Should().BeNull();
    }

    [Fact]
    public void ImpersonatedUserId_GetSet_RoundTrips()
    {
        var svc = new ExchangeService();
        var userId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, "user@example.com");

        svc.ImpersonatedUserId = userId;

        svc.ImpersonatedUserId.Should().BeSameAs(userId);
    }

    [Fact]
    public void PreferredCulture_DefaultsToNull()
    {
        var svc = new ExchangeService();

        svc.PreferredCulture.Should().BeNull();
    }

    [Fact]
    public void PreferredCulture_GetSet_RoundTrips()
    {
        var svc = new ExchangeService();
        var culture = CultureInfo.GetCultureInfo("fr-FR");

        svc.PreferredCulture = culture;

        svc.PreferredCulture.Should().Be(culture);
    }

    [Fact]
    public void DateTimePrecision_DefaultsToDefault()
    {
        var svc = new ExchangeService();

        svc.DateTimePrecision.Should().Be(DateTimePrecision.Default);
    }

    [Fact]
    public void DateTimePrecision_GetSet_RoundTrips()
    {
        var svc = new ExchangeService
        {
            DateTimePrecision = DateTimePrecision.Milliseconds
        };

        svc.DateTimePrecision.Should().Be(DateTimePrecision.Milliseconds);
    }

    [Fact]
    public void EnableScpLookup_DefaultsToTrue()
    {
        var svc = new ExchangeService();

        svc.EnableScpLookup.Should().BeTrue();
    }

    [Fact]
    public void EnableScpLookup_GetSet_RoundTrips()
    {
        var svc = new ExchangeService
        {
            EnableScpLookup = false
        };

        svc.EnableScpLookup.Should().BeFalse();
    }

    [Fact]
    public void TraceEnablePrettyPrinting_DefaultsToTrue()
    {
        var svc = new ExchangeService();

        svc.TraceEnablePrettyPrinting.Should().BeTrue();
    }

    [Fact]
    public void TraceEnablePrettyPrinting_GetSet_RoundTrips()
    {
        var svc = new ExchangeService
        {
            TraceEnablePrettyPrinting = false
        };

        svc.TraceEnablePrettyPrinting.Should().BeFalse();
    }

    [Fact]
    public void UnifiedMessaging_IsNotNull()
    {
        var svc = new ExchangeService();

        svc.UnifiedMessaging.Should().NotBeNull();
    }

    [Fact]
    public void UnifiedMessaging_ReturnsSameInstanceOnRepeatedAccess()
    {
        var svc = new ExchangeService();

        svc.UnifiedMessaging.Should().BeSameAs(svc.UnifiedMessaging);
    }

    #endregion

    #region Inherited ExchangeServiceBase properties

    [Fact]
    public void Timeout_DefaultsTo100000()
    {
        var svc = new ExchangeService();

        svc.Timeout.Should().Be(100000);
    }

    [Fact]
    public void Timeout_SetValidValue_Persists()
    {
        var svc = new ExchangeService
        {
            Timeout = 30000
        };

        svc.Timeout.Should().Be(30000);
    }

    [Fact]
    public void Timeout_SetZero_ThrowsArgumentException()
    {
        var svc = new ExchangeService();

        Action act = () => svc.Timeout = 0;

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Timeout_SetNegative_ThrowsArgumentException()
    {
        var svc = new ExchangeService();

        Action act = () => svc.Timeout = -1;

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UserAgent_DefaultContainsExchangeServicesClient()
    {
        var svc = new ExchangeService();

        svc.UserAgent.Should().Contain("ExchangeServicesClient");
    }

    [Fact]
    public void UserAgent_SetCustomValue_AppendsOriginalAgentInParens()
    {
        var svc = new ExchangeService();
        var originalAgent = svc.UserAgent;

        svc.UserAgent = "MyApp/1.0";

        svc.UserAgent.Should().StartWith("MyApp/1.0");
        svc.UserAgent.Should().Contain($"({originalAgent})");
    }

    [Fact]
    public void Credentials_Set_ClearsUseDefaultCredentials()
    {
        var svc = new ExchangeService
        {
            UseDefaultCredentials = true,

            Credentials = new WebCredentials()
        };

        svc.UseDefaultCredentials.Should().BeFalse();
    }

    [Fact]
    public void Credentials_Set_ResetsCookieContainer()
    {
        var svc = new ExchangeService();
        var originalCookies = svc.CookieContainer;

        svc.Credentials = new WebCredentials();

        svc.CookieContainer.Should().NotBeSameAs(originalCookies);
    }

    [Fact]
    public void UseDefaultCredentials_SetTrue_ClearsCredentials()
    {
        var svc = new ExchangeService
        {
            Credentials = new WebCredentials(),

            UseDefaultCredentials = true
        };

        svc.Credentials.Should().BeNull();
    }

    [Fact]
    public void UseDefaultCredentials_SetTrue_ResetsCookieContainer()
    {
        var svc = new ExchangeService();
        var originalCookies = svc.CookieContainer;

        svc.UseDefaultCredentials = true;

        svc.CookieContainer.Should().NotBeSameAs(originalCookies);
    }

    [Fact]
    public void TraceEnabled_DefaultsToFalse()
    {
        var svc = new ExchangeService();

        svc.TraceEnabled.Should().BeFalse();
    }

    [Fact]
    public void TraceListener_SetNull_DisablesTracing()
    {
        var svc = new ExchangeService
        {
            TraceEnabled = true,

            TraceListener = null!
        };

        svc.TraceEnabled.Should().BeFalse();
    }

    [Fact]
    public void AcceptGzipEncoding_DefaultsToTrue()
    {
        var svc = new ExchangeService();

        svc.AcceptGzipEncoding.Should().BeTrue();
    }

    [Fact]
    public void KeepAlive_DefaultsToTrue()
    {
        var svc = new ExchangeService();

        svc.KeepAlive.Should().BeTrue();
    }

    [Fact]
    public void SendClientLatencies_DefaultsToTrue()
    {
        var svc = new ExchangeService();

        svc.SendClientLatencies.Should().BeTrue();
    }

    [Fact]
    public void HttpHeaders_DefaultsToEmptyCollection()
    {
        var svc = new ExchangeService();

        svc.HttpHeaders.Should().BeEmpty();
    }

    #endregion

    #region ValidateTargetVersion

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ValidateTargetVersion_NullOrEmpty_ThrowsArgumentException(string? version)
    {
        Action act = () => ExchangeService.ValidateTargetVersion(version!);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("2.4")]
    [InlineData("2.9")]
    [InlineData("Exchange2013")]
    [InlineData("Exchange2016")]
    [InlineData("Exchange2019")]
    public void ValidateTargetVersion_ValidVersion_DoesNotThrow(string version)
    {
        Action act = () => ExchangeService.ValidateTargetVersion(version);

        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateTargetVersion_WithValidMinimumParam_DoesNotThrow()
    {
        Action act = () => ExchangeService.ValidateTargetVersion("2.9; minimum=2.4");

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("badvalue")]
    [InlineData("notexchange")]
    [InlineData("2.4; bad=2.3")]
    [InlineData("1.2;x=y;z=w")]
    public void ValidateTargetVersion_InvalidVersion_ThrowsArgumentException(string version)
    {
        Action act = () => ExchangeService.ValidateTargetVersion(version);

        act.Should().Throw<ArgumentException>();
    }

    #endregion
}
