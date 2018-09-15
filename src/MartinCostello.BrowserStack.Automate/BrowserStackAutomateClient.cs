// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a client for the <c>BrowserStack</c> Automate REST API.
    /// </summary>
    public class BrowserStackAutomateClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackAutomateClient"/> class.
        /// </summary>
        /// <param name="userName">The user name to use to authenticate.</param>
        /// <param name="accessKey">The access key to use to authenticate.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="userName"/> or <paramref name="accessKey"/> is <see langword="null"/> or white space.
        /// </exception>
        public BrowserStackAutomateClient(string userName, string accessKey)
            : this(userName, accessKey, new HttpClient())
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
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("No user name specified.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("No access key specified.", nameof(accessKey));
            }

            Client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            UserName = userName;
            SetAuthorization(userName, accessKey);
        }

        /// <summary>
        /// Gets the base URI of the BrowserStack Automate REST API.
        /// </summary>
        public static Uri ApiBaseAddress => new Uri("https://api.browserstack.com/automate/", UriKind.Absolute);

        /// <summary>
        /// Gets the user name in use.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets or sets the <c>Authorization</c> value to use.
        /// </summary>
        private string Authorization { get; set; }

        /// <summary>
        /// Gets the <see cref="HttpClient"/> in use.
        /// </summary>
        private HttpClient Client { get; }

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
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            return new BrowserStackAutomateClient(credential.UserName, credential.Password);
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

            using (var request = CreateRequest(HttpMethod.Delete, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                }
            }
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
            if (buildIds == null)
            {
                throw new ArgumentNullException(nameof(buildIds));
            }

            if (buildIds.Count < 1)
            {
                throw new ArgumentException("No build Ids specified.", nameof(buildIds));
            }

            string query = string.Join("&", buildIds.Select((p) => $"buildId={Uri.EscapeDataString(p)}"));

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "builds?{0}", query);

            using (var request = CreateRequest(HttpMethod.Delete, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                }
            }
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

            using (var request = CreateRequest(HttpMethod.Delete, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                }
            }
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

            using (var request = CreateRequest(HttpMethod.Delete, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                }
            }
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
            if (sessionIds == null)
            {
                throw new ArgumentNullException(nameof(sessionIds));
            }

            if (sessionIds.Count < 1)
            {
                throw new ArgumentException("No session Ids specified.", nameof(sessionIds));
            }

            string query = string.Join("&", sessionIds.Select((p) => $"sessionId={Uri.EscapeDataString(p)}"));

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "session?{0}", query);

            using (var request = CreateRequest(HttpMethod.Delete, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Gets the browsers as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the browsers.
        /// </returns>
        public virtual async Task<ICollection<Browser>> GetBrowsersAsync(CancellationToken cancellationToken = default)
        {
            using (var request = CreateRequest(HttpMethod.Get, "browsers.json"))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<List<Browser>>(response).ConfigureAwait(false);
                }
            }
        }

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
            string status,
            CancellationToken cancellationToken = default)
        {
            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds.json{0}",
                BuildQuery(limit, offset, status));

            using (var request = CreateRequest(HttpMethod.Get, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<List<Build>>(response).ConfigureAwait(false);
                }
            }
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

            using (var request = CreateRequest(HttpMethod.Get, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<ProjectDetailItem>(response).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Gets the projects as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the projects.
        /// </returns>
        public virtual async Task<ICollection<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            using (var request = CreateRequest(HttpMethod.Get, "projects.json"))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<List<Project>>(response).ConfigureAwait(false);
                }
            }
        }

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
        public virtual async Task<SessionDetail> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string relativeUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            using (var request = CreateRequest(HttpMethod.Get, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<SessionDetail>(response).ConfigureAwait(false);
                }
            }
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
            string status,
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

            using (var request = CreateRequest(HttpMethod.Get, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<List<Session>>(response).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Gets the status of the <c>BrowserStack</c> Automate plan as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the status of the Automate plan.
        /// </returns>
        public virtual async Task<AutomatePlanStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            using (var request = CreateRequest(HttpMethod.Get, "plan.json"))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<AutomatePlanStatus>(response).ConfigureAwait(false);
                }
            }
        }

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
        public virtual async Task<RecycleAccessKeyResult> RecycleAccessKeyAsync(CancellationToken cancellationToken = default)
        {
            var value = new { };
            var json = SerializeAsJson(value);

            using (var request = CreateRequest(HttpMethod.Put, "recycle_key.json", json))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);

                    var result = await DeserializeAsync<RecycleAccessKeyResult>(response).ConfigureAwait(false);

                    if (result != null)
                    {
                        SetAuthorization(UserName, result.NewKey);
                    }

                    return result;
                }
            }
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
        public virtual Task<Build> SetBuildNameAsync(
            int buildId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "builds/{0}.json", buildId);

            return SetNameAsync<Build>(relativeUri, name, cancellationToken);
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
        public virtual Task<Project> SetProjectNameAsync(
            int projectId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "projects/{0}.json", projectId);

            return SetNameAsync<Project>(relativeUri, name, cancellationToken);
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
        public virtual Task<Session> SetSessionNameAsync(
            string sessionId,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("No name specified.", nameof(name));
            }

            string relativeUri = string.Format(CultureInfo.InvariantCulture, "sessions/{0}.json", sessionId);

            return SetNameAsync<Session>(relativeUri, name, cancellationToken);
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
        public virtual async Task<Session> SetSessionStatusAsync(
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

            var value = new
            {
                status,
                reason,
            };

            var json = SerializeAsJson(value);

            using (var request = CreateRequest(HttpMethod.Put, relativeUri, json))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<Session>(response).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Deserializes the content of the specified <see cref="HttpResponseMessage"/> as an asychronous operation.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the content as.</typeparam>
        /// <param name="response">The HTTP response to deserialize.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asychronous operation to deserialize the response.
        /// </returns>
        protected virtual async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serializes the specified <see cref="object"/> as JSON.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <returns>
        /// A <see cref="string"/> containing the JSON representation of <paramref name="value"/>.
        /// </returns>
        protected virtual string SerializeAsJson(object value)
            => JsonConvert.SerializeObject(value);

        /// <summary>
        /// Builds the query string parameters to use, if any, for the specified parameters.
        /// </summary>
        /// <param name="limit">The limit to use, if any.</param>
        /// <param name="offset">The offset to use, if any.</param>
        /// <param name="status">The status to filter to, if any.</param>
        /// <returns>
        /// The query string to use, if any.
        /// </returns>
        private static string BuildQuery(int? limit, int? offset, string status)
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
                    builder.Append("&");
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
        /// Ensures that the specified <see cref="HttpResponseMessage"/> was successful.
        /// </summary>
        /// <param name="response">The HTTP response message.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to test the response for success.
        /// </returns>
        private async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                try
                {
                    var error = await DeserializeAsync<BrowserStackAutomateError>(response).ConfigureAwait(false);
                    throw new BrowserStackAutomateException(error);
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    // Just fall-through to EnsureSuccessStatusCode() if deserialization fails
                }
                catch (JsonReaderException)
                {
                    // Just fall-through to EnsureSuccessStatusCode() if deserialization fails
                }
            }

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Sets the <c>Authorization</c> header value used by this instance.
        /// </summary>
        /// <param name="userName">The user name to use.</param>
        /// <param name="accessKey">The access key to use.</param>
        private void SetAuthorization(string userName, string accessKey)
        {
            Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", userName, accessKey)));
        }

        /// <summary>
        /// Creates a new HTTP request message.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="relativeUri">The relative URI of the request.</param>
        /// <param name="json">The optional HTTP JSON content to send.</param>
        /// <returns>
        /// The <see cref="HttpRequestMessage"/> to send.
        /// </returns>
        private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUri, string json = null)
        {
            var requestUri = new Uri(ApiBaseAddress, relativeUri);
            var request = new HttpRequestMessage(method, requestUri);

            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Authorization);

                if (json != null)
                {
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                return request;
            }
            catch (Exception)
            {
                request.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets the specified type of logs associated with the specified build and session Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id to return the logs for.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <param name="logType">The log type to retrieve.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
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
            CancellationToken cancellationToken = default)
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

            using (var request = CreateRequest(HttpMethod.Get, relativeUri))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Returns an HTML error page, so just return empty if there are no logs
                        return string.Empty;
                    }

                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Sets the name of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="relativeUri">The relative URI of the resource to set the name of.</param>
        /// <param name="name">The new name.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to set the name for the specified resource.
        /// </returns>
        private async Task<T> SetNameAsync<T>(
            string relativeUri,
            string name,
            CancellationToken cancellationToken = default)
        {
            var json = SerializeAsJson(new { name });

            using (var request = CreateRequest(HttpMethod.Put, relativeUri, json))
            {
                using (var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await EnsureSuccessAsync(response).ConfigureAwait(false);
                    return await DeserializeAsync<T>(response).ConfigureAwait(false);
                }
            }
        }
    }
}
