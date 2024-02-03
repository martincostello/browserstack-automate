// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a request to set a session's status. This class cannot be inherited.
/// </summary>
internal sealed class SetSessionStatusRequest
{
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = default!;

    /// <summary>
    /// Gets or sets the reason.
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = default!;
}
