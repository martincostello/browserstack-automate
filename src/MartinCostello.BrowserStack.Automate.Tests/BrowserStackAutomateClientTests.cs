// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
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
            ICollection<BuildItem> builds = await target.GetBuildsAsync();

            // Assert
            builds.Should().NotBeNullOrEmpty();

            foreach (var build in builds)
            {
                build.Should().NotBeNull();
                build.Item.Should().NotBeNull();
                build.Item.Duration.Should().BeGreaterOrEqualTo(1);
                build.Item.HashedId.Should().NotBeNullOrEmpty();
                build.Item.Name.Should().NotBeNullOrEmpty();
                build.Item.Status.Should().NotBeNullOrEmpty();
            }

            // Arrange
            foreach (var build in builds)
            {
                // Act
                ICollection<SessionItem> sessions = await target.GetSessionsAsync(build.Item.HashedId);

                // Assert
                sessions.Should().NotBeNull();

                sessions.All((p) => !string.IsNullOrEmpty(p.Item.BrowserName));
                sessions.All((p) => !string.IsNullOrEmpty(p.Item.BrowserVersion));

                // Limit the sessions for performance
                foreach (var session in sessions.Take(1))
                {
                    AssertSession(session, build.Item.Name);

                    // Act
                    var sessionDetail = await target.GetSessionAsync(session.Item.HashedId);

                    // Assert
                    sessionDetail.Should().NotBeNull();
                    sessionDetail.Item.Should().NotBeNull();
                    sessionDetail.Item.BrowserUri.Should().NotBeNull();
                    sessionDetail.Item.VideoUri.Should().NotBeNull();
                    sessionDetail.Item.BrowserName.Should().Be(session.Item.BrowserName);
                    sessionDetail.Item.BrowserVersion.Should().Be(session.Item.BrowserVersion);
                    sessionDetail.Item.BuildName.Should().Be(session.Item.BuildName);
                    sessionDetail.Item.Duration.Should().Be(session.Item.Duration);
                    sessionDetail.Item.HashedId.Should().Be(session.Item.HashedId);
                    sessionDetail.Item.LogsUri.Should().Be(session.Item.LogsUri);
                    sessionDetail.Item.Name.Should().Be(session.Item.Name);
                    sessionDetail.Item.OSName.Should().Be(session.Item.OSName);
                    sessionDetail.Item.OSVersion.Should().Be(session.Item.OSVersion);
                    sessionDetail.Item.ProjectName.Should().Be(session.Item.ProjectName);
                    sessionDetail.Item.Reason.Should().Be(session.Item.Reason);
                    sessionDetail.Item.Status.Should().Be(session.Item.Status);

                    // Act
                    SessionItem updatedSession = await target.SetSessionCompletedAsync(session.Item.HashedId, string.Empty);

                    // Assert
                    updatedSession.Should().NotBeNull();
                    updatedSession.Item.Should().NotBeNull();
                    updatedSession.Item.HashedId.Should().Be(session.Item.HashedId);
                }

                // Arrange
                limit = 5;
                status = null;

                // Act
                sessions = await target.GetSessionsAsync(build.Item.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Count.Should().BeLessOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Item.Name);
                }

                // Arrange
                limit = null;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.Item.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Item.Name);
                }

                // Arrange
                limit = 5;
                status = SessionStatuses.Done;

                // Act
                sessions = await target.GetSessionsAsync(build.Item.HashedId, limit, status);

                // Assert
                sessions.Should().NotBeNull();
                sessions.Count.Should().BeLessOrEqualTo(limit.Value);

                foreach (var session in sessions)
                {
                    AssertSession(session, build.Item.Name);
                }
            }

            // Arrange
            limit = 5;
            status = null;

            // Act
            builds = await target.GetBuildsAsync(limit, status);

            // Assert
            builds.Should().NotBeNull();
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

            foreach (var build in builds)
            {
                AssertBuild(build, status);
            }

            // Act
            ICollection<ProjectItem> projects = await target.GetProjectsAsync();

            // Assert
            projects.Should().NotBeNullOrEmpty();

            DateTime minimumDate = new DateTime(2011, 1, 1, 0, 0, 0);

            foreach (var project in projects)
            {
                project.Should().NotBeNull();
                project.Item.Should().NotBeNull();
                project.Item.GroupId.Should().BeGreaterThan(0);
                project.Item.Id.Should().BeGreaterThan(0);
                project.Item.CreatedAt.Should().BeAfter(minimumDate);
                project.Item.UpdatedAt.Should().BeAfter(minimumDate);
                project.Item.Name.Should().NotBeNullOrEmpty();

                if (project.Item.UserId.HasValue)
                {
                    project.Item.UserId.Should().BeGreaterThan(0);
                }
            }

            // Arrange
            foreach (int projectId in projects.Select((p) => p.Item.Id))
            {
                // Act
                ProjectDetailItem project = await target.GetProjectAsync(projectId);

                // Assert
                project.Should().NotBeNull();
                project.Item.Should().NotBeNull();
                project.Item.GroupId.Should().BeGreaterThan(0);
                project.Item.Id.Should().BeGreaterThan(0);
                project.Item.CreatedAt.Should().BeAfter(minimumDate);
                project.Item.UpdatedAt.Should().BeAfter(minimumDate);
                project.Item.Name.Should().NotBeNullOrEmpty();
                project.Item.Builds.Should().NotBeNull();

                foreach (var build in project.Item.Builds)
                {
                    build.Should().NotBeNull();
                    build.Item.Should().NotBeNull();
                    build.Item.CreatedAt.Should().BeOnOrAfter(project.Item.CreatedAt);
                    build.Item.Duration.Should().BeGreaterOrEqualTo(1);
                    build.Item.Id.Should().BeGreaterThan(0);
                    build.Item.HashedId.Should().NotBeNullOrEmpty();
                    build.Item.Name.Should().NotBeNullOrEmpty();
                    build.Item.Status.Should().NotBeNullOrEmpty();
                    build.Item.UpdatedAt.Should().BeAfter(project.Item.CreatedAt);
                    build.Item.UpdatedAt.Should().BeAfter(build.Item.CreatedAt);
                    build.Item.GroupId.Should().Be(project.Item.GroupId);
                    build.Item.ProjectId.Should().Be(project.Item.Id);
                    build.Item.UserId.Should().BeGreaterThan(0);
                }
            }

            // Act
            AutomatePlanStatus plan = await target.GetStatusAsync();

            // Assert
            plan.Should().NotBeNull();
            plan.AutomatePlan.Should().NotBeNullOrEmpty();
            plan.MaximumAllowedParallelSessions.Should().BeGreaterOrEqualTo(1);
            plan.ParallelSessionsRunning.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public static void Constructor_Validates_Parameters()
        {
            // Act and Assert
            Constructor(() => new BrowserStackAutomateClient(null, "MyAccessKey")).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient(string.Empty, "MyAccessKey")).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient("          ", "MyAccessKey")).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("userName");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", null)).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("accessKey");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", string.Empty)).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("accessKey");
            Constructor(() => new BrowserStackAutomateClient("MyUserName", "          ")).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("accessKey");
        }

        [Fact]
        public static void FromCredential_Throws_If_Credential_Is_Null()
        {
            // Arrange
            NetworkCredential credential = null;

            // Act and Assert
            Invoking(() => BrowserStackAutomateClient.FromCredential(credential))
                .ShouldThrow<ArgumentNullException>()
                .And
                .ParamName.Should().Be("credential");
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
                .ShouldThrow<ArgumentException>()
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
                .ShouldThrow<ArgumentException>()
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
                .ShouldThrow<ArgumentException>()
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
                .ShouldThrow<ArgumentException>()
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
                .ShouldThrow<ArgumentOutOfRangeException>()
                .Where((p) => p.ParamName == "limit")
                .Where((p) => p.Message.StartsWith("The limit value cannot be less than one."))
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
                .ShouldThrow<ArgumentOutOfRangeException>()
                .Where((p) => p.ParamName == "limit")
                .Where((p) => p.Message.StartsWith("The limit value cannot be less than one."))
                .And
                .ActualValue.Should().Be(0);
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

        /// <summary>
        /// Asserts that the specified <see cref="BuildItem"/> is correct.
        /// </summary>
        /// <param name="build">The <see cref="BuildItem"/> to assert on.</param>
        /// <param name="expectedStatus">The optional expected status.</param>
        private static void AssertBuild(BuildItem build, string expectedStatus = null)
        {
            build.Should().NotBeNull();
            build.Item.Should().NotBeNull();
            build.Item.Duration.Should().BeGreaterOrEqualTo(1);
            build.Item.HashedId.Should().NotBeNullOrEmpty();
            build.Item.Name.Should().NotBeNullOrEmpty();
            build.Item.Status.Should().NotBeNullOrEmpty();

            if (expectedStatus != null)
            {
                build.Item.Status.Should().Be(expectedStatus);
            }
        }

        /// <summary>
        /// Asserts that the specified <see cref="SessionItem"/> is correct.
        /// </summary>
        /// <param name="session">The <see cref="SessionItem"/> to assert on.</param>
        /// <param name="expectedName">The expected name of the session item.</param>
        private static void AssertSession(SessionItem session, string expectedName)
        {
            session.Should().NotBeNull();
            session.Item.Should().NotBeNull();
            session.Item.BuildName.Should().Be(expectedName);
            session.Item.HashedId.Should().NotBeNullOrEmpty();
            session.Item.LogsUri.Should().NotBeNullOrEmpty();
            session.Item.OSName.Should().NotBeNullOrEmpty();
            session.Item.OSVersion.Should().NotBeNullOrEmpty();
            session.Item.ProjectName.Should().NotBeNullOrEmpty();
            session.Item.Reason.Should().NotBeNullOrEmpty();
            session.Item.Status.Should().NotBeNullOrEmpty();
            session.Item.Duration.Should().BeGreaterThan(0);
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
