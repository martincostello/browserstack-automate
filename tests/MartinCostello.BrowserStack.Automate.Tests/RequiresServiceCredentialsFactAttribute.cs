// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace MartinCostello.BrowserStack.Automate
{
    /// <summary>
    /// A test that requires service credentials to be configured as environment variables. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class RequiresServiceCredentialsFactAttribute : FactAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresServiceCredentialsFactAttribute"/> class.
        /// </summary>
        public RequiresServiceCredentialsFactAttribute()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BrowserStack_UserName")) ||
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BrowserStack_AccessKey")))
            {
                Skip = "No BrowserStack service credentials are configured.";
            }
        }
    }
}
