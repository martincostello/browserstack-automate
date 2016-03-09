// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using FluentAssertions;
    using Xunit;

    /// <summary>
    /// A class containing tests for the <see cref="BrowserStackAutomateClientExtensions"/> class.
    /// </summary>
    public static class BrowserStackAutomateClientExtensionsTests
    {
        [Fact]
        public static void SetSessionCompletedAsync_Throws_If_Client_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient client = null;

            string sessionId = "MySessionId";
            string reason = "My reason";

            // Act and Assert
            client
                .Awaiting((p) => p.SetSessionCompletedAsync(sessionId, reason))
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("client");
        }

        [Fact]
        public static void SetSessionErrorAsync_Throws_If_Client_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient client = null;

            string sessionId = "MySessionId";
            string reason = "My reason";

            // Act and Assert
            client
                .Awaiting((p) => p.SetSessionErrorAsync(sessionId, reason))
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName
                .Should()
                .Be("client");
        }
    }
}
