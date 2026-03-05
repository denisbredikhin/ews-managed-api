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

using System.Collections.Generic;
using System.Threading;
using SystemTask = System.Threading.Tasks.Task;

using AwesomeAssertions;

using Microsoft.Exchange.WebServices.Data;

using Xunit;

public class LazyMemberTests
{
    [Fact]
    public void Member_FirstAccess_InvokesDelegate()
    {
        int callCount = 0;
        var lazy = new LazyMember<string>(() =>
        {
            callCount++;
            return "hello";
        });

        _ = lazy.Member;

        callCount.Should().Be(1);
    }

    [Fact]
    public void Member_SecondAccess_DoesNotReinvokeDelegate()
    {
        int callCount = 0;
        var lazy = new LazyMember<string>(() =>
        {
            callCount++;
            return "hello";
        });

        _ = lazy.Member;
        _ = lazy.Member;

        callCount.Should().Be(1);
    }

    [Fact]
    public void Member_ReturnsValueFromDelegate()
    {
        var lazy = new LazyMember<List<int>>(() => [1, 2, 3]);

        lazy.Member.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public async SystemTask Member_ConcurrentAccess_InitializesOnce()
    {
        int callCount = 0;
        var lazy = new LazyMember<string>(() =>
        {
            Interlocked.Increment(ref callCount);
            Thread.Sleep(10);
            return "value";
        });

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => SystemTask.Run(() => lazy.Member))
            .ToArray();

        await SystemTask.WhenAll(tasks);

        callCount.Should().Be(1);
        tasks.All(t => t.Result == "value").Should().BeTrue();
    }

    [Fact]
    public void Member_NullValue_IsAllowed()
    {
        var lazy = new LazyMember<string?>(() => null);

        lazy.Member.Should().BeNull();
    }
}
