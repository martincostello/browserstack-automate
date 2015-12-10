// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a project.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Project
    {
        /// <summary>
        /// Gets or sets the project Id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time the project was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time the project was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the group Id.
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
    }
}
