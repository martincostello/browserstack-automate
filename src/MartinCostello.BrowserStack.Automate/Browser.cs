// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a browser.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(BrowserName) + "}")]
    public class Browser : IBrowserInfo
    {
        /// <summary>
        /// Gets or sets the Operating System version.
        /// </summary>
        [JsonProperty("os_version")]
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets the Operating System name.
        /// </summary>
        [JsonProperty("os")]
        public string OSName { get; set; }

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        [JsonProperty("browser_version")]
        public string BrowserVersion { get; set; }

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        [JsonProperty("device")]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        [JsonProperty("browser")]
        public string BrowserName { get; set; }
    }
}
