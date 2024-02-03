// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

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
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time the build was updated.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the group Id.
        /// </summary>
        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the build Id.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a delta.
        /// </summary>
        [JsonPropertyName("delta")]
        public bool IsDelta { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [JsonPropertyName("tags")]
        public ICollection<string> Tags { get; set; } = [];

        /// <summary>
        /// Gets or sets the project Id.
        /// </summary>
        [JsonPropertyName("automation_project_id")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
    }
}
