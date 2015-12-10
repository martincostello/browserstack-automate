// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a build item.
    /// </summary>
    public class BuildItem
    {
        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        [JsonProperty("automation_build")]
        public Build Item { get; set; }
    }
}
