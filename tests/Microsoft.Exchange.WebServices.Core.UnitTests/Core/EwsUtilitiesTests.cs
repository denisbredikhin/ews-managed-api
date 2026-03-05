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

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class EwsUtilitiesTests
{
    #region Namespace mapping

    [Fact]
    public void GetNamespacePrefix_Types_ReturnsCorrectPrefix()
        => EwsUtilities.GetNamespacePrefix(XmlNamespace.Types).Should().Be(EwsUtilities.EwsTypesNamespacePrefix);

    [Fact]
    public void GetNamespacePrefix_Messages_ReturnsCorrectPrefix()
        => EwsUtilities.GetNamespacePrefix(XmlNamespace.Messages).Should().Be(EwsUtilities.EwsMessagesNamespacePrefix);

    [Fact]
    public void GetNamespacePrefix_Soap_ReturnsSoapPrefix()
        => EwsUtilities.GetNamespacePrefix(XmlNamespace.Soap).Should().Be(EwsUtilities.EwsSoapNamespacePrefix);

    [Fact]
    public void GetNamespacePrefix_Autodiscover_ReturnsAutodiscoverPrefix()
        => EwsUtilities.GetNamespacePrefix(XmlNamespace.Autodiscover).Should().Be(EwsUtilities.AutodiscoverSoapNamespacePrefix);

    [Fact]
    public void GetNamespacePrefix_UnknownNamespace_ReturnsEmpty()
    {
        EwsUtilities.GetNamespacePrefix(XmlNamespace.NotSpecified).Should().BeEmpty();
    }

    [Fact]
    public void GetNamespaceUri_Types_ReturnsCorrectUri()
        => EwsUtilities.GetNamespaceUri(XmlNamespace.Types).Should().Be(EwsUtilities.EwsTypesNamespace);

    [Fact]
    public void GetNamespaceUri_Messages_ReturnsCorrectUri()
        => EwsUtilities.GetNamespaceUri(XmlNamespace.Messages).Should().Be(EwsUtilities.EwsMessagesNamespace);

    [Fact]
    public void GetNamespaceUri_Soap_ReturnsCorrectUri()
        => EwsUtilities.GetNamespaceUri(XmlNamespace.Soap).Should().Be(EwsUtilities.EwsSoapNamespace);

    [Fact]
    public void GetNamespaceUri_Soap12_ReturnsCorrectUri()
        => EwsUtilities.GetNamespaceUri(XmlNamespace.Soap12).Should().Be(EwsUtilities.EwsSoap12Namespace);

    [Fact]
    public void GetNamespaceUri_UnknownNamespace_ReturnsEmpty()
    {
        EwsUtilities.GetNamespaceUri(XmlNamespace.NotSpecified).Should().BeEmpty();
    }

    [Fact]
    public void GetNamespaceFromUri_TypesNamespace_ReturnsTypes()
        => EwsUtilities.GetNamespaceFromUri(EwsUtilities.EwsTypesNamespace).Should().Be(XmlNamespace.Types);

    [Fact]
    public void GetNamespaceFromUri_MessagesNamespace_ReturnsMessages()
        => EwsUtilities.GetNamespaceFromUri(EwsUtilities.EwsMessagesNamespace).Should().Be(XmlNamespace.Messages);

    [Fact]
    public void GetNamespaceFromUri_SoapNamespace_ReturnsSoap()
        => EwsUtilities.GetNamespaceFromUri(EwsUtilities.EwsSoapNamespace).Should().Be(XmlNamespace.Soap);

    [Fact]
    public void GetNamespaceFromUri_UnknownUri_ReturnsNotSpecified()
    {
        EwsUtilities.GetNamespaceFromUri("http://unknown").Should().Be(XmlNamespace.NotSpecified);
    }

    #endregion

    #region Boolean conversion

    [Fact]
    public void BoolToXSBool_True_ReturnsXSTrue()
    {
        EwsUtilities.BoolToXSBool(true).Should().Be(EwsUtilities.XSTrue);
    }

    [Fact]
    public void BoolToXSBool_False_ReturnsXSFalse()
    {
        EwsUtilities.BoolToXSBool(false).Should().Be(EwsUtilities.XSFalse);
    }

    [Fact]
    public void XSTrue_EqualsLiteralTrue()
    {
        EwsUtilities.XSTrue.Should().Be("true");
    }

    [Fact]
    public void XSFalse_EqualsLiteralFalse()
    {
        EwsUtilities.XSFalse.Should().Be("false");
    }

    #endregion

    #region DateTime conversions

    [Fact]
    public void DateTimeToXSDateTime_UtcKind_AppendsZ()
    {
        var dt = new DateTime(2024, 3, 15, 10, 30, 0, DateTimeKind.Utc);

        var result = EwsUtilities.DateTimeToXSDateTime(dt);

        result.Should().EndWith("Z");
        result.Should().StartWith("2024-03-15T10:30:00");
    }

    [Fact]
    public void DateTimeToXSDateTime_UnspecifiedKind_NoOffset()
    {
        var dt = new DateTime(2024, 3, 15, 10, 30, 0, DateTimeKind.Unspecified);

        var result = EwsUtilities.DateTimeToXSDateTime(dt);

        result.Should().Be("2024-03-15T10:30:00.000");
    }

    [Fact]
    public void DateTimeToXSDate_UtcKind_AppendsZ()
    {
        var dt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc);

        var result = EwsUtilities.DateTimeToXSDate(dt);

        result.Should().Be("2024-03-15Z");
    }

    [Fact]
    public void DateTimeToXSDate_UnspecifiedKind_NoOffset()
    {
        var dt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Unspecified);

        var result = EwsUtilities.DateTimeToXSDate(dt);

        result.Should().Be("2024-03-15");
    }

    #endregion

    #region TimeSpan conversions

    [Fact]
    public void TimeSpanToXSDuration_SimplePositive_FormatsCorrectly()
    {
        var ts = new TimeSpan(1, 2, 30, 45);

        var result = EwsUtilities.TimeSpanToXSDuration(ts);

        result.Should().Be("P1DT2H30M45.0S");
    }

    [Fact]
    public void TimeSpanToXSDuration_Negative_HasMinusPrefix()
    {
        var ts = new TimeSpan(-1, 0, 0, 0);

        var result = EwsUtilities.TimeSpanToXSDuration(ts);

        result.Should().StartWith("-P");
    }

    [Fact]
    public void XSDurationToTimeSpan_ParsesSimpleDuration()
    {
        var result = EwsUtilities.XSDurationToTimeSpan("P1DT2H30M45S");

        result.Days.Should().Be(1);
        result.Hours.Should().Be(2);
        result.Minutes.Should().Be(30);
        result.Seconds.Should().Be(45);
    }

    [Fact]
    public void XSDurationToTimeSpan_InvalidInput_Throws()
    {
        Action act = () => EwsUtilities.XSDurationToTimeSpan("not_a_duration");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TimeSpanToXSTime_FormatsHHMMSS()
    {
        var ts = new TimeSpan(14, 30, 45);

        var result = EwsUtilities.TimeSpanToXSTime(ts);

        result.Should().Be("14:30:45");
    }

    #endregion

    #region DayOfWeek conversion

    [Theory]
    [InlineData(DayOfTheWeek.Monday, DayOfWeek.Monday)]
    [InlineData(DayOfTheWeek.Tuesday, DayOfWeek.Tuesday)]
    [InlineData(DayOfTheWeek.Sunday, DayOfWeek.Sunday)]
    [InlineData(DayOfTheWeek.Saturday, DayOfWeek.Saturday)]
    public void EwsToSystemDayOfWeek_SpecificDay_Converts(DayOfTheWeek ewsDay, DayOfWeek expected)
    {
        EwsUtilities.EwsToSystemDayOfWeek(ewsDay).Should().Be(expected);
    }

    [Theory]
    [InlineData(DayOfTheWeek.Day)]
    [InlineData(DayOfTheWeek.Weekday)]
    [InlineData(DayOfTheWeek.WeekendDay)]
    public void EwsToSystemDayOfWeek_AmbiguousValue_Throws(DayOfTheWeek ambiguous)
    {
        Action act = () => EwsUtilities.EwsToSystemDayOfWeek(ambiguous);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, DayOfTheWeek.Monday)]
    [InlineData(DayOfWeek.Friday, DayOfTheWeek.Friday)]
    public void SystemToEwsDayOfTheWeek_Converts(DayOfWeek systemDay, DayOfTheWeek expected)
    {
        EwsUtilities.SystemToEwsDayOfTheWeek(systemDay).Should().Be(expected);
    }

    #endregion

    #region Type name utilities

    [Fact]
    public void GetPrintableTypeName_SimpleType_ReturnsShortName()
    {
        EwsUtilities.GetPrintableTypeName(typeof(int)).Should().Be("int");
    }

    [Fact]
    public void GetPrintableTypeName_GenericType_ReturnsFormattedName()
    {
        var result = EwsUtilities.GetPrintableTypeName(typeof(List<int>));

        result.Should().Be("List<int>");
    }

    [Fact]
    public void GetPrintableTypeName_ArrayType_ReturnsArrayNotation()
    {
        var result = EwsUtilities.GetPrintableTypeName(typeof(string[]));

        result.Should().Be("string[]");
    }

    #endregion

    #region BuildVersion

    [Fact]
    public void BuildVersion_ReturnsNonNullNonEmpty()
    {
        EwsUtilities.BuildVersion.Should().NotBeNullOrEmpty();
    }

    #endregion
}
