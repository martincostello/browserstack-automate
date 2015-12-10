// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A class containing the names of the session statuses. This class cannot be inherited.
    /// </summary>
    public static class SessionStatuses
    {
        /// <summary>
        /// Gets the name of the status for a session which is completed.
        /// </summary>
        public static string Completed => "completed";

        /// <summary>
        /// Gets the name of the status for a session which is done.
        /// </summary>
        public static string Done => BuildStatuses.Done;

        /// <summary>
        /// Gets the name of the status for a session which has an error.
        /// </summary>
        public static string Error => "error";
    }
}
