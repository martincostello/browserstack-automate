// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing a project detail item.
    /// </summary>
    public class ProjectDetailItem
    {
        /// <summary>
        /// Gets or sets the project detail.
        /// </summary>
        [JsonProperty("project")]
        public ProjectDetail Project { get; set; }
    }
}
