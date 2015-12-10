// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// Defines an item returned from the <c>BrowserStack</c> Automate API.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public interface IAutomateItem<T>
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        T Item { get; }
    }
}
