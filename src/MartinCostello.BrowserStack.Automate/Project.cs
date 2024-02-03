// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class representing a project.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Project
    {
        /// <summary>
        /// Gets or sets the project Id.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time the project was last updated.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the project was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }

        /// <summary>
        /// Gets or sets the group Id.
        /// </summary>
        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }
    }
}
