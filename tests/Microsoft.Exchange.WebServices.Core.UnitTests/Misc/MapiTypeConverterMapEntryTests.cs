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

public class MapiTypeConverterMapEntryTests
{
    [Fact]
    public void Constructor_KnownType_Succeeds()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        entry.Type.Should().Be(typeof(int));
    }

    [Fact]
    public void DefaultValue_Int_IsZero()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        entry.DefaultValue.Should().Be(0);
    }

    [Fact]
    public void DefaultValue_String_IsNull()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(string));

        entry.DefaultValue.Should().BeNull();
    }

    [Fact]
    public void DefaultValue_Bool_IsFalse()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(bool));

        entry.DefaultValue.Should().Be(false);
    }

    [Fact]
    public void DefaultValue_DateTime_IsMinValue()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(DateTime));

        entry.DefaultValue.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void ConvertToValue_ValidStringForInt_Converts()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        var result = entry.ConvertToValue("42");

        result.Should().Be(42);
    }

    [Fact]
    public void ConvertToValue_InvalidString_ThrowsServiceXmlDeserializationException()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        Action act = () => entry.ConvertToValue("not_an_int");

        act.Should().Throw<ServiceXmlDeserializationException>();
    }

    [Fact]
    public void ConvertToValueOrDefault_EmptyString_ReturnsDefault()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        var result = entry.ConvertToValueOrDefault(string.Empty);

        result.Should().Be(0);
    }

    [Fact]
    public void ConvertToValueOrDefault_ValidString_Converts()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        var result = entry.ConvertToValueOrDefault("99");

        result.Should().Be(99);
    }

    [Fact]
    public void ChangeType_SameType_ReturnsSameValue()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(string));

        var result = entry.ChangeType("hello");

        result.Should().Be("hello");
    }

    [Fact]
    public void ChangeType_ConvertibleType_Converts()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(double));

        var result = entry.ChangeType(3); // int -> double

        result.Should().Be(3.0);
    }

    [Fact]
    public void ChangeType_IncompatibleType_ThrowsFormatException()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(bool));

        Action act = () => entry.ChangeType("not_a_bool_convertible");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ChangeType_ArrayWithValidType_ReturnsSameArray()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(string)) { IsArray = true };
        var arr = new string[] { "a", "b" };

        var result = entry.ChangeType(arr);

        result.Should().BeSameAs(arr);
    }

    [Fact]
    public void ChangeType_ArrayWithWrongElementType_Throws()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(string)) { IsArray = true };
        var arr = new int[] { 1, 2 };

        Action act = () => entry.ChangeType(arr);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ChangeType_EmptyArray_Throws()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(string)) { IsArray = true };

        Action act = () => entry.ChangeType(Array.Empty<string>());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsArray_DefaultIsFalse()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int));

        entry.IsArray.Should().BeFalse();
    }

    [Fact]
    public void IsArray_WhenSet_IsTrue()
    {
        var entry = new MapiTypeConverterMapEntry(typeof(int)) { IsArray = true };

        entry.IsArray.Should().BeTrue();
    }
}
