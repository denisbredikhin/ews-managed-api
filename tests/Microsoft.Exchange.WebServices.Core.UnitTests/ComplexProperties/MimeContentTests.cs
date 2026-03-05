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

namespace Microsoft.Exchange.WebServices.Core.UnitTests.ComplexProperties;

using System.Text;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class MimeContentTests
{
    [Fact]
    public void DefaultConstructor_ContentIsNull()
    {
        var mime = new MimeContent();

        mime.Content.Should().BeNull();
        mime.CharacterSet.Should().BeNull();
    }

    [Fact]
    public void FullConstructor_SetsProperties()
    {
        byte[] content = [72, 101, 108, 108, 111]; // "Hello"
        var mime = new MimeContent("UTF-8", content);

        mime.CharacterSet.Should().Be("UTF-8");
        mime.Content.Should().BeEquivalentTo(content);
    }

    [Fact]
    public void ToString_NullContent_ReturnsEmpty()
    {
        var mime = new MimeContent();

        mime.ToString().Should().BeEmpty();
    }

    [Fact]
    public void ToString_WithUtf8Content_DecodesCorrectly()
    {
        string text = "Hello, World!";
        byte[] content = Encoding.UTF8.GetBytes(text);
        var mime = new MimeContent("UTF-8", content);

        mime.ToString().Should().Be(text);
    }

    [Fact]
    public void ToString_WithNoCharacterSet_FallsBackToBase64()
    {
        // When CharacterSet is null, the code uses Encoding.UTF8.EncodingName (the display name,
        // e.g. "Unicode (UTF-8)") which Encoding.GetEncoding() cannot resolve, so it catches
        // ArgumentException and returns the base64 representation instead.
        byte[] content = Encoding.UTF8.GetBytes("Test content");
        var mime = new MimeContent(null!, content);

        mime.ToString().Should().Be(Convert.ToBase64String(content));
    }

    [Fact]
    public void ToString_WithInvalidCharacterSet_ReturnsBase64()
    {
        byte[] content = [1, 2, 3];
        var mime = new MimeContent("invalid-charset-xyz", content);

        var result = mime.ToString();

        result.Should().Be(Convert.ToBase64String(content));
    }
}
