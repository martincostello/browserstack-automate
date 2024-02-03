// Copyright (c) Martin Costello, 2015. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.BrowserStack.Automate;

/// <summary>
/// A class representing a session.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{" + nameof(Name) + "}")]
public class Session : IBrowserInfo
{
    /// <summary>
    /// Gets or sets the reason.
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed Id of the session.
    /// </summary>
    [JsonPropertyName("hashed_id")]
    public string HashedId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration of the session in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets the device.
    /// </summary>
    [JsonPropertyName("device")]
    public string Device { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the browser version.
    /// </summary>
    [JsonPropertyName("browser_version")]
    public string BrowserVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OS version.
    /// </summary>
    [JsonPropertyName("os_version")]
    public string OSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Operating System name.
    /// </summary>
    [JsonPropertyName("os")]
    public string OSName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    [JsonPropertyName("project_name")]
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the build name.
    /// </summary>
    [JsonPropertyName("build_name")]
    public string BuildName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URI of the logs.
    /// </summary>
    [JsonPropertyName("logs")]
    public string LogsUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the browser name.
    /// </summary>
    [JsonPropertyName("browser")]
    public string BrowserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the browser URL.
    /// </summary>
    [JsonPropertyName("browser_url")]
    public string BrowserUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public URL.
    /// </summary>
    [JsonPropertyName("public_url")]
    public string PublicUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Appium logs URL.
    /// </summary>
    [JsonPropertyName("appium_logs_url")]
    public string AppiumLogsUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the video URL.
    /// </summary>
    [JsonPropertyName("video_url")]
    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to view the browser's console logs.
    /// </summary>
    [JsonPropertyName("browser_console_logs_url")]
    public string BrowserConsoleLogsUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to view the browser's HAR logs.
    /// </summary>
    [JsonPropertyName("har_logs_url")]
    public string HarLogsUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to view the Selenium logs.
    /// </summary>
    [JsonPropertyName("selenium_logs_url")]
    public string SeleniumLogsUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL to view the Selenium 4 telemetry logs, if enabled.
    /// </summary>
    [JsonPropertyName("selenium_telemetry_logs_url")]
    public string SeleniumTelemetryLogsUrl { get; set; } = string.Empty;
}
