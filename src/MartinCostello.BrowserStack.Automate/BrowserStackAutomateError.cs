// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing a BrowserStack Automate error.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(Message) + "}")]
#if NET451
    [System.Serializable]
#endif
    public class BrowserStackAutomateError
    {
        /// <summary>
        /// Gets or sets the error status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
