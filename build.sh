#!/bin/sh
export artifacts=$(dirname "$(readlink -f "$0")")/artifacts
export configuration=Release

dotnet restore MartinCostello.BrowserStack.Automate.sln --verbosity minimal || exit 1
dotnet build src/MartinCostello.BrowserStack.Automate/MartinCostello.BrowserStack.Automate.csproj --output $artifacts --configuration $configuration --framework "netstandard1.3" || exit 1
dotnet test tests/MartinCostello.BrowserStack.Automate.Tests/MartinCostello.BrowserStack.Automate.Tests.csproj --output $artifacts --configuration $configuration --framework "netcoreapp1.0" || exit 1
