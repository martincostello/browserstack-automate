# ![Alt text](browserstack-logo.png) BrowserStack Automate REST API .NET Client

## Build Status

[![Build status](https://img.shields.io/appveyor/ci/martincostello/browserstack-automate/master.svg)](https://ci.appveyor.com/project/martincostello/browserstack-automate)

[![NuGet](https://buildstats.info/nuget/MartinCostello.BrowserStack.Automate)](http://www.nuget.org/packages/MartinCostello.BrowserStack.Automate)

[![Build History](https://buildstats.info/appveyor/chart/martincostello/browserstack-automate?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/browserstack-automate)

## Overview

This repository contains a .NET client library/NuGet package for the [BrowserStack Automate](https://www.browserstack.com/automate) REST API.

Features include:

 * Querying the status of a BrowserStack Automate plan.
 * Querying the available browsers.
 * Querying and deleting builds.
 * Querying and deleting projects.
 * Querying and deleting sessions.
 * Querying session log.
 * Setting the status of a session.
 * Regenerating the API access key.

The assembly supports .NET Core 1.0 and .NET Framework 4.5.1.

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
            string userName = Environment.GetEnvironmentVariable("BrowserStack_UserName");
            string accessKey = Environment.GetEnvironmentVariable("BrowserStack_AccessKey");

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

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/browserstack-automate/issues).

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/browserstack-automate): https://github.com/martincostello/browserstack-automate.git

## License

This project is licensed under the [Apache 2.0](https://github.com/martincostello/browserstack-automate/blob/master/LICENSE) license.

## Building and Testing

To build and test the assembly run one of the following set of commands:

**Linux/OS X**

```sh
EXPORT BrowserStack_UserName  = "MyUserName"
EXPORT BrowserStack_AccessKey = "MyAccessKey"
./build.sh
```

**Windows**

```powershell
$env:BrowserStack_UserName  = "MyUserName"
$env:BrowserStack_AccessKey = "MyAccessKey"
.\Build.ps1
```

_If you do not have a BrowserStack Automate access key you can still just run the build script and the integration tests that require credentials will be skipped._
