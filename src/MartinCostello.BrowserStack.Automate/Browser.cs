// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing a browser.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(BrowserName) + "}")]
    public class Browser : IBrowserInfo
    {
        /// <summary>
        /// Gets or sets the Operating System version.
        /// </summary>
        [JsonPropertyName("os_version")]
        public string OSVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Operating System name.
        /// </summary>
        [JsonPropertyName("os")]
        public string OSName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the browser version.
        /// </summary>
        [JsonPropertyName("browser_version")]
        public string BrowserVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the device name.
        /// </summary>
        [JsonPropertyName("device")]
        public string Device { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        [JsonPropertyName("browser")]
        public string BrowserName { get; set; } = string.Empty;
    }
}
