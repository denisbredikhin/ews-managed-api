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

public class MapiTypeConverterTests
{
    [Fact]
    public void ConvertToValue_Boolean_ParsesTrue()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Boolean, "true");

        result.Should().Be(true);
    }

    [Fact]
    public void ConvertToValue_Boolean_ParsesFalse()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Boolean, "false");

        result.Should().Be(false);
    }

    [Fact]
    public void ConvertToValue_Integer_ParsesDecimal()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Integer, "42");

        result.Should().Be(42);
    }

    [Fact]
    public void ConvertToValue_Integer_FallsBackToStringForSchematized()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Integer, "NoData");

        result.Should().Be("NoData");
    }

    [Fact]
    public void ConvertToValue_Double_ParsesDecimal()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Double, "3.14");

        ((double)result).Should().BeApproximately(3.14, 0.0001);
    }

    [Fact]
    public void ConvertToValue_String_ReturnsString()
    {
        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.String, "hello world");

        result.Should().Be("hello world");
    }

    [Fact]
    public void ConvertToValue_Binary_DecodesBase64()
    {
        byte[] original = [1, 2, 3, 4];
        string base64 = Convert.ToBase64String(original);

        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.Binary, base64);

        result.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void ConvertToValue_SystemTime_ParsesDateTime()
    {
        string dateStr = "2024-03-15T10:30:00.000Z";

        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.SystemTime, dateStr);

        ((DateTime)result).Should().Be(new DateTime(2024, 3, 15, 10, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ConvertToString_Integer_ReturnsString()
    {
        var result = MapiTypeConverter.ConvertToString(MapiPropertyType.Integer, 42);

        result.Should().Be("42");
    }

    [Fact]
    public void ConvertToString_NullValue_ReturnsEmpty()
    {
        var result = MapiTypeConverter.ConvertToString(MapiPropertyType.String, null!);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ConvertToString_Boolean_ReturnsLowerCase()
    {
        var result = MapiTypeConverter.ConvertToString(MapiPropertyType.Boolean, true);

        result.Should().Be("true");
    }

    [Fact]
    public void ConvertToValue_StringList_ReturnsStringArray()
    {
        var strings = new[] { "a", "b", "c" };

        var result = MapiTypeConverter.ConvertToValue(MapiPropertyType.StringArray, strings);

        result.Should().BeEquivalentTo(strings);
    }

    [Fact]
    public void IsArrayType_ArrayType_ReturnsTrue()
    {
        MapiTypeConverter.IsArrayType(MapiPropertyType.IntegerArray).Should().BeTrue();
        MapiTypeConverter.IsArrayType(MapiPropertyType.StringArray).Should().BeTrue();
        MapiTypeConverter.IsArrayType(MapiPropertyType.BinaryArray).Should().BeTrue();
    }

    [Fact]
    public void IsArrayType_NonArrayType_ReturnsFalse()
    {
        MapiTypeConverter.IsArrayType(MapiPropertyType.Integer).Should().BeFalse();
        MapiTypeConverter.IsArrayType(MapiPropertyType.String).Should().BeFalse();
        MapiTypeConverter.IsArrayType(MapiPropertyType.Boolean).Should().BeFalse();
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("0", 0)]
    [InlineData("-100", -100)]
    public void ParseMapiIntegerValue_ValidInteger_ReturnsInt(string input, int expected)
    {
        var result = MapiTypeConverter.ParseMapiIntegerValue(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void ParseMapiIntegerValue_NonInteger_ReturnsOriginalString()
    {
        var result = MapiTypeConverter.ParseMapiIntegerValue("NotAnInt");

        result.Should().Be("NotAnInt");
    }

    [Fact]
    public void MapiTypeConverterMap_ContainsAllExpectedTypes()
    {
        var map = MapiTypeConverter.MapiTypeConverterMap;

        map.Should().ContainKey(MapiPropertyType.Boolean);
        map.Should().ContainKey(MapiPropertyType.Integer);
        map.Should().ContainKey(MapiPropertyType.String);
        map.Should().ContainKey(MapiPropertyType.SystemTime);
        map.Should().ContainKey(MapiPropertyType.Binary);
    }
}
