namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// A class containing tests for the <see cref="BrowserStackAutomateClient"/> class.
    /// </summary>
    public static class BrowserStackAutomateClientTests
    {
        [RequiresServiceCredentialsFact]
        public static async Task Can_Query_BrowserStack_Automate_Api()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            // Act
            var browsers = await target.GetBrowsersAsync();

            // Assert
            Assert.NotNull(browsers);

            // Act
            var builds = await target.GetBuildsAsync();

            // Assert
            Assert.NotNull(builds);

            // Act
            var projects = await target.GetProjectsAsync();

            // Assert
            Assert.NotNull(projects);

            // Act
            var status = await target.GetStatusAsync();

            // Assert
            Assert.NotNull(status);
        }

        [Fact]
        public static void Constructor_Validates_Parameters()
        {
            // Act and Assert
            Assert.Throws<ArgumentException>("userName", () => new BrowserStackAutomateClient(null, "MyAccessKey"));
            Assert.Throws<ArgumentException>("userName", () => new BrowserStackAutomateClient(string.Empty, "MyAccessKey"));
            Assert.Throws<ArgumentException>("userName", () => new BrowserStackAutomateClient("          ", "MyAccessKey"));
            Assert.Throws<ArgumentException>("accessKey", () => new BrowserStackAutomateClient("MyUserName", null));
            Assert.Throws<ArgumentException>("accessKey", () => new BrowserStackAutomateClient("MyUserName", string.Empty));
            Assert.Throws<ArgumentException>("accessKey", () => new BrowserStackAutomateClient("MyUserName", "          "));
        }

        [Fact]
        public static async Task GetProjectAsync_Throws_If_ProjectId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string projectId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("projectId", () => target.GetProjectAsync(projectId));
        }

        [Fact]
        public static async Task GetSessionAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionAsync(sessionId));
        }

        [Fact]
        public static async Task GetSessionLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = string.Empty;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = "x";
            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionsAsync(buildId));
        }

        [Fact]
        public static async Task SetSessionStatusAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;
            string status = null;
            string reason = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.SetSessionStatusAsync(sessionId, status, reason));
        }

        [Fact]
        public static async Task SetSessionStatusAsync_Throws_If_Status_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            string status = null;
            string reason = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("status", () => target.SetSessionStatusAsync(sessionId, status, reason));
        }

        [Fact]
        public static async Task GetBuildsAsync_Throws_If_Limit_Is_Less_Than_One()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            int? limit = 0;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("limit", () => target.GetBuildsAsync(limit, status));

            Assert.Equal(0, error.ActualValue);
            Assert.StartsWith("The limit value cannot be less than one.", error.Message);
        }

        [Fact]
        public static async Task GetSessionsAsync_Throws_If_Limit_Is_Less_Than_One()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            int? limit = 0;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("limit", () => target.GetSessionsAsync(sessionId, limit, status));

            Assert.Equal(0, error.ActualValue);
            Assert.StartsWith("The limit value cannot be less than one.", error.Message);
        }

        /// <summary>
        /// Creates an authenticated instance of <see cref="BrowserStackAutomateClient"/>.
        /// </summary>
        /// <returns>
        /// The authenticated instance of <see cref="BrowserStackAutomateClient"/>.
        /// </returns>
        private static BrowserStackAutomateClient CreateAuthenticatedClient()
        {
            string userName = Environment.GetEnvironmentVariable("BrowserStack_UserName");
            string accessKey = Environment.GetEnvironmentVariable("BrowserStack_AccessKey");

            return new BrowserStackAutomateClient(userName, accessKey);
        }

        /// <summary>
        /// Creates an instance of <see cref="BrowserStackAutomateClient"/>.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="BrowserStackAutomateClient"/>.
        /// </returns>
        private static BrowserStackAutomateClient CreateClient() => new BrowserStackAutomateClient("x", "x");
    }
}
