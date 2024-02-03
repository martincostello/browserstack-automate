﻿// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a BrowserStack Automate session's details. This class cannot be inherited.
/// </summary>
internal sealed class AutomationSessionDetail
{
    /// <summary>
    /// Gets or sets the session.
    /// </summary>
    [JsonPropertyName("automation_session")]
    public Session SessionDetail { get; set; } = default!;
}
