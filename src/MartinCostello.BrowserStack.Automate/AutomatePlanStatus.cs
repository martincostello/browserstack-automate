// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing the current status of a <c>BrowserStack</c> Automate plan.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Parallel Sessions: {" + nameof(ParallelSessionsRunning) + "}/{" + nameof(MaximumAllowedParallelSessions) + "}")]
    public class AutomatePlanStatus
    {
        /// <summary>
        /// Gets or sets the current plan.
        /// </summary>
        [JsonProperty("automate_plan")]
        public string AutomatePlan { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed number of parallel sessions.
        /// </summary>
        [JsonProperty("parallel_sessions_max_allowed")]
        public int MaximumAllowedParallelSessions { get; set; }

        /// <summary>
        /// Gets or sets the number of parallel sessions currently running.
        /// </summary>
        [JsonProperty("parallel_sessions_running")]
        public int ParallelSessionsRunning { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed number of parallel sessions for the team.
        /// </summary>
        [JsonProperty("team_parallel_sessions_max_allowed")]
        public int TeamMaximumAllowedParallelSessions { get; set; }

        /// <summary>
        /// Gets or sets the number of parallel sessions currently queued.
        /// </summary>
        [JsonProperty("queued_sessions")]
        public int QueuedParallelSessions { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed number of parallel sessions that can be queued.
        /// </summary>
        [JsonProperty("queued_sessions_max_allowed")]
        public int MaximumQueuedParallelSessions { get; set; }
    }
}
