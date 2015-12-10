// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a session detail item.
    /// </summary>
    public class SessionDetailItem
    {
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        [JsonProperty("automation_session")]
        public SessionDetail Item { get; set; }
    }
}
