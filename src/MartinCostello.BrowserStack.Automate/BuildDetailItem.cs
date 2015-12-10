// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a build detail item.
    /// </summary>
    public class BuildDetailItem : IAutomateItem<BuildDetail>
    {
        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        [JsonProperty("automation_build")]
        public BuildDetail Item { get; set; }
    }
}
