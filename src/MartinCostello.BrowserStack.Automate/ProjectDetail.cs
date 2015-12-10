// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing detailed information about a project.
    /// </summary>
    public class ProjectDetail : Project
    {
        /// <summary>
        /// Gets or sets the build details.
        /// </summary>
        [JsonProperty("builds")]
        public ICollection<BuildDetailItem> Builds { get; set; }
    }
}
