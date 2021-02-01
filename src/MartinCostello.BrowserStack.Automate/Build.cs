// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a build.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Build
    {
        /// <summary>
        /// Gets or sets the hashed Id of the build.
        /// </summary>
        [JsonProperty("hashed_id")]
        public string HashedId { get; set; }

        /// <summary>
        /// Gets or sets the name of the build.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration of the build, in seconds.
        /// </summary>
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the status of the build.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
