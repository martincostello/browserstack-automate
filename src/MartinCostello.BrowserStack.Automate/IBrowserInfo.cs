// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// Defines information about a browser.
    /// </summary>
    public interface IBrowserInfo
    {
        /// <summary>
        /// Gets the Operating System version.
        /// </summary>
        string OSVersion { get; }

        /// <summary>
        /// Gets the Operating System name.
        /// </summary>
        string OSName { get; }

        /// <summary>
        /// Gets the browser version.
        /// </summary>
        string BrowserVersion { get; }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        string Device { get; }

        /// <summary>
        /// Gets the browser name.
        /// </summary>
        string BrowserName { get; }
    }
}
