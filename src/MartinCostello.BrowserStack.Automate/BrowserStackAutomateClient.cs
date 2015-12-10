// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

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
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("No user name specified.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("No access key specified.", nameof(accessKey));
            }

            UserName = userName;
            SetAuthorization(userName, accessKey);
        }

        /// <summary>
        /// Gets the base URI of the <c>BrowserStack</c> Automate REST API.
        /// </summary>
        public static Uri ApiBaseAddress => new Uri("https://www.browserstack.com/automate/", UriKind.Absolute);

        /// <summary>
        /// Gets the user name in use.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets or sets the <c>Authorization</c> value to use.
        /// </summary>
        private string Authorization { get; set; }

        /// <summary>
        /// Gets the browsers as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the browsers.
        /// </returns>
        public virtual async Task<ICollection<Browser>> GetBrowsersAsync()
        {
            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync("browsers.json"))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<List<Browser>>();
                }
            }
        }

        /// <summary>
        /// Gets the builds as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the builds.
        /// </returns>
        public virtual Task<ICollection<BuildItem>> GetBuildsAsync() => GetBuildsAsync(null, null);

        /// <summary>
        /// Gets the builds as an asynchronous operation.
        /// </summary>
        /// <param name="limit">The optional number of builds to return. The default value is 10.</param>
        /// <param name="status">The optional status to filter builds to.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the builds.
        /// </returns>
        public virtual async Task<ICollection<BuildItem>> GetBuildsAsync(int? limit, string status)
        {
            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds.json{0}",
                BuildQuery(limit, status));

            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<List<BuildItem>>();
                }
            }
        }

        /// <summary>
        /// Gets the project with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="projectId">The Id of the project to return.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the project with the specified Id.
        /// </returns>
        public virtual async Task<ProjectDetailItem> GetProjectAsync(int projectId)
        {
            string requestUri = string.Format(CultureInfo.InvariantCulture, "projects/{0}.json", projectId);

            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<ProjectDetailItem>();
                }
            }
        }

        /// <summary>
        /// Gets the projects as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the projects.
        /// </returns>
        public virtual async Task<ICollection<ProjectItem>> GetProjectsAsync()
        {
            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync("projects.json"))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<List<ProjectItem>>();
                }
            }
        }

        /// <summary>
        /// Gets the session associated with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The session Id to return.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the session with the specified Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<SessionDetailItem> GetSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<SessionDetailItem>();
                }
            }
        }

        /// <summary>
        /// Gets the logs of the session associated with the specified build Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id of the session.</param>
        /// <param name="sessionId">The session Id to return the logs for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the logs for the session with the specified Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> or <paramref name="sessionId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<string> GetSessionLogsAsync(string buildId, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(buildId))
            {
                throw new ArgumentException("No build Id specified.", nameof(buildId));
            }

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds/{0}/sessions/{1}/logs",
                Uri.EscapeDataString(buildId),
                Uri.EscapeDataString(sessionId));

            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Gets the sessions associated with the specified build Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id of the sessions to return.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the sessions for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual Task<ICollection<SessionItem>> GetSessionsAsync(string buildId) => GetSessionsAsync(buildId, null, null);

        /// <summary>
        /// Gets the sessions associated with the specified build Id as an asynchronous operation.
        /// </summary>
        /// <param name="buildId">The build Id of the sessions to return.</param>
        /// <param name="limit">The optional number of builds to return. The default value is 10.</param>
        /// <param name="status">The optional status to filter builds to.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the sessions for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="buildId"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<ICollection<SessionItem>> GetSessionsAsync(string buildId, int? limit, string status)
        {
            if (string.IsNullOrWhiteSpace(buildId))
            {
                throw new ArgumentException("No build Id specified.", nameof(buildId));
            }

            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "builds/{0}/sessions.json{1}",
                Uri.EscapeDataString(buildId),
                BuildQuery(limit, status));

            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<List<SessionItem>>();
                }
            }
        }

        /// <summary>
        /// Gets the status of the <c>BrowserStack</c> Automate plan as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the status of the Automate plan.
        /// </returns>
        public virtual async Task<AutomatePlanStatus> GetStatusAsync()
        {
            using (var client = CreateClient())
            {
                using (var response = await client.GetAsync("plan.json"))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<AutomatePlanStatus>();
                }
            }
        }

        /// <summary>
        /// Recycles the current access key as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to recycle the access key which returns the new and old access keys.
        /// </returns>
        /// <remarks>
        /// The credentials used by the current instance are automatically updated if successful.
        /// </remarks>
        public virtual async Task<RecycleAccessKeyResult> RecycleAccessKeyAsync()
        {
            var value = new { };

            using (var client = CreateClient())
            {
                using (var response = await client.PutAsJsonAsync("recycle_key.json", value))
                {
                    response.EnsureSuccessStatusCode();

                    RecycleAccessKeyResult result = await response.Content.ReadAsAsync<RecycleAccessKeyResult>();

                    if (result != null)
                    {
                        SetAuthorization(UserName, result.NewKey);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Sets the status of the session with the specified Id as an asynchronous operation.
        /// </summary>
        /// <param name="sessionId">The session Id to set the status of.</param>
        /// <param name="status">The new status.</param>
        /// <param name="reason">An optional reason to specify.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the sessions for the specified build Id.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="sessionId"/> or <paramref name="status"/> is <see langword="null"/> or white space.
        /// </exception>
        public virtual async Task<string> SetSessionStatusAsync(string sessionId, string status, string reason)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("No session Id specified.", nameof(sessionId));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("No status specified.", nameof(status));
            }

            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "sessions/{0}.json",
                Uri.EscapeDataString(sessionId));

            var value = new
            {
                status = status,
                reason = reason,
            };

            using (var client = CreateClient())
            {
                using (var response = await client.PutAsJsonAsync(requestUri, value))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="HttpClient"/> to use.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="HttpClient"/>.
        /// </returns>
        protected virtual HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();

            try
            {
                client.BaseAddress = ApiBaseAddress;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Authorization);

                return client;
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Builds the query string parameters to use, if any, for the specified parameters.
        /// </summary>
        /// <param name="limit">The limit to use, if any.</param>
        /// <param name="status">The status to filter to, if any.</param>
        /// <returns>
        /// The query string to use, if any.
        /// </returns>
        private string BuildQuery(int? limit, string status)
        {
            StringBuilder builder = new StringBuilder();

            if (limit.HasValue)
            {
                if (limit < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(limit), limit.Value, "The limit value cannot be less than one.");
                }

                builder.AppendFormat(CultureInfo.InvariantCulture, "limit={0}", limit.Value);
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
        /// Sets the <c>Authorization</c> header value used by this instance.
        /// </summary>
        /// <param name="userName">The user name to use.</param>
        /// <param name="accessKey">The access key to use.</param>
        private void SetAuthorization(string userName, string accessKey)
        {
            Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", userName, accessKey)));
        }
    }
}
