// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a project item.
    /// </summary>
    public class ProjectItem : IAutomateItem<Project>
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        [JsonProperty("automation_project")]
        public Project Item { get; set; }
    }
}
