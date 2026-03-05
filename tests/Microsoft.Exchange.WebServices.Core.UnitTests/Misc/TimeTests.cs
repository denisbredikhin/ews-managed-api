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

namespace Microsoft.Exchange.WebServices.Core.UnitTests.Misc;

using System;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class TimeTests
{
    [Fact]
    public void DefaultConstructor_ProducesZeroTime()
    {
        var time = new Time();

        time.Hours.Should().Be(0);
        time.Minutes.Should().Be(0);
        time.Seconds.Should().Be(0);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(90, 1, 30)]
    [InlineData(60, 1, 0)]
    [InlineData(1439, 23, 59)]
    public void MinutesConstructor_SetsHoursAndMinutesCorrectly(int totalMinutes, int expectedHours, int expectedMinutes)
    {
        var time = new Time(totalMinutes);

        time.Hours.Should().Be(expectedHours);
        time.Minutes.Should().Be(expectedMinutes);
        time.Seconds.Should().Be(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1440)]
    [InlineData(2000)]
    public void MinutesConstructor_OutOfRange_Throws(int minutes)
    {
        Action act = () => _ = new Time(minutes);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DateTimeConstructor_ExtractsTimeParts()
    {
        var dt = new DateTime(2024, 1, 15, 14, 30, 45);

        var time = new Time(dt);

        time.Hours.Should().Be(14);
        time.Minutes.Should().Be(30);
        time.Seconds.Should().Be(45);
    }

    [Fact]
    public void HoursMinutesSecondsConstructor_SetsAllParts()
    {
        var time = new Time(10, 20, 30);

        time.Hours.Should().Be(10);
        time.Minutes.Should().Be(20);
        time.Seconds.Should().Be(30);
    }

    [Theory]
    [InlineData(24)]
    [InlineData(-1)]
    public void Hours_OutOfRange_Throws(int hours)
    {
        var time = new Time();
        Action act = () => time.Hours = hours;

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(60)]
    [InlineData(-1)]
    public void Minutes_OutOfRange_Throws(int minutes)
    {
        var time = new Time();
        Action act = () => time.Minutes = minutes;

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(60)]
    [InlineData(-1)]
    public void Seconds_OutOfRange_Throws(int seconds)
    {
        var time = new Time();
        Action act = () => time.Seconds = seconds;

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, 0, 0, "00:00:00")]
    [InlineData(9, 5, 3, "09:05:03")]
    [InlineData(23, 59, 59, "23:59:59")]
    [InlineData(14, 0, 0, "14:00:00")]
    public void ToXSTime_FormatsCorrectly(int h, int m, int s, string expected)
    {
        var time = new Time(h, m, s);

        time.ToXSTime().Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 30, 90)]
    [InlineData(2, 0, 120)]
    [InlineData(23, 59, 1439)]
    public void ConvertToMinutes_ReturnsCorrectValue(int hours, int minutes, int expectedMinutes)
    {
        var time = new Time(hours, minutes, 0);

        time.ConvertToMinutes().Should().Be(expectedMinutes);
    }
}
