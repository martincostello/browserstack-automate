# BrowserStack Automate REST API C# Client

## Build Status

[![Build status](https://img.shields.io/appveyor/ci/martincostello/browserstack-automate/master.svg)](https://ci.appveyor.com/project/martincostello/browserstack-automate) [![Coverage Status](https://img.shields.io/codecov/c/github/martincostello/browserstack-automate/master.svg)](https://codecov.io/github/martincostello/browserstack-automate)

[![Build History](https://ci-buildstats.azurewebsites.net/appveyor/chart/martincostello/browserstack-automate?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/browserstack-automate)

## Overview

This repository contains a C# client library and a NuGet package for the [BrowserStack Automate](https://www.browserstack.com/automate) REST API.

Features include:

 * Querying the status of a BrowserStack Automate plan.
 * Querying the available browsers.
 * Querying builds.
 * Querying projects.
 * Querying sessions.
 * Setting the status of a session.
 * Regenerating the API access key.

## Installation

```batchfile
Install-Package MartinCostello.BrowserStack.Automate
```

## Usage Examples

The following example shows a custom [xUnit.net](https://xunit.github.io/) ```[Trait]``` that checks for an available BrowserStack Automate session before running the test, otherwise it is skipped.

```csharp
namespace MyApp.Tests
{
    using System;
    using System.Configuration;
    using MartinCostello.BrowserStack.Automate;
    using Xunit;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RequiresBrowserStackAutomateAttribute : FactAttribute
    {
        public RequiresBrowserStackAutomateAttribute()
        {
            string userName = ConfigurationManager.AppSettings["BrowserStack:UserName"];
            string accessKey = ConfigurationManager.AppSettings["BrowserStack:AccessKey"];

            var client = new BrowserStackAutomateClient(userName, accessKey);
            var plan = client.GetStatusAsync().Result;

            if (plan.MaximumAllowedParallelSessions < 1 ||
                plan.ParallelSessionsRunning == plan.MaximumAllowedParallelSessions)
            {
                Skip = "No BrowserStack Automate sessions are currently available.";
            }
        }
    }
}
```

## Prerequisites

### Compilation and Debugging

 * Microsoft Windows 7 SP1 (or later);
 * Visual Studio (Community, Professional or Enterprise) 2015 (or later).

### Runtime

The following prerequisites are required to use the assembly:

 * Microsoft Windows 7 SP1 (or later);
 * Microsoft .NET Framework 4.5 (or later).

## Compilation

To compile the application, you can do any of the following:

 * Open ```src\MartinCostello.BrowserStack.Automate.sln``` in Visual Studio;
 * Run ```Build.cmd``` from the command prompt.

## License

Licensed under the Apache 2.0 license.
