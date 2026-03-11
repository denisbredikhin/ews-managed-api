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

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class MessageBodyTests
{
    [Fact]
    public void DefaultConstructor_HasDefaultValues()
    {
        var body = new MessageBody();

        body.BodyType.Should().Be(BodyType.HTML);
        body.Text.Should().BeNull();
    }

    [Fact]
    public void BodyTypeAndTextConstructor_SetsBothFields()
    {
        var body = new MessageBody(BodyType.HTML, "<b>Hello</b>");

        body.BodyType.Should().Be(BodyType.HTML);
        body.Text.Should().Be("<b>Hello</b>");
    }

    [Fact]
    public void TextOnlyConstructor_DefaultsToHtml()
    {
        var body = new MessageBody("plain text");

        body.BodyType.Should().Be(BodyType.HTML);
        body.Text.Should().Be("plain text");
    }

    [Fact]
    public void ToString_WithText_ReturnsText()
    {
        var body = new MessageBody(BodyType.Text, "Hello World");

        body.ToString().Should().Be("Hello World");
    }

    [Fact]
    public void ToString_NullText_ReturnsEmpty()
    {
        var body = new MessageBody();

        body.ToString().Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversionFromString_CreatesHtmlBody()
    {
        MessageBody body = "<p>content</p>";

        body.BodyType.Should().Be(BodyType.HTML);
        body.Text.Should().Be("<p>content</p>");
    }

    [Fact]
    public void ImplicitConversionToString_ReturnsText()
    {
        var body = new MessageBody(BodyType.Text, "extract me");

        string text = body;

        text.Should().Be("extract me");
    }

    [Fact]
    public void Properties_CanBeChanged()
    {
        var body = new MessageBody
        {
            BodyType = BodyType.HTML,
            Text = "<html/>"
        };

        body.BodyType.Should().Be(BodyType.HTML);
        body.Text.Should().Be("<html/>");
    }
}
