// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing the <see cref="JsonSerializerContext"/> to use for JSON serialization. This class cannot be inherited.
/// </summary>
[ExcludeFromCodeCoverage]
[JsonSerializable(typeof(AutomatePlanStatus))]
[JsonSerializable(typeof(AutomationSession))]
[JsonSerializable(typeof(AutomationSessionDetail))]
[JsonSerializable(typeof(BrowserStackAutomateError))]
[JsonSerializable(typeof(EmptyRequest))]
[JsonSerializable(typeof(List<AutomationBuild>))]
[JsonSerializable(typeof(List<AutomationSession>))]
[JsonSerializable(typeof(List<Browser>))]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(ProjectDetail))]
[JsonSerializable(typeof(ProjectDetailItem))]
[JsonSerializable(typeof(RecycleAccessKeyResult))]
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(SetNameRequest))]
[JsonSerializable(typeof(SetSessionStatusRequest))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext
{
}
