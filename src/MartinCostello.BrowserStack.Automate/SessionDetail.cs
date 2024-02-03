﻿// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing detail about a session.
    /// </summary>
    public class SessionDetail : Session
    {
        /// <summary>
        /// Gets or sets the URI of the browser.
        /// </summary>
        [JsonPropertyName("browser_url")]
        public string BrowserUri { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the public URI of the session.
        /// </summary>
        [JsonPropertyName("public_url")]
        public string PublicUri { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URI of the video.
        /// </summary>
        [JsonPropertyName("video_url")]
        public string VideoUri { get; set; } = string.Empty;
    }
}
