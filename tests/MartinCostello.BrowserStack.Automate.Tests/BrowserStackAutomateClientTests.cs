// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Polly.CircuitBreaker;
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
            browsers.Should().NotBeNullOrEmpty();
            browsers.Should().NotContainNulls();

            browsers.All((p) => !string.IsNullOrEmpty(p.BrowserVersion));
            browsers.All((p) => !string.IsNullOrEmpty(p.Device));

            foreach (var browser in browsers)
            {
                browser.Should().NotBeNull();
                browser.BrowserName.Should().NotBeNullOrEmpty();
                browser.BrowserName.Should().NotBeNullOrEmpty();
                browser.OSName.Should().NotBeNullOrEmpty();
                browser.OSVersion.Should().NotBeNullOrEmpty();
            }

            // Act
            ICollection<Build> builds = await target.GetBuildsAsync();

            // Assert
            builds.Should().NotBeNull();
            builds.Should().NotContainNulls();

            foreach (var build in builds)
            {
                build.Should().NotBeNull();
                build.Duration.Should().BeGreaterOrEqualTo(1);
                build.HashedId.Should().NotBeNullOrEmpty();
                build.Name.Should().NotBeNullOrEmpty();
                build.Status.Should().NotBeNullOrEmpty();
            }

            // Arrange
            foreach (var build in builds)
            {
                // Act
                ICollection<Session> sessions = await target.GetSessionsAsync(build.HashedId);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Should().NotContainNulls();

                sessions.All((p) => !string.IsNullOrEmpty(p.BrowserName));
                sessions.All((p) => !string.IsNullOrEmpty(p.BrowserVersion));

                // Limit the sessions for performance
                foreach (var session in sessions.Take(1))
                {
                    AssertSession(session, build.Name);

                    // Act
                    var sessionDetail = await target.GetSessionAsync(session.HashedId);

                    // Assert
                    sessionDetail.Should().NotBeNull();
                    sessionDetail.BrowserUri.Should().NotBeNull();
                    sessionDetail.PublicUri.Should().NotBeNull();
                    sessionDetail.VideoUri.Should().NotBeNull();
                    sessionDetail.BrowserName.Should().Be(session.BrowserName);
                    sessionDetail.BrowserVersion.Should().Be(session.BrowserVersion);
                    sessionDetail.BuildName.Should().Be(session.BuildName);
                    sessionDetail.Duration.Should().Be(session.Duration);
                    sessionDetail.HashedId.Should().Be(session.HashedId);
                    sessionDetail.LogsUri.Should().Be(session.LogsUri);
                    sessionDetail.Name.Should().Be(session.Name);
                    sessionDetail.OSName.Should().Be(session.OSName);
                    sessionDetail.OSVersion.Should().Be(session.OSVersion);
                    sessionDetail.ProjectName.Should().Be(session.ProjectName);
                    sessionDetail.Reason.Should().Be(session.Reason);
                    sessionDetail.Status.Should().Be(session.Status);

                    // Act
                    Session updatedSession = await target.SetSessionCompletedAsync(session.HashedId, string.Empty);

                    // Assert
                    updatedSession.Should().NotBeNull();
                    updatedSession.HashedId.Should().Be(session.HashedId);
                }

                // Arrange
                limit = 5;
                status = null;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Should().NotContainNulls();
                sessions.Count.Should().BeLessOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);
                }

                // Arrange
                limit = null;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Should().NotContainNulls();

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);
                }

                // Arrange
                limit = 5;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Should().NotContainNulls();
                sessions.Count.Should().BeLessOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Name);

                    // Act (no Assert)
                    await target.GetSessionLogsAsync(build.HashedId, session.HashedId);
                }
            }

            // Arrange
            limit = 5;
            status = null;

            // Act
            builds = await target.GetBuildsAsync(limit, status);

            // Assert
            builds.Should().NotBeNull();
            builds.Should().NotContainNulls();
            builds.Count.Should().BeLessOrEqualTo(limit.Value);

            foreach (var build in builds)
            {
                AssertBuild(build);
            }

            // Arrange
            limit = null;
            status = BuildStatuses.Done;

            // Act
            builds = await target.GetBuildsAsync(limit, status);

            // Assert
            builds.Should().NotBeNull();
            builds.Should().NotContainNulls();

            foreach (var build in builds)
            {
                AssertBuild(build, status);
            }

            // Act
            ICollection<Project> projects = await target.GetProjectsAsync();

            // Assert
            projects.Should().NotBeNullOrEmpty();
            projects.Should().NotContainNulls();

            DateTime minimumDate = new DateTime(2011, 1, 1, 0, 0, 0);

            foreach (var project in projects)
            {
                project.Should().NotBeNull();
                project.GroupId.Should().BeGreaterThan(0);
                project.Id.Should().BeGreaterThan(0);
                project.CreatedAt.Should().BeAfter(minimumDate);
                project.UpdatedAt.Should().BeAfter(minimumDate);
                project.Name.Should().NotBeNullOrEmpty();

                if (project.UserId.HasValue)
                {
                    project.UserId.Should().BeGreaterThan(0);
                }
            }

            // Arrange
            foreach (int projectId in projects.Select((p) => p.Id))
            {
                // Act
                ProjectDetailItem projectItem = await target.GetProjectAsync(projectId);

                // Assert
                projectItem.Should().NotBeNull();
                projectItem.Project.Should().NotBeNull();

                var project = projectItem.Project;
                project.GroupId.Should().BeGreaterThan(0);
                project.Id.Should().BeGreaterThan(0);
                project.CreatedAt.Should().BeAfter(minimumDate);
                project.UpdatedAt.Should().BeAfter(minimumDate);
                project.Name.Should().NotBeNullOrEmpty();
                project.Builds.Should().NotBeNull();
                project.Builds.Should().NotContainNulls();

                foreach (var build in project.Builds)
                {
                    build.Should().NotBeNull();
                    build.Should().NotBeNull();
                    build.CreatedAt.Should().BeOnOrAfter(project.CreatedAt);
                    build.Duration.Should().BeGreaterOrEqualTo(1);
                    build.Id.Should().BeGreaterThan(0);
                    build.HashedId.Should().NotBeNullOrEmpty();
                    build.Name.Should().NotBeNullOrEmpty();
                    build.Status.Should().NotBeNullOrEmpty();
                    build.UpdatedAt.Should().BeAfter(project.CreatedAt);
                    build.UpdatedAt.Should().BeAfter(build.CreatedAt);
                    build.GroupId.Should().Be(project.GroupId);
                    build.ProjectId.Should().Be(project.Id);
                    build.UserId.Should().BeGreaterThan(0);
                }
            }

            // Act
            AutomatePlanStatus plan = await target.GetStatusAsync();

            // Assert
            plan.Should().NotBeNull();
            plan.AutomatePlan.Should().NotBeNullOrEmpty();
            plan.MaximumAllowedParallelSessions.Should().BeGreaterOrEqualTo(1);
            plan.MaximumQueuedParallelSessions.Should().BeGreaterOrEqualTo(1);
            plan.ParallelSessionsRunning.Should().BeGreaterOrEqualTo(0);
            plan.QueuedParallelSessions.Should().BeGreaterOrEqualTo(0);
            plan.TeamMaximumAllowedParallelSessions.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public static void Constructor_Validates_Parameters()
        {
            // Act and Assert
            Constructor(() => new BrowserStackAutomateClient(null, "MyAccessKey")).Should().Throw<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient(string.Empty, "MyAccessKey")).Should().Throw<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient("          ", "MyAccessKey")).Should().Throw<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", null)).Should().Throw<ArgumentException>().And.ParamName.Should().Be("accessKey");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", string.Empty)).Should().Throw<ArgumentException>().And.ParamName.Should().Be("accessKey");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", "          ")).Should().Throw<ArgumentException>().And.ParamName.Should().Be("accessKey");
        }

        [Fact]
        public static void FromCredential_Throws_If_Credential_Is_Null()
        {
            // Arrange
            NetworkCredential credential = null;

            // Act and Assert
            Invoking(() => BrowserStackAutomateClient.FromCredential(credential))
                .Should()
                .Throw<ArgumentNullException>()
                .And
                .ParamName.Should().Be("credential");
        }

        [Fact]
        public static void DeleteBuildAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;

            // Act and Assert
            target
                .Awaiting((p) => p.DeleteBuildAsync(buildId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("buildId");
        }

        [RequiresServiceCredentialsFact]
        public static void DeleteBuildAsync_Throws_If_BuildId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string buildId = Guid.NewGuid().ToString();

            // Act and Assert
            target
                .Awaiting((p) => p.DeleteBuildAsync(buildId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [RequiresServiceCredentialsFact]
        public static void DeleteProjectAsync_Throws_If_ProjectId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            int projectId = 0;

            // Act and Assert
            target
                .Awaiting((p) => p.DeleteProjectAsync(projectId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [Fact]
        public static void DeleteSessionAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;

            // Act and Assert
            target
                .Awaiting((p) => p.DeleteSessionAsync(sessionId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("sessionId");
        }

        [RequiresServiceCredentialsFact]
        public static void DeleteSessionAsync_Throws_If_SessionId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            target
                .Awaiting((p) => p.DeleteSessionAsync(sessionId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [Fact]
        public static void GetSessionAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionAsync(sessionId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("sessionId");
        }

        [RequiresServiceCredentialsFact]
        public static void GetSessionAsync_Throws_If_SessionId_Is_Not_Found()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionAsync(sessionId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [Fact]
        public static void GetSessionLogsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;
            string sessionId = Guid.NewGuid().ToString();

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionLogsAsync(buildId, sessionId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("buildId");
        }

        [Fact]
        public static void GetSessionLogsAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = Guid.NewGuid().ToString();
            string sessionId = null;

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionLogsAsync(buildId, sessionId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("sessionId");
        }

        [Fact]
        public static void GetSessionsAsync_Throws_If_BuildId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string buildId = null;

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionsAsync(buildId))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("buildId");
        }

        [Fact]
        public static void SetSessionStatusAsync_Throws_If_SessionId_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = null;
            string status = null;
            string reason = null;

            // Act and Assert
            target
                .Awaiting((p) => p.SetSessionStatusAsync(sessionId, status, reason))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("sessionId");
        }

        [Fact]
        public static void SetSessionStatusAsync_Throws_If_Status_Is_Null()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            string status = null;
            string reason = null;

            // Act and Assert
            target
                .Awaiting((p) => p.SetSessionStatusAsync(sessionId, status, reason))
                .Should()
                .Throw<ArgumentException>()
                .And
                .ParamName.Should().Be("status");
        }

        [Fact]
        public static void GetBuildsAsync_Throws_If_Limit_Is_Less_Than_One()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            int? limit = 0;
            string status = null;

            // Act and Assert
            target
                .Awaiting((p) => p.GetBuildsAsync(limit, status))
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .Where((p) => p.ParamName == "limit")
                .Where((p) => p.Message.StartsWith("The limit value cannot be less than one.", StringComparison.Ordinal))
                .And
                .ActualValue.Should().Be(0);
        }

        [Fact]
        public static void GetSessionsAsync_Throws_If_Limit_Is_Less_Than_One()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateClient();

            string sessionId = "x";
            int? limit = 0;
            string status = null;

            // Act and Assert
            target
                .Awaiting((p) => p.GetSessionsAsync(sessionId, limit, status))
                .Should()
                .Throw<ArgumentOutOfRangeException>()
                .Where((p) => p.ParamName == "limit")
                .Where((p) => p.Message.StartsWith("The limit value cannot be less than one.", StringComparison.Ordinal))
                .And
                .ActualValue.Should().Be(0);
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Build()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string buildId = "CHANGE_ME";

            // Act
            await target.DeleteBuildAsync(buildId);

            // Assert
            target
                .Awaiting((p) => p.DeleteBuildAsync(buildId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Project()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            int projectId = 0;

            // Act
            await target.DeleteProjectAsync(projectId);

            // Assert
            target
                .Awaiting((p) => p.DeleteProjectAsync(projectId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
        }

        [RequiresServiceCredentialsFact(Skip = "Test can only be run manually to prevent accidental destruction of data.")]
        public static async Task Can_Delete_Session()
        {
            // Arrange
            BrowserStackAutomateClient target = CreateAuthenticatedClient();

            string sessionId = "CHANGE_ME";

            // Act
            await target.DeleteSessionAsync(sessionId);

            // Assert
            target
                .Awaiting((p) => p.DeleteSessionAsync(sessionId))
                .Should()
                .Throw<BrowserStackAutomateException>()
                .And
                .ErrorDetail.Should().NotBeNull();
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
            actual.Should().NotBeNull();
            actual.OldKey.Should().Be(expected);
            actual.NewKey.Should().NotBe(expected);
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

            using (var provider = services.BuildServiceProvider())
            {
                var client = provider.GetRequiredService<BrowserStackAutomateClient>();

                // Act and Assert
                await Assert.ThrowsAsync<IsolatedCircuitException>(() => client.GetBuildsAsync());
            }
        }

        /// <summary>
        /// Asserts that the specified <see cref="Build"/> is correct.
        /// </summary>
        /// <param name="build">The <see cref="Build"/> to assert on.</param>
        /// <param name="expectedStatus">The optional expected status.</param>
        private static void AssertBuild(Build build, string expectedStatus = null)
        {
            build.Should().NotBeNull();
            build.Duration.Should().BeGreaterOrEqualTo(1);
            build.HashedId.Should().NotBeNullOrEmpty();
            build.Name.Should().NotBeNullOrEmpty();
            build.Status.Should().NotBeNullOrEmpty();

            if (expectedStatus != null)
            {
                build.Status.Should().Be(expectedStatus);
            }
        }

        /// <summary>
        /// Asserts that the specified <see cref="Session"/> is correct.
        /// </summary>
        /// <param name="session">The <see cref="Session"/> to assert on.</param>
        /// <param name="expectedName">The expected name of the session item.</param>
        private static void AssertSession(Session session, string expectedName)
        {
            session.Should().NotBeNull();
            session.BuildName.Should().Be(expectedName);
            session.HashedId.Should().NotBeNullOrEmpty();
            session.LogsUri.Should().NotBeNullOrEmpty();
            session.OSName.Should().NotBeNullOrEmpty();
            session.OSVersion.Should().NotBeNullOrEmpty();
            session.ProjectName.Should().NotBeNullOrEmpty();
            session.Reason.Should().NotBeNullOrEmpty();
            session.Status.Should().NotBeNullOrEmpty();
            session.Duration.Should().BeGreaterThan(0);
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
        private static BrowserStackAutomateClient CreateClient() => new BrowserStackAutomateClient("x", "x");

        /// <summary>
        /// Helper method for asserting on constructor invocation.
        /// </summary>
        /// <typeparam name="T">The type of the object being constructed.</typeparam>
        /// <param name="func">A delegate to a method that constructs the object.</param>
        /// <returns>
        /// A delegate representing the operation to invoke the constructor.
        /// </returns>
        private static Action Constructor<T>(Func<T> func)
            where T : class
        {
            return Invoking(func);
        }

        /// <summary>
        /// Helper method for asserting on static methods.
        /// </summary>
        /// <typeparam name="T">The type of the return value of the method.</typeparam>
        /// <param name="func">A delegate to a method that invokes the method.</param>
        /// <returns>
        /// A delegate representing the operation to invoke the method.
        /// </returns>
        private static Action Invoking<T>(Func<T> func) => () => func();
    }
}
