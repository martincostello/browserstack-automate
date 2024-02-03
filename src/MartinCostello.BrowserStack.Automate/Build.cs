// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a build.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
public class Build
{
    /// <summary>
    /// Gets or sets the hashed Id of the build.
    /// </summary>
    [JsonPropertyName("hashed_id")]
    public string HashedId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the build.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration of the build, in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets the status of the build.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public URL of the build, if any.
    /// </summary>
    [JsonPropertyName("public_url")]
    public string? PublicUrl { get; set; }

    /// <summary>
    /// Gets or sets the build tag of the build, if any.
    /// </summary>
    [JsonPropertyName("build_tag")]
    public string? BuildTag { get; set; }
}
