// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing a client for the <c>BrowserStack</c> Automate REST API.
    /// </summary>
    public class BrowserStackAutomateClient : IDisposable
    {
        /// <summary>
        /// Gets the used specified <see cref="HttpClient"/> to use.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Whether the instance owns the <see cref="HttpClient"/> instance.
        /// </summary>
        private readonly bool _ownsClient;

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateClient"/> class.
        /// </summary>
        /// <param name="userName">The user name to use to authenticate.</param>
        /// <param name="accessKey">The access key to use to authenticate.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="userName"/> or <paramref name="accessKey"/> is <see langword="null"/> or white space.
        /// </exception>
        public BrowserStackAutomateClient(string userName, string accessKey)
            : this(userName, accessKey, new HttpClient(), ownsClient: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateClient"/> class.
        /// </summary>
        /// <param name="userName">The user name to use to authenticate.</param>
        /// <param name="accessKey">The access key to use to authenticate.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="userName"/> or <paramref name="accessKey"/> is <see langword="null"/> or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        public BrowserStackAutomateClient(string userName, string accessKey, HttpClient httpClient)
            : this(userName, accessKey, httpClient, ownsClient: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateClient"/> class.
        /// </summary>
        /// <param name="userName">The user name to use to authenticate.</param>
        /// <param name="accessKey">The access key to use to authenticate.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use.</param>
        /// <param name="ownsClient">Whether the instance owns the HTTP client.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="userName"/> or <paramref name="accessKey"/> is <see langword="null"/> or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="httpClient"/> is <see langword="null"/>.
        /// </exception>
        private BrowserStackAutomateClient(string userName, string accessKey, HttpClient httpClient, bool ownsClient)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("No user name specified.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("No access key specified.", nameof(accessKey));
            }

            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsClient = ownsClient;
            UserName = userName;

            _client.BaseAddress ??= ApiBaseAddress;

            if (_client.DefaultRequestHeaders.Authorization is null)
            {
                SetAuthorization(_client, userName, accessKey);
            }
        }

        /// <summary>
        /// Gets the base URI of the BrowserStack Automate REST API.
        /// </summary>
        public static Uri ApiBaseAddress { get; } = new("https://api.browserstack.com/automate/", UriKind.Absolute);

        /// <summary>
        /// Gets the user name in use.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Creates an instance of <see cref="BrowserStackAutomateClient"/> using the specified <see cref="NetworkCredential"/>.
        /// </summary>
        /// <param name="credential">The credential to use the authenticate.</param>
        /// <returns>
        /// The created instance of <see cref="BrowserStackAutomateClient"/> authenticated using <paramref name="credential"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="credential"/> is <see langword="null"/>.
        /// </exception>
        public static BrowserStackAutomateClient FromCredential(NetworkCredential credential)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(credential);
#else
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }
#endif

            return new BrowserStackAutomateClient(credential.UserName, credential.Password);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deletes the build with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The Id of the build to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to delete the build with the specified Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> is <see langword="null"/> or white space.
        /// </exception>
        /// <exception cref="BrowserStackAutomateException">
        /// The build could not be deleted.
        /// </exception>
        public virtual async Task DeleteBuildAsync(string buildId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(buildId))
            {
                throw new ArgumentException("No build Id specified.", nameof(buildId));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "builds/{0}.json", Uri.EscapeDataString(buildId));

            await DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the builds with the specified Ids as an asynchronous operation.
        /// </summary>
        /// <param name="buildIds">The Ids of the builds to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to delete the build with the specified Id.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buildIds"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildIds"/> is empty.
        /// </exception>
        /// <exception cref="BrowserStackAutomateException">
        /// The builds could not be deleted.
        /// </exception>
        public virtual async Task DeleteBuildsAsync(ICollection<string> buildIds, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(buildIds);
#else
            if (buildIds == null)
            {
                throw new ArgumentNullException(nameof(buildIds));
            }
#endif

            if (buildIds.Count < 1)
            {
                throw new ArgumentException("No build Ids specified.", nameof(buildIds));
            }

            string query = string.Join("&", buildIds.Select((p) => $"buildId={Uri.EscapeDataString(p)}"));

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "builds?{0}", query);

            await DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the project with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="projectId">The Id of the project to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to delete the project with the specified Id.
        /// </returns>
        /// <exception cref="BrowserStackAutomateException">
        /// The project could not be deleted.
        /// </exception>
        public virtual async Task DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default)
        {
            string relativeUri = string.Format(CultureInfo.InvariantCulture, "projects/{0}.json", projectId);

            await DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The Id of the session to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to delete the session with the specified Id.
        /// </returns>
        /// <exception cref="BrowserStackAutomateException">
        /// The session could not be deleted.
        /// </exception>
        public virtual async Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            await DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the sessions with the specified Ids as an asynchronous operation.
        /// </summary>
        /// <param name="sessionIds">The Ids of the sessions to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to delete the sessions with the specified Ids.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sessionIds"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="sessionIds"/> is empty.
        /// </exception>
        /// <exception cref="BrowserStackAutomateException">
        /// The sessions could not be deleted.
        /// </exception>
        public virtual async Task DeleteSessionsAsync(ICollection<string> sessionIds, CancellationToken cancellationToken = default)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(sessionIds);
#else
            if (sessionIds == null)
            {
                throw new ArgumentNullException(nameof(sessionIds));
            }
#endif

            if (sessionIds.Count < 1)
            {
                throw new ArgumentException("No session Ids specified.", nameof(sessionIds));
            }

            string query = string.Join("&", sessionIds.Select((p) => $"sessionId={Uri.EscapeDataString(p)}"));

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "session?{0}", query);

            await DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the browsers as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the browsers.
        /// </returns>
        public virtual async Task<ICollection<Browser>> GetBrowsersAsync(CancellationToken cancellationToken = default)
            => await GetJsonAsync("browsers.json", AppJsonSerializerContext.Default.ListBrowser, cancellationToken).ConfigureAwait(false) ?? [];

        /// <summary>
        /// Gets the builds as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the builds.
        /// </returns>
        public virtual Task<ICollection<Build>> GetBuildsAsync(CancellationToken cancellationToken = default)
            => GetBuildsAsync(null, null, null, cancellationToken);

        /// <summary>
        /// Gets the builds as an asynchronous operation.
        /// </summary>
        /// <param name="limit">The optional number of builds to return. The default value is 10.</param>
        /// <param name="offset">The optional offset for builds to return. The default value is 0.</param>
        /// <param name="status">The optional status to filter builds to.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the builds.
        /// </returns>
        public virtual async Task<ICollection<Build>> GetBuildsAsync(
            int? limit,
            int? offset,
            string? status,
            CancellationToken cancellationToken = default)
        {
            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds.json{0}",
                BuildQuery(limit, offset, status));

            var builds = await GetJsonAsync(
                relativeUri,
                AppJsonSerializerContext.Default.ListAutomationBuild,
                cancellationToken).ConfigureAwait(false) ?? [];

            return builds
                .Select((p) => p.Build)
                .ToList();
        }

        /// <summary>
        /// Gets the project with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="projectId">The Id of the project to return.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the project with the specified Id.
        /// </returns>
        public virtual async Task<ProjectDetailItem> GetProjectAsync(int projectId, CancellationToken cancellationToken = default)
        {
            string relativeUri = string.Format(CultureInfo.InvariantCulture, "projects/{0}.json", projectId);

            var item = await GetJsonAsync(
                relativeUri,
                AppJsonSerializerContext.Default.ProjectDetailItem,
                cancellationToken).ConfigureAwait(false);

            return item!;
        }

        /// <summary>
        /// Gets the projects as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the projects.
        /// </returns>
        public virtual async Task<ICollection<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
            => await GetJsonAsync("projects.json", AppJsonSerializerContext.Default.ListProject, cancellationToken).ConfigureAwait(false) ?? [];

        /// <summary>
        /// Gets the session associated with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The session Id to return.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the session with the specified Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<SessionDetail?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            var result = await GetJsonAsync(
                relativeUri,
                AppJsonSerializerContext.Default.AutomationSessionDetail,
                cancellationToken).ConfigureAwait(false);

            return result?.SessionDetail;
        }

        /// <summary>
        /// Gets the session logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the build and
        /// session with the specified Ids as a string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<string> GetSessionLogsAsync(string buildId, string sessionId, CancellationToken cancellationToken = default)
            => GetLogsAsync(buildId, sessionId, "logs", cancellationToken);

        /// <summary>
        /// Gets the raw session Appium logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the build and session
        /// with the specified Ids as a string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<string> GetSessionAppiumLogsAsync(string buildId, string sessionId, CancellationToken cancellationToken = default)
            => GetLogsAsync(buildId, sessionId, "appiumlogs", cancellationToken);

        /// <summary>
        /// Gets the session console logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the build and session
        /// with the specified Ids as a string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<string> GetSessionConsoleLogsAsync(string buildId, string sessionId, CancellationToken cancellationToken = default)
            => GetLogsAsync(buildId, sessionId, "consolelogs", cancellationToken);

        /// <summary>
        /// Gets the session network logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the build and session
        /// with the specified Ids as a string in HAR (HTTP Archive) format.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<string> GetSessionNetworkLogsAsync(string buildId, string sessionId, CancellationToken cancellationToken = default)
            => GetLogsAsync(buildId, sessionId, "networklogs", cancellationToken);

        /// <summary>
        /// Gets the sessions associated with the specified build Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id of the sessions to return.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the sessions for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<ICollection<Session>> GetSessionsAsync(string buildId, CancellationToken cancellationToken = default)
            => GetSessionsAsync(buildId, null, null, null, cancellationToken);

        /// <summary>
        /// Gets the sessions associated with the specified build Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id of the sessions to return.</param>
        /// <param name="limit">The optional number of sessions to return. The default value is 10.</param>
        /// <param name="offset">The optional offset for sessions to return. The default value is 0.</param>
        /// <param name="status">The optional status to filter sessions to.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the sessions for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<ICollection<Session>> GetSessionsAsync(
            string buildId,
            int? limit,
            int? offset,
            string? status,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(buildId))
            {
                throw new ArgumentException("No build Id specified.", nameof(buildId));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds/{0}/sessions.json{1}",
                Uri.EscapeDataString(buildId),
                BuildQuery(limit, offset, status));

            var sessions = await GetJsonAsync(
                relativeUri,
                AppJsonSerializerContext.Default.ListAutomationSession,
                cancellationToken).ConfigureAwait(false) ?? [];

            return sessions
                .Select((p) => p.Session)
                .ToList();
        }

        /// <summary>
        /// Gets the status of the <c>BrowserStack</c> Automate plan as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the status of the Automate plan.
        /// </returns>
        public virtual async Task<AutomatePlanStatus?> GetStatusAsync(CancellationToken cancellationToken = default)
            => await GetJsonAsync("plan.json", AppJsonSerializerContext.Default.AutomatePlanStatus, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Recycles the current access key as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to recycle the access key which returns the new and old access keys.
        /// </returns>
        /// <remarks>
        /// The credentials used by the current instance are automatically updated if successful.
        /// </remarks>
        public virtual async Task<RecycleAccessKeyResult?> RecycleAccessKeyAsync(CancellationToken cancellationToken = default)
        {
            var result = await PutJsonAsync(
                "recycle_key.json",
                new EmptyRequest(),
                AppJsonSerializerContext.Default.EmptyRequest,
                AppJsonSerializerContext.Default.RecycleAccessKeyResult,
                cancellationToken).ConfigureAwait(false);

            if (result?.NewKey is { } accessKey)
            {
                SetAuthorization(_client, UserName, accessKey);
            }

            return result;
        }

        /// <summary>
        /// Sets the name of the build with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to set the name of.</param>
        /// <param name="name">The new name.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<Build?> SetBuildNameAsync(
            int buildId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "builds/{0}.json", buildId);

            return SetNameAsync(relativeUri, name, AppJsonSerializerContext.Default.Build, cancellationToken);
        }

        /// <summary>
        /// Sets the name of the project with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="projectId">The project Id to set the name of.</param>
        /// <param name="name">The new name.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified project Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<Project?> SetProjectNameAsync(
            int projectId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "projects/{0}.json", projectId);

            return SetNameAsync(relativeUri, name, AppJsonSerializerContext.Default.Project, cancellationToken);
        }

        /// <summary>
        /// Sets the name of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The session Id to set the name of.</param>
        /// <param name="name">The new name.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified session Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<Session?> SetSessionNameAsync(
            string sessionId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "sessions/{0}.json", sessionId);

            return SetNameAsync(relativeUri, name, AppJsonSerializerContext.Default.Session, cancellationToken);
        }

        /// <summary>
        /// Sets the status of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The session Id to set the status of.</param>
        /// <param name="status">The new status.</param>
        /// <param name="reason">An optional reason to specify.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the status for the specified session Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="sessionId"/> or <paramref name="status"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<Session?> SetSessionStatusAsync(
            string sessionId,
            string status,
            string reason,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("No status specified.", nameof(status));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            var request = new SetSessionStatusRequest()
            {
                Status = status,
                Reason = reason,
            };

            var session = await PutJsonAsync(
                relativeUri,
                request,
                AppJsonSerializerContext.Default.SetSessionStatusRequest,
                AppJsonSerializerContext.Default.AutomationSession,
                cancellationToken).ConfigureAwait(false);

            return session?.Session;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _ownsClient)
                {
                    _client?.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Builds the query string parameters to use, if any, for the specified parameters.
        /// </summary>
        /// <param name="limit">The limit to use, if any.</param>
        /// <param name="offset">The offset to use, if any.</param>
        /// <param name="status">The status to filter to, if any.</param>
        /// <returns>
        /// The query string to use, if any.
        /// </returns>
        private static string BuildQuery(int? limit, int? offset, string? status)
        {
            var builder = new StringBuilder();

            if (limit.HasValue)
            {
                if (limit < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(limit), limit.Value, "The limit value cannot be less than one.");
                }

                builder.AppendFormat(CultureInfo.InvariantCulture, "limit={0}", limit.Value);
            }

            if (offset.HasValue)
            {
                if (offset < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(offset), offset.Value, "The offset value cannot be less than zero.");
                }

                builder.AppendFormat(CultureInfo.InvariantCulture, "offset={0}", offset.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }

                builder.AppendFormat(CultureInfo.InvariantCulture, "status={0}", Uri.EscapeDataString(status));
            }

            if (builder.Length > 0)
            {
                builder.Insert(0, "?");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Sets the authorization header for the specified <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client">The HTTP client to configure.</param>
        /// <param name="userName">The BrowserStack Automate user name.</param>
        /// <param name="accessKey">The BrowserStack Automate access key.</param>
        private static void SetAuthorization(HttpClient client, string userName, string accessKey)
        {
            string authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{accessKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorization);
        }

        /// <summary>
        /// Ensures that the specified <see cref="HttpResponseMessage"/> was successful.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to test the response for success.
        /// </returns>
        private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                try
                {
                    var error = await response.Content.ReadFromJsonAsync(
                        AppJsonSerializerContext.Default.BrowserStackAutomateError,
                        cancellationToken).ConfigureAwait(false);

                    throw new BrowserStackAutomateException(error);
                }
                catch (JsonException)
                {
                    // Just fall-through to EnsureSuccessStatusCode() if deserialization fails
                }
            }

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Gets the specified type of logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="logType">The log type to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the build and session with the specified Ids.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        private async Task<string> GetLogsAsync(
            string buildId,
            string sessionId,
            string logType,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(buildId))
            {
                throw new ArgumentException("No build Id specified.", nameof(buildId));
            }

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds/{0}/sessions/{1}/{2}",
                Uri.EscapeDataString(buildId),
                Uri.EscapeDataString(sessionId),
                Uri.EscapeDataString(logType));

            using var response = await _client.GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                // Returns an HTML error page, so just return empty if there are no logs
                return string.Empty;
            }

            response.EnsureSuccessStatusCode();

