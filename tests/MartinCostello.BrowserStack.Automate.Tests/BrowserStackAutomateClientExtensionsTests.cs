// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Xunit;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class containing tests for the <see cref="BrowserStackAutomateClientExtensions"/> class.
    /// </summary>
    public static class BrowserStackAutomateClientExtensionsTests
    {
        [Fact]
        public static async Task SetSessionCompletedAsync_Throws_If_Client_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient client = null!;

            string sessionId = "MySessionId";
            string reason = "My reason";

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>("client", () => client.SetSessionCompletedAsync(sessionId, reason));
        }

        [Fact]
        public static async Task SetSessionErrorAsync_Throws_If_Client_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient client = null!;

            string sessionId = "MySessionId";
            string reason = "My reason";

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>("client", () => client.SetSessionErrorAsync(sessionId, reason));
        }
    }
}
