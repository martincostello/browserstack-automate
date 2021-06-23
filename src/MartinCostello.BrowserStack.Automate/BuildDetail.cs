// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing detail about a build.
    /// </summary>
    public class BuildDetail : Build
    {
        /// <summary>
        /// Gets or sets the date and time the build was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time the build was updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the group Id.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the build Id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a delta.
        /// </summary>
        [JsonProperty("delta")]
        public bool IsDelta { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [JsonProperty("tags")]
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the project Id.
        /// </summary>
        [JsonProperty("automation_project_id")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }
}
