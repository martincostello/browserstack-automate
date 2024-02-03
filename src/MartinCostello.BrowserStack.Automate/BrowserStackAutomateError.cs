// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a BrowserStack Automate error.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{" + nameof(Message) + "}")]
public class BrowserStackAutomateError
{
    /// <summary>
    /// Gets or sets the error status.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
