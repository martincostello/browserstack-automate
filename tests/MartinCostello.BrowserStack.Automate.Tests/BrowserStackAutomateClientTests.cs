// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Shouldly;
using Xunit;

namespace MartinCostello.BrowserStack.Automate
{
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
            int? offset;
            string status;

            // Act
            ICollection<Browser> browsers = await target.GetBrowsersAsync();

            // Assert
            browsers.ShouldNotBeNull();
            browsers.ShouldNotContain((p) => p == null);

            foreach (var browser in browsers)
            {
                browser.ShouldNotBeNull();
                browser.BrowserName.ShouldNotBeNullOrEmpty();
                browser.OSName.ShouldNotBeNullOrEmpty();
                browser.OSVersion.ShouldNotBeNullOrEmpty();
            }

            // Act
            ICollection<Build> builds = await target.GetBuildsAsync();

            // Assert
            builds.ShouldNotBeNull();
            builds.ShouldNotContain((p) => p == null);

            foreach (var build in builds)
            {
                build.ShouldNotBeNull();
                build.Duration.ShouldBeGreaterThanOrEqualTo(1);
                build.HashedId.ShouldNotBeNullOrEmpty();
                build.Name.ShouldNotBeNullOrEmpty();
                build.Status.ShouldNotBeNullOrEmpty();
            }

            // Arrange
            foreach (var build in builds)
            {
                // Act
                ICollection<Session> sessions = await target.GetSessionsAsync(build.HashedId);

                // Assert
                sessions.ShouldNotBeNull();
                sessions.ShouldNotContain((p) => p == null);

                // Limit the sessions for performance
                foreach (var session in sessions.Take(1))
                {
                    AssertSession(session, build.Name);

                    // Act
                    var sessionDetail = await target.GetSessionAsync(session.HashedId);

                    // Assert
                    sessionDetail.ShouldNotBeNull();
                    sessionDetail.BrowserUri.ShouldNotBeNull();
                    sessionDetail.PublicUri.ShouldNotBeNull();
                    sessionDetail.VideoUri.ShouldNotBeNull();
                    sessionDetail.BrowserName.ShouldNotBeNullOrEmpty();
                    sessionDetail.BrowserName.ShouldBe(session.BrowserName);
                    sessionDetail.BrowserVersion.ShouldNotBeNullOrEmpty();
                    sessionDetail.BrowserVersion.ShouldBe(session.BrowserVersion);
                    sessionDetail.BuildName.ShouldBe(session.BuildName);
                    sessionDetail.Duration.ShouldBe(session.Duration);
                    sessionDetail.HashedId.ShouldBe(session.HashedId);
                    sessionDetail.LogsUri.ShouldBe(session.LogsUri);
                    sessionDetail.Name.ShouldBe(session.Name);
                    sessionDetail.OSName.ShouldBe(session.OSName);
                    sessionDetail.OSVersion.ShouldBe(session.OSVersion);
                    sessionDetail.ProjectName.ShouldBe(session.ProjectName);
                    sessionDetail.Reason.ShouldBe(session.Reason);
                    sessionDetail.Status.ShouldBe(session.Status);

                    // Act
                    Session updatedSession = await target.SetSessionCompletedAsync(session.HashedId, string.Empty);

                    // Assert
                    updatedSession.ShouldNotBeNull();
                    updatedSession.HashedId.ShouldBe(session.HashedId);
                }

                // Arrange
                limit = 5;
                offset = 1;
                status = null;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, offset, status);