#if NET8_0_OR_GREATER
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Deletes the specified resource as an asynchronous operation.
        /// </summary>
        /// <param name="relativeUri">The relative URI of the resource to delete.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to delete the specified resource.
        /// </returns>
        private async Task DeleteAsync(string relativeUri, CancellationToken cancellationToken)
        {
            using var response = await _client.DeleteAsync(relativeUri, cancellationToken).ConfigureAwait(false);
            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the specified resource as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The type of the JSON response.</typeparam>
        /// <param name="relativeUri">The relative URI of the resource to delete.</param>
        /// <param name="jsonTypeInfo">The JSON type information to use.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to delete the specified resource.
        /// </returns>
        private async Task<T?> GetJsonAsync<T>(string relativeUri, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
        {
            using var response = await _client.GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);

            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the name of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="relativeUri">The relative URI of the resource to set the name of.</param>
        /// <param name="request">The request body.</param>
        /// <param name="requestJsonTypeInfo">The JSON type information to use for the request.</param>
        /// <param name="responseJsonTypeInfo">The JSON type information to use for the response.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified resource.
        /// </returns>
        private async Task<TResponse?> PutJsonAsync<TRequest, TResponse>(
            string relativeUri,
            TRequest request,
            JsonTypeInfo<TRequest> requestJsonTypeInfo,
            JsonTypeInfo<TResponse> responseJsonTypeInfo,
            CancellationToken cancellationToken)
        {
            using var response = await _client.PutAsJsonAsync(
                relativeUri,
                request,
                requestJsonTypeInfo,
                cancellationToken).ConfigureAwait(false);

            await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync(responseJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the name of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="relativeUri">The relative URI of the resource to set the name of.</param>
        /// <param name="name">The new name.</param>
        /// <param name="jsonTypeInfo">The JSON type information to use.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified resource.
        /// </returns>
        private async Task<T?> SetNameAsync<T>(
            string relativeUri,
            string name,
            JsonTypeInfo<T> jsonTypeInfo,
            CancellationToken cancellationToken)
        {
            var request = new SetNameRequest() { Name = name };

            return await PutJsonAsync(
                relativeUri,
                request,
                AppJsonSerializerContext.Default.SetNameRequest,
                jsonTypeInfo,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
