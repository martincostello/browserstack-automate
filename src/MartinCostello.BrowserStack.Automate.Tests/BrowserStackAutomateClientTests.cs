namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            int? limit;
            string status;

            // Act
            ICollection<Browser> browsers = await target.GetBrowsersAsync();

            // Assert
            Assert.NotNull(browsers);
            Assert.NotEmpty(browsers);

            Assert.True(browsers.Any((p) => !string.IsNullOrEmpty(p.BrowserVersion)));
            Assert.True(browsers.Any((p) => !string.IsNullOrEmpty(p.Device)));

            foreach (var browser in browsers)
            {
                Assert.NotNull(browser);
                Assert.False(string.IsNullOrEmpty(browser.BrowserName));
                Assert.False(string.IsNullOrEmpty(browser.OSName));
                Assert.False(string.IsNullOrEmpty(browser.OSVersion));
            }

            // Act
            ICollection<BuildItem> builds = await target.GetBuildsAsync();

            // Assert
            Assert.NotNull(builds);
            Assert.NotEmpty(builds);

            foreach (var build in builds)
            {
                Assert.NotNull(build);
                Assert.NotNull(build.Item);
                Assert.True(build.Item.Duration >= 1);
                Assert.False(string.IsNullOrEmpty(build.Item.HashedId));
                Assert.False(string.IsNullOrEmpty(build.Item.Name));
                Assert.False(string.IsNullOrEmpty(build.Item.Status));
            }

            // Arrange
            foreach (var build in builds)
            {
                // Act
                ICollection<SessionItem> sessions = await target.GetSessionsAsync(build.Item.HashedId);

                // Assert
                Assert.NotNull(sessions);
                Assert.NotEmpty(sessions);

                // Limit the sessions for performance
                foreach (var session in sessions.Take(1))
                {
                    Assert.NotNull(session);
                    Assert.NotNull(session.Item);
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserName));
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserVersion));
                    Assert.Equal(build.Item.Name, session.Item.BuildName);
                    Assert.False(string.IsNullOrEmpty(session.Item.HashedId));
                    Assert.False(string.IsNullOrEmpty(session.Item.LogsUri));
                    Assert.False(string.IsNullOrEmpty(session.Item.Name));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSName));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSVersion));
                    Assert.False(string.IsNullOrEmpty(session.Item.ProjectName));
                    Assert.False(string.IsNullOrEmpty(session.Item.Reason));
                    Assert.False(string.IsNullOrEmpty(session.Item.Status));
                    Assert.True(session.Item.Duration > 0);

                    // Act
                    var sessionDetail = await target.GetSessionAsync(session.Item.HashedId);

                    // Assert
                    Assert.NotNull(sessionDetail);
                    Assert.NotNull(sessionDetail.Item);
                    Assert.NotNull(sessionDetail.Item.BrowserUri);
                    Assert.NotNull(sessionDetail.Item.VideoUri);
                    Assert.Equal(session.Item.BrowserName, sessionDetail.Item.BrowserName);
                    Assert.Equal(session.Item.BrowserVersion, sessionDetail.Item.BrowserVersion);
                    Assert.Equal(session.Item.BuildName, sessionDetail.Item.BuildName);
                    Assert.Equal(session.Item.Duration, sessionDetail.Item.Duration);
                    Assert.Equal(session.Item.HashedId, sessionDetail.Item.HashedId);
                    Assert.Equal(session.Item.LogsUri, sessionDetail.Item.LogsUri);
                    Assert.Equal(session.Item.Name, sessionDetail.Item.Name);
                    Assert.Equal(session.Item.OSName, sessionDetail.Item.OSName);
                    Assert.Equal(session.Item.OSVersion, sessionDetail.Item.OSVersion);
                    Assert.Equal(session.Item.ProjectName, sessionDetail.Item.ProjectName);
                    Assert.Equal(session.Item.Reason, sessionDetail.Item.Reason);
                    Assert.Equal(session.Item.Status, sessionDetail.Item.Status);
                }

                // Arrange
                limit = 5;
                status = null;

                // Act
                sessions = await target.GetSessionsAsync(build.Item.HashedId, limit, status);

                // Assert
                Assert.NotNull(sessions);
                Assert.True(sessions.Count <= limit);

                foreach (var session in sessions)
                {
                    Assert.NotNull(session);
                    Assert.NotNull(session.Item);
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserName));
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserVersion));
                    Assert.Equal(build.Item.Name, session.Item.BuildName);
                    Assert.False(string.IsNullOrEmpty(session.Item.HashedId));
                    Assert.False(string.IsNullOrEmpty(session.Item.LogsUri));
                    Assert.False(string.IsNullOrEmpty(session.Item.Name));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSName));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSVersion));
                    Assert.False(string.IsNullOrEmpty(session.Item.ProjectName));
                    Assert.False(string.IsNullOrEmpty(session.Item.Reason));
                    Assert.False(string.IsNullOrEmpty(session.Item.Status));
                    Assert.True(session.Item.Duration > 0);
                }

                // Arrange
                limit = null;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.Item.HashedId, limit, status);

                // Assert
                Assert.NotNull(sessions);

                foreach (var session in sessions)
                {
                    Assert.NotNull(session);
                    Assert.NotNull(session.Item);
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserName));
                    Assert.False(string.IsNullOrEmpty(session.Item.BrowserVersion));
                    Assert.Equal(build.Item.Name, session.Item.BuildName);
                    Assert.False(string.IsNullOrEmpty(session.Item.HashedId));
                    Assert.False(string.IsNullOrEmpty(session.Item.LogsUri));
                    Assert.False(string.IsNullOrEmpty(session.Item.Name));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSName));
                    Assert.False(string.IsNullOrEmpty(session.Item.OSVersion));
                    Assert.False(string.IsNullOrEmpty(session.Item.ProjectName));
                    Assert.False(string.IsNullOrEmpty(session.Item.Reason));
                    Assert.Equal(status, session.Item.Status);
                    Assert.True(session.Item.Duration > 0);
                }
            }

            // Arrange
            limit = 5;
            status = null;

            // Act
            builds = await target.GetBuildsAsync(limit, status);

            // Assert
            Assert.NotNull(builds);
            Assert.True(builds.Count <= limit.Value);

            foreach (var build in builds)
            {
                Assert.NotNull(build);
                Assert.NotNull(build.Item);
                Assert.True(build.Item.Duration >= 1);
                Assert.False(string.IsNullOrEmpty(build.Item.HashedId));
                Assert.False(string.IsNullOrEmpty(build.Item.Name));
                Assert.False(string.IsNullOrEmpty(build.Item.Status));
            }

            // Arrange
            limit = null;
            status = BuildStatuses.Done;

            // Act
            builds = await target.GetBuildsAsync(limit, status);

            // Assert
            Assert.NotNull(builds);

            foreach (var build in builds)
            {
                Assert.NotNull(build);
                Assert.NotNull(build.Item);
                Assert.True(build.Item.Duration >= 1);
                Assert.False(string.IsNullOrEmpty(build.Item.HashedId));
                Assert.False(string.IsNullOrEmpty(build.Item.Name));
                Assert.Equal(status, build.Item.Status);
            }

            // Act
            ICollection<ProjectItem> projects = await target.GetProjectsAsync();

            // Assert
            Assert.NotNull(projects);
            Assert.NotEmpty(projects);

            DateTime minimumDate = new DateTime(2011, 1, 1, 0, 0, 0);

            foreach (var project in projects)
            {
                Assert.NotNull(project);
                Assert.NotNull(project.Item);
                Assert.True(project.Item.Id > 0);
                Assert.True(project.Item.GroupId > 0);
                Assert.True(project.Item.CreatedAt > minimumDate);
                Assert.True(project.Item.UpdatedAt > minimumDate);
                Assert.False(string.IsNullOrEmpty(project.Item.Name));
            }

            // Arrange
            foreach (int projectId in projects.Select((p) => p.Item.Id))
            {
                // Act
                ProjectDetailItem project = await target.GetProjectAsync(projectId);

                // Assert
                Assert.NotNull(project);
                Assert.NotNull(project.Item);
                Assert.True(project.Item.Id > 0);
                Assert.True(project.Item.GroupId > 0);
                Assert.True(project.Item.CreatedAt > minimumDate);
                Assert.True(project.Item.UpdatedAt > minimumDate);
                Assert.False(string.IsNullOrEmpty(project.Item.Name));

                Assert.NotNull(project.Item.Builds);
                Assert.NotEmpty(project.Item.Builds);

                foreach (var build in project.Item.Builds)
                {
                    Assert.NotNull(build);
                    Assert.NotNull(build.Item);

                    Assert.True(project.Item.CreatedAt >= project.Item.CreatedAt);
                    Assert.True(build.Item.Duration >= 1);
                    Assert.True(build.Item.Id > 0);
                    Assert.False(string.IsNullOrEmpty(build.Item.HashedId));
                    Assert.False(string.IsNullOrEmpty(build.Item.Name));
                    Assert.False(string.IsNullOrEmpty(build.Item.Status));
                    Assert.True(project.Item.UpdatedAt > project.Item.CreatedAt);
                    Assert.Equal(project.Item.GroupId, build.Item.GroupId);
                    Assert.Equal(project.Item.Id, build.Item.ProjectId);
                    Assert.True(build.Item.UserId > 0);
                }
            }

            // Act
            AutomatePlanStatus plan = await target.GetStatusAsync();

            // Assert
            Assert.NotNull(status);
            Assert.False(string.IsNullOrEmpty(plan.AutomatePlan));
            Assert.True(plan.MaximumAllowedParallelSessions >= 1);
            Assert.True(plan.ParallelSessionsRunning >= 0);
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
        public static async Task GetSessionAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionAsync(sessionId));
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

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually so that the API key can be updated.")]
        public static async Task Can_Recycle_BrowserStack_Api_Key()
        {
            // Arrange
            string expected = Environment.GetEnvironmentVariable("BrowserStack_AccessKey");
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            // Act
            RecycleAccessKeyResult actual = await target.RecycleAccessKeyAsync();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual.OldKey);
            Assert.NotEqual(expected, actual.NewKey);
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
