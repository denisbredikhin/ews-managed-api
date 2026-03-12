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

using System;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class StringListTests
{
    [Fact]
    public void DefaultConstructor_IsEmpty()
    {
        var list = new StringList();

        list.Count.Should().Be(0);
    }

    [Fact]
    public void EnumerableConstructor_PopulatesWithStrings()
    {
        var list = new StringList(["a", "b", "c"]);

        list.Count.Should().Be(3);
        list.Contains("a").Should().BeTrue();
        list.Contains("b").Should().BeTrue();
        list.Contains("c").Should().BeTrue();
    }

    [Fact]
    public void Add_IncreasesCount()
    {
        var list = new StringList
        {
            "item"
        };

        list.Count.Should().Be(1);
    }

    [Fact]
    public void Add_ItemIsRetrievable()
    {
        var list = new StringList
        {
            "hello"
        };

        list[0].Should().Be("hello");
    }

    [Fact]
    public void AddRange_AddsOnlyUniqueItems()
    {
        var list = new StringList(["a"]);
        list.AddRange(["a", "b", "c"]);

        list.Count.Should().Be(3);
        list.Contains("a").Should().BeTrue();
        list.Contains("b").Should().BeTrue();
        list.Contains("c").Should().BeTrue();
    }

    [Fact]
    public void AddRange_AllDuplicates_DoesNotChangeList()
    {
        var list = new StringList(["a", "b"]);
        list.AddRange(["a", "b"]);

        list.Count.Should().Be(2);
    }

    [Fact]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var list = new StringList(["a", "b"]);

        bool result = list.Remove("a");

        result.Should().BeTrue();
        list.Count.Should().Be(1);
        list.Contains("a").Should().BeFalse();
    }

    [Fact]
    public void Remove_NonExistentItem_ReturnsFalse()
    {
        var list = new StringList(["a"]);

        bool result = list.Remove("missing");

        result.Should().BeFalse();
        list.Count.Should().Be(1);
    }

    [Fact]
    public void RemoveAt_ValidIndex_Removes()
    {
        var list = new StringList(["a", "b", "c"]);

        list.RemoveAt(1);

        list.Count.Should().Be(2);
        list.Contains("b").Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void RemoveAt_OutOfRange_Throws(int index)
    {
        var list = new StringList(["a", "b", "c"]);

        Action act = () => list.RemoveAt(index);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Clear_EmptiesList()
    {
        var list = new StringList(["a", "b"]);

        list.Clear();

        list.Count.Should().Be(0);
    }

    [Fact]
    public void Indexer_Get_ReturnsCorrectItem()
    {
        var list = new StringList(["x", "y", "z"]);

        list[1].Should().Be("y");
    }

    [Fact]
    public void Indexer_Set_UpdatesItem()
    {
        var list = new StringList(["a", "b"])
        {
            [0] = "updated"
        };

        list[0].Should().Be("updated");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Indexer_Get_OutOfRange_Throws(int index)
    {
        var list = new StringList(["a"]);

        Action act = () => _ = list[index];

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToString_ReturnsCommaSeparated()
    {
        var list = new StringList(["a", "b", "c"]);

        list.ToString().Should().Be("a,b,c");
    }

    [Fact]
    public void ToString_EmptyList_ReturnsEmpty()
    {
        var list = new StringList();

        list.ToString().Should().BeEmpty();
    }

    [Fact]
    public void Equals_SameContent_ReturnsTrue()
    {
        var list1 = new StringList(["a", "b"]);
        var list2 = new StringList(["a", "b"]);

        list1.Equals(list2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentContent_ReturnsFalse()
    {
        var list1 = new StringList(["a", "b"]);
        var list2 = new StringList(["a", "c"]);

        list1.Equals(list2).Should().BeFalse();
    }

    [Fact]
    public void Equals_NonStringListObject_ReturnsFalse()
    {
        var list = new StringList(["a"]);

        list.Equals("not_a_stringlist").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_EqualLists_SameHashCode()
    {
        var list1 = new StringList(["x", "y"]);
        var list2 = new StringList(["x", "y"]);

        list1.GetHashCode().Should().Be(list2.GetHashCode());
    }

    [Fact]
    public void GetEnumerator_IteratesAllItems()
    {
        var list = new StringList(["one", "two", "three"]);
        var collected = new List<string>();

        foreach (var item in list)
            collected.Add(item);

        collected.Should().Equal("one", "two", "three");
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new StringList(["alpha"]);

        list.Contains("alpha").Should().BeTrue();
    }

    [Fact]
    public void Contains_NonExistentItem_ReturnsFalse()
    {
        var list = new StringList(["alpha"]);

        list.Contains("beta").Should().BeFalse();
    }
}
