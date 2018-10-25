// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a session.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Session : IBrowserInfo
    {
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the hashed Id of the session.
        /// </summary>
        [JsonProperty("hashed_id")]
        public string HashedId { get; set; }

        /// <summary>
        /// Gets or sets the duration of the session in seconds.
        /// </summary>
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)] // TODO Should be nullable, but don't want to break binary compatibility
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        [JsonProperty("browser_version")]
        public string BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the OS version.
        /// </summary>
        [JsonProperty("os_version")]
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Operating System name.
        /// </summary>
        [JsonProperty("os")]
        public string OSName { get; set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [JsonProperty("project_name")]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the session status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the build name.
        /// </summary>
        [JsonProperty("build_name")]
        public string BuildName { get; set; }

        /// <summary>
        /// Gets or sets the URI of the logs.
        /// </summary>
        [JsonProperty("logs")]
        public string LogsUri { get; set; }

        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        [JsonProperty("browser")]
        public string BrowserName { get; set; }
    }
}
