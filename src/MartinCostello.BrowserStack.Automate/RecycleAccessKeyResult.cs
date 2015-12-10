// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing the result of recycling the access key.
    /// </summary>
    public class RecycleAccessKeyResult
    {
        /// <summary>
        /// Gets or sets the new access key.
        /// </summary>
        [JsonProperty("new_key")]
        public string NewKey { get; set; }

        /// <summary>
        /// Gets or sets the old access key.
        /// </summary>
        [JsonProperty("old_key")]
        public string OldKey { get; set; }
    }
}
