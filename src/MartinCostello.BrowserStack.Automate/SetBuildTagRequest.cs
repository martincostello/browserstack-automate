// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a request to set a build tag. This class cannot be inherited.
/// </summary>
internal sealed class SetBuildTagRequest
{
    /// <summary>
    /// Gets or sets the build tag.
    /// </summary>
    [JsonPropertyName("build_tag")]
    public string BuildTag { get; set; } = default!;
}
