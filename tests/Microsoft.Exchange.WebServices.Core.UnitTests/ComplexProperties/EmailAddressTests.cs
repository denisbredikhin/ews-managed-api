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

public class EmailAddressTests
{
    [Fact]
    public void DefaultConstructor_AllPropertiesNull()
    {
        var email = new EmailAddress();

        email.Name.Should().BeNull();
        email.Address.Should().BeNull();
        email.RoutingType.Should().BeNull();
        email.MailboxType.Should().BeNull();
        email.Id.Should().BeNull();
    }

    [Fact]
    public void SmtpAddressConstructor_SetsAddress()
    {
        var email = new EmailAddress("user@example.com");

        email.Address.Should().Be("user@example.com");
        email.Name.Should().BeNull();
    }

    [Fact]
    public void NameAndAddressConstructor_SetsBothFields()
    {
        var email = new EmailAddress("John Doe", "john@example.com");

        email.Name.Should().Be("John Doe");
        email.Address.Should().Be("john@example.com");
    }

    [Fact]
    public void FullConstructor_SetsAllPublicFields()
    {
        var email = new EmailAddress("Jane", "jane@example.com", "SMTP");

        email.Name.Should().Be("Jane");
        email.Address.Should().Be("jane@example.com");
        email.RoutingType.Should().Be("SMTP");
    }

    [Fact]
    public void ToString_WithAddressOnly_ReturnsAddress()
    {
        var email = new EmailAddress("user@example.com");

        email.ToString().Should().Be("user@example.com");
    }

    [Fact]
    public void ToString_WithNameAndAddress_ReturnsFormattedString()
    {
        var email = new EmailAddress("John Doe", "john@example.com");

        email.ToString().Should().Be("John Doe <john@example.com>");
    }

    [Fact]
    public void ToString_WithRoutingType_IncludesRoutingInAddress()
    {
        var email = new EmailAddress("Jane", "jane@example.com", "SMTP");

        email.ToString().Should().Be("Jane <SMTP:jane@example.com>");
    }

    [Fact]
    public void ToString_EmptyAddress_ReturnsEmpty()
    {
        var email = new EmailAddress();

        email.ToString().Should().BeEmpty();
    }

    [Fact]
    public void ToString_AddressWithRoutingButNoName_ReturnsRoutedAddress()
    {
        var email = new EmailAddress(null!, "user@example.com", "X400");

        email.ToString().Should().Be("X400:user@example.com");
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesEmailAddress()
    {
        EmailAddress email = "convert@example.com";

        email.Address.Should().Be("convert@example.com");
        email.Name.Should().BeNull();
    }

    [Fact]
    public void SmtpRoutingType_IsCorrectConstant()
    {
        EmailAddress.SmtpRoutingType.Should().Be("SMTP");
    }

    [Fact]
    public void GetSearchString_ReturnsAddress()
    {
        ISearchStringProvider email = new EmailAddress("Display", "addr@test.com");

        email.GetSearchString().Should().Be("addr@test.com");
    }

    [Fact]
    public void Properties_CanBeSetAfterConstruction()
    {
        var email = new EmailAddress();
        email.Name = "Test";
        email.Address = "test@test.com";
        email.RoutingType = "SMTP";
        email.MailboxType = MailboxType.Mailbox;

        email.Name.Should().Be("Test");
        email.Address.Should().Be("test@test.com");
        email.RoutingType.Should().Be("SMTP");
        email.MailboxType.Should().Be(MailboxType.Mailbox);
    }
}
