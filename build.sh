#!/bin/sh
dotnet restore --verbosity minimal
dotnet build src/MartinCostello.BrowserStack.Automate
dotnet test tests/MartinCostello.BrowserStack.Automate.Tests
dotnet pack src/MartinCostello.BrowserStack.Automate
