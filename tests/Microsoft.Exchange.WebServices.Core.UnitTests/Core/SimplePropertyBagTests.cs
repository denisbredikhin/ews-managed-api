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

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class SimplePropertyBagTests
{
    private readonly SimplePropertyBag<string> _sut = new();

    [Fact]
    public void Indexer_SetNewKey_AddsToAddedItems()
    {
        _sut["key1"] = "value1";

        _sut.AddedItems.Should().Contain("key1");
        _sut.ModifiedItems.Should().BeEmpty();
        _sut.RemovedItems.Should().BeEmpty();
    }

    [Fact]
    public void Indexer_UpdateExistingKey_AddsToModifiedItems()
    {
        _sut["key1"] = "value1";
        _sut.ClearChangeLog();

        _sut["key1"] = "value2";

        _sut.ModifiedItems.Should().Contain("key1");
        _sut.AddedItems.Should().BeEmpty();
    }

    [Fact]
    public void Indexer_SetNull_RemovesKey()
    {
        _sut["key1"] = "value1";
        _sut.ClearChangeLog();

        _sut["key1"] = null;

        _sut.RemovedItems.Should().Contain("key1");
        _sut.ContainsKey("key1").Should().BeFalse();
    }

    [Fact]
    public void Indexer_SetNullOnNonExistentKey_DoesNothing()
    {
        _sut["missing"] = null;

        _sut.RemovedItems.Should().BeEmpty();
        _sut.AddedItems.Should().BeEmpty();
    }

    [Fact]
    public void Indexer_GetMissingKey_ReturnsNull()
    {
        var value = _sut["nonexistent"];

        value.Should().BeNull();
    }

    [Fact]
    public void Indexer_GetExistingKey_ReturnsValue()
    {
        _sut["key1"] = "hello";

        _sut["key1"].Should().Be("hello");
    }

    [Fact]
    public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
    {
        _sut["key1"] = 42;

        bool result = _sut.TryGetValue("key1", out object? value);

        result.Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        bool result = _sut.TryGetValue("missing", out object? value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void ContainsKey_AfterSet_ReturnsTrue()
    {
        _sut["k"] = "v";

        _sut.ContainsKey("k").Should().BeTrue();
    }

    [Fact]
    public void ContainsKey_NotSet_ReturnsFalse()
    {
        _sut.ContainsKey("k").Should().BeFalse();
    }

    [Fact]
    public void ClearChangeLog_ClearsAllChangeLists()
    {
        _sut["a"] = "1";
        _sut["b"] = "2";
        _sut["b"] = "3";

        _sut.ClearChangeLog();

        _sut.AddedItems.Should().BeEmpty();
        _sut.ModifiedItems.Should().BeEmpty();
        _sut.RemovedItems.Should().BeEmpty();
    }

    [Fact]
    public void OnChange_FiredWhenValueSet()
    {
        int changeCount = 0;
        _sut.OnChange += () => changeCount++;

        _sut["x"] = "y";

        changeCount.Should().Be(1);
    }

    [Fact]
    public void OnChange_FiredWhenValueRemoved()
    {
        int changeCount = 0;
        _sut["x"] = "y";
        _sut.OnChange += () => changeCount++;

        _sut["x"] = null;

        changeCount.Should().Be(1);
    }

    [Fact]
    public void Indexer_DeleteThenReassign_TracksAsModified()
    {
        _sut["key1"] = "original";
        _sut.ClearChangeLog();
        _sut["key1"] = null;  // marks for removal
        _sut["key1"] = "new"; // reassign should flip to modified

        _sut.RemovedItems.Should().NotContain("key1");
        _sut.ModifiedItems.Should().Contain("key1");
    }

    [Fact]
    public void GetEnumerator_IteratesAllStoredItems()
    {
        _sut["a"] = 1;
        _sut["b"] = 2;

        var pairs = _sut.ToList();

        pairs.Should().HaveCount(2);
        pairs.Should().Contain(p => p.Key == "a" && (int)p.Value! == 1);
        pairs.Should().Contain(p => p.Key == "b" && (int)p.Value! == 2);
    }

    [Fact]
    public void ModifiedItems_DuplicateModification_NotAddedTwice()
    {
        _sut["k"] = "v1";
        _sut.ClearChangeLog();
        _sut["k"] = "v2";
        _sut["k"] = "v3";

        _sut.ModifiedItems.Should().ContainSingle(x => x == "k");
    }
}
