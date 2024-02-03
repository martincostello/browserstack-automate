// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class containing the names of the build statuses. This class cannot be inherited.
/// </summary>
public static class BuildStatuses
{
    /// <summary>
    /// Gets the name of the status for a job which is done.
    /// </summary>
    public static string Done => "done";

    /// <summary>
    /// Gets the name of the status for a job which has failed.
    /// </summary>
    public static string Failed => "failed";

    /// <summary>
    /// Gets the name of the status for a job which is running.
    /// </summary>
    public static string Running => "running";
}