                // Assert
                sessions.ShouldNotBeNull();
                sessions.ShouldNotContain((p) => p == null);
                sessions.Count.ShouldBeLessThanOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);
                }

                // Arrange
                limit = null;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, offset, status);

                // Assert
                sessions.ShouldNotBeNull();
                sessions.ShouldNotContain((p) => p == null);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);
                }

                // Arrange
                limit = 5;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, offset, status);

                // Assert
                sessions.ShouldNotBeNull();
                sessions.ShouldNotContain((p) => p == null);
                sessions.Count.ShouldBeLessThanOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);

                    // Act (no Assert)
                    await target.GetSessionLogsAsync(build.HashedId, session.HashedId);

                    // Act (no Assert)
                    await target.GetSessionAppiumLogsAsync(build.HashedId, session.HashedId);

                    // Act (no Assert)
                    await target.GetSessionConsoleLogsAsync(build.HashedId, session.HashedId);

                    // Act (no Assert)
                    await target.GetSessionNetworkLogsAsync(build.HashedId, session.HashedId);
                }
            }

            // Arrange
            limit = 5;
            offset = 1;
            status = null;

            // Act
            builds = await target.GetBuildsAsync(limit, offset, status);

            // Assert
            builds.ShouldNotBeNull();
            builds.ShouldNotContain((p) => p == null);
            builds.Count.ShouldBeLessThanOrEqualTo(limit.Value);

            foreach (var build in builds)
            {
                AssertBuild(build);
            }

            // Arrange
            limit = null;
            offset = null;
            status = BuildStatuses.Done;

            // Act
            builds = await target.GetBuildsAsync(limit, offset, status);

            // Assert
            builds.ShouldNotBeNull();
            builds.ShouldNotContain((p) => p == null);

            foreach (var build in builds)
            {
                AssertBuild(build, status);
            }

            // Act
            ICollection<Project> projects = await target.GetProjectsAsync();

            // Assert
            projects.ShouldNotContain((p) => p == null);

            var minimumDate = new DateTime(2011, 1, 1, 0, 0, 0);

            foreach (var project in projects)
            {
                project.ShouldNotBeNull();
                project.GroupId.ShouldBeGreaterThan(0);
                project.Id.ShouldBeGreaterThan(0);
                project.CreatedAt.ShouldBeGreaterThan(minimumDate);
                project.UpdatedAt.ShouldBeGreaterThan(minimumDate);
                project.Name.ShouldNotBeNullOrEmpty();

                if (project.UserId.HasValue)
                {
                    project.UserId.Value.ShouldBeGreaterThan(0);
                }
            }

            // Arrange
            foreach (int projectId in projects.Select((p) => p.Id))
            {
                // Act
                ProjectDetailItem projectItem = await target.GetProjectAsync(projectId);

                // Assert
                projectItem.ShouldNotBeNull();
                projectItem.Project.ShouldNotBeNull();

                var project = projectItem.Project;
                project.GroupId.ShouldBeGreaterThan(0);
                project.Id.ShouldBeGreaterThan(0);
                project.CreatedAt.ShouldBeGreaterThan(minimumDate);
                project.UpdatedAt.ShouldBeGreaterThan(minimumDate);
                project.Name.ShouldNotBeNullOrEmpty();
                project.Builds.ShouldNotBeNull();
                project.Builds.ShouldNotContain((p) => p == null);

                foreach (var build in project.Builds)
                {
                    build.ShouldNotBeNull();
                    build.ShouldNotBeNull();
                    build.CreatedAt.ShouldBeGreaterThanOrEqualTo(project.CreatedAt);
                    build.Duration.ShouldBeGreaterThanOrEqualTo(0);
                    build.Id.ShouldBeGreaterThan(0);
                    build.HashedId.ShouldNotBeNullOrEmpty();
                    build.Name.ShouldNotBeNullOrEmpty();
                    build.Status.ShouldNotBeNullOrEmpty();
                    build.UpdatedAt.ShouldBeGreaterThan(project.CreatedAt);
                    build.UpdatedAt.ShouldBeGreaterThan(build.CreatedAt);
                    build.GroupId.ShouldBe(project.GroupId);
                    build.ProjectId.ShouldBe(project.Id);
                    build.UserId.ShouldBeGreaterThan(0);
                }
            }

            // Act
            AutomatePlanStatus plan = await target.GetStatusAsync();

            // Assert
            plan.ShouldNotBeNull();
            plan.AutomatePlan.ShouldNotBeNullOrEmpty();
            plan.MaximumAllowedParallelSessions.ShouldBeGreaterThanOrEqualTo(1);
            plan.MaximumQueuedParallelSessions.ShouldBeGreaterThanOrEqualTo(1);
            plan.ParallelSessionsRunning.ShouldBeGreaterThanOrEqualTo(0);
            plan.QueuedParallelSessions.ShouldBeGreaterThanOrEqualTo(0);
            plan.TeamMaximumAllowedParallelSessions.ShouldBeGreaterThanOrEqualTo(0);
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
        public static void FromCredential_Throws_If_Credential_Is_Null()
        {
            // Arrange
            NetworkCredential credential = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("credential", () => BrowserStackAutomateClient.FromCredential(credential));
        }

        [Fact]
        public static async Task DeleteBuildAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.DeleteBuildAsync(buildId));
        }

        [Fact]
        public static async Task DeleteBuildsAsync_Throws_If_BuildIds_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            ICollection<string> buildIds = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>("buildIds", () => target.DeleteBuildsAsync(buildIds));
        }

        [Fact]
        public static async Task DeleteBuildsAsync_Throws_If_BuildIds_Is_Empty()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            var buildIds = Array.Empty<string>();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildIds", () => target.DeleteBuildsAsync(buildIds));
        }

        [RequiresServiceCredentialsFact]
        public static async Task DeleteBuildAsync_Throws_If_BuildId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string buildId = Guid.NewGuid().ToString();

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteBuildAsync(buildId));
            error.ErrorDetail.ShouldNotBeNull();
        }

        [RequiresServiceCredentialsFact]
        public static async Task DeleteProjectAsync_Throws_If_ProjectId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            int projectId = 0;

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteProjectAsync(projectId));
            error.ErrorDetail.ShouldNotBeNull();
        }

        [Fact]
        public static async Task DeleteSessionAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.DeleteSessionAsync(sessionId));
        }

        [Fact]
        public static async Task DeleteSessionsAsync_Throws_If_SessionIds_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            ICollection<string> sessionIds = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>("sessionIds", () => target.DeleteSessionsAsync(sessionIds));
        }

        [Fact]
        public static async Task DeleteSessionsAsync_Throws_If_SessionIds_Is_Empty()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            var sessionIds = Array.Empty<string>();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionIds", () => target.DeleteSessionsAsync(sessionIds));
        }

        [RequiresServiceCredentialsFact]
        public static async Task DeleteSessionAsync_Throws_If_SessionId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteSessionAsync(sessionId));
            error.ErrorDetail.ShouldNotBeNull();
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

        [RequiresServiceCredentialsFact]
        public static async Task GetSessionAsync_Throws_If_SessionId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.GetSessionAsync(sessionId));
            error.ErrorDetail.ShouldNotBeNull();
        }

        [Fact]
        public static async Task GetSessionLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = Guid.NewGuid().ToString();
            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionAppiumLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionAppiumLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionAppiumLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = Guid.NewGuid().ToString();
            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionAppiumLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionConsoleLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionConsoleLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionConsoleLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = Guid.NewGuid().ToString();
            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionConsoleLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionNetworkLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("buildId", () => target.GetSessionNetworkLogsAsync(buildId, sessionId));
        }

        [Fact]
        public static async Task GetSessionNetworkLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = Guid.NewGuid().ToString();
            string sessionId = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("sessionId", () => target.GetSessionNetworkLogsAsync(buildId, sessionId));
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
        public static async Task SetBuildNameAsync_Throws_If_Name_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            int buildId = 1;
            string name = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("name", () => target.SetBuildNameAsync(buildId, name));
        }

        [Fact]
        public static async Task SetProjectNameAsync_Throws_If_Name_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            int projectId = 1;
            string name = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("name", () => target.SetProjectNameAsync(projectId, name));
        }

        [Fact]
        public static async Task SetSessionNameAsync_Throws_If_Name_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            string name = null;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>("name", () => target.SetSessionNameAsync(sessionId, name));
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
            int? offset = 0;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("limit", () => target.GetBuildsAsync(limit, offset, status));

            error.Message.ShouldStartWith("The limit value cannot be less than one.");
            error.ActualValue.ShouldBe(0);
        }

        [Fact]
        public static async Task GetSessionsAsync_Throws_If_Limit_Is_Less_Than_One()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            int? limit = 0;
            int? offset = 0;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("limit", () => target.GetSessionsAsync(sessionId, limit, offset, status));

            error.Message.ShouldStartWith("The limit value cannot be less than one.");
            error.ActualValue.ShouldBe(0);
        }

        [Fact]
        public static async Task GetBuildsAsync_Throws_If_Offset_Is_Less_Than_Zero()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            int? limit = 1;
            int? offset = -1;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("offset", () => target.GetBuildsAsync(limit, offset, status));

            error.Message.ShouldStartWith("The offset value cannot be less than zero.");
            error.ActualValue.ShouldBe(-1);
        }

        [Fact]
        public static async Task GetSessionsAsync_Throws_If_Offset_Is_Less_Than_Zero()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            int? limit = 1;
            int? offset = -1;
            string status = null;

            // Act and Assert
            var error = await Assert.ThrowsAsync<ArgumentOutOfRangeException>("offset", () => target.GetSessionsAsync(sessionId, limit, offset, status));

            error.Message.ShouldStartWith("The offset value cannot be less than zero.");
            error.ActualValue.ShouldBe(-1);
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Build()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string buildId = "CHANGE_ME";

            // Act
            await target.DeleteBuildAsync(buildId);

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteBuildAsync(buildId));
            error.ErrorDetail.ShouldNotBeNull();
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Project()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            int projectId = 0;

            // Act
            await target.DeleteProjectAsync(projectId);

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteProjectAsync(projectId));
            error.ErrorDetail.ShouldNotBeNull();
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Session()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = "CHANGE_ME";

            // Act
            await target.DeleteSessionAsync(sessionId);

            // Act and Assert
            var error = await Assert.ThrowsAsync<BrowserStackAutomateException>(() => target.DeleteSessionAsync(sessionId));
            error.ErrorDetail.ShouldNotBeNull();
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
            actual.ShouldNotBeNull();
            actual.OldKey.ShouldBe(expected);
            actual.NewKey.ShouldNotBe(expected);
        }

        [Fact]
        public static async Task Can_Configure_With_HttpClient_Factory()
        {
            // Arrange
            var services = new ServiceCollection();

            // Create a Polly policy that is a pre-isolated circuit breaker that is guaranteed to fail
            var policy = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.MaxValue);
            policy.Isolate();

            services
                .AddPolicyRegistry()
                .Add(policy.PolicyKey, policy.AsAsyncPolicy<HttpResponseMessage>());

            services
                .AddSingleton<BrowserStackAutomateClient>()
                .AddHttpClient("BrowserStack Automate")
                .AddTypedClient((httpClient) => new BrowserStackAutomateClient("a", "b", httpClient))
                .ConfigureHttpClient((httpClient) => httpClient.Timeout = TimeSpan.FromSeconds(10))
                .AddPolicyHandlerFromRegistry(policy.PolicyKey);

            using var provider = services.BuildServiceProvider();
            var client = provider.GetRequiredService<BrowserStackAutomateClient>();

            // Act and Assert
            await Assert.ThrowsAsync<IsolatedCircuitException>(() => client.GetBuildsAsync());
        }

        /// <summary>
        /// Asserts that the specified <see cref="Build"/> is correct.
        /// </summary>
        /// <param name="build">The <see cref="Build"/> to assert on.</param>
        /// <param name="expectedStatus">The optional expected status.</param>
        private static void AssertBuild(Build build, string expectedStatus = null)
        {
            build.ShouldNotBeNull();
            build.Duration.ShouldBeGreaterThanOrEqualTo(1);
            build.HashedId.ShouldNotBeNullOrEmpty();
            build.Name.ShouldNotBeNullOrEmpty();
            build.Status.ShouldNotBeNullOrEmpty();

            if (expectedStatus != null)
            {
                build.Status.ShouldBe(expectedStatus);
            }
        }

        /// <summary>
        /// Asserts that the specified <see cref="Session"/> is correct.
        /// </summary>
        /// <param name="session">The <see cref="Session"/> to assert on.</param>
        /// <param name="expectedName">The expected name of the session item.</param>
        private static void AssertSession(Session session, string expectedName)
        {
            session.ShouldNotBeNull();
            session.BuildName.ShouldBe(expectedName);
            session.HashedId.ShouldNotBeNullOrEmpty();
            session.LogsUri.ShouldNotBeNullOrEmpty();
            session.OSName.ShouldNotBeNullOrEmpty();
            session.OSVersion.ShouldNotBeNullOrEmpty();
            session.ProjectName.ShouldNotBeNullOrEmpty();
            session.Reason.ShouldNotBeNullOrEmpty();
            session.Status.ShouldNotBeNullOrEmpty();
            session.Duration.ShouldBeGreaterThan(-1);
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

            var credential = new NetworkCredential(userName, accessKey);

            return BrowserStackAutomateClient.FromCredential(credential);
        }

        /// <summary>
        /// Creates an instance of <see cref="BrowserStackAutomateClient"/>.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="BrowserStackAutomateClient"/>.
        /// </returns>
        private static BrowserStackAutomateClient CreateClient() => new ("x", "x");
    }
}
