param(
    [Parameter(Mandatory=$false)][bool]   $RestorePackages  = $false,
    [Parameter(Mandatory=$false)][string] $Configuration    = "Release",
    [Parameter(Mandatory=$false)][string] $VersionSuffix    = "",
    [Parameter(Mandatory=$false)][string] $OutputPath       = "",
    [Parameter(Mandatory=$false)][bool]   $PatchVersion     = $false,
    [Parameter(Mandatory=$false)][bool]   $RunTests         = $true,
	[Parameter(Mandatory=$false)][bool]   $CreatePackages   = $true
)

$ErrorActionPreference = "Stop"

$solutionPath  = Split-Path $MyInvocation.MyCommand.Definition
$getDotNet     = Join-Path $solutionPath "tools\install.ps1"
$dotnetVersion = "1.0.0-preview2-003121"

if ($OutputPath -eq "") {
    $OutputPath = "$(Convert-Path "$PSScriptRoot")\artifacts"
}

$env:DOTNET_INSTALL_DIR = "$(Convert-Path "$PSScriptRoot")\.dotnetcli"

if ($env:CI -ne $null) {

    $RestorePackages = $true
    $PatchVersion = $true

	if (($VersionSuffix -eq "" -and $env:APPVEYOR_REPO_TAG -eq "false" -and $env:APPVEYOR_BUILD_NUMBER -ne "") -eq $true) {

	    $LastVersionBuild = (Get-Content ".\releases.txt" | Select -Last 1)
		$LastVersion = New-Object -TypeName System.Version -ArgumentList $LastVersionBuild
		$ThisVersion = $env:APPVEYOR_BUILD_NUMBER -as [int]
		$ThisBuildNumber = $ThisVersion - $LastVersion.Build

	    $VersionSuffix = "beta-" + $ThisBuildNumber.ToString("0000")
	}
}

if (!(Test-Path $env:DOTNET_INSTALL_DIR)) {
    mkdir $env:DOTNET_INSTALL_DIR | Out-Null
    $installScript = Join-Path $env:DOTNET_INSTALL_DIR "install.ps1"
    Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" -OutFile $installScript
    & $installScript -Version "$dotnetVersion" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath
}

$env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
$dotnet   = "$env:DOTNET_INSTALL_DIR\dotnet"

function DotNetRestore { param([string]$Project)
    & $dotnet restore $Project --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }
}

function DotNetBuild { param([string]$Project, [string]$Configuration, [string]$Framework, [string]$VersionSuffix)
    if ($VersionSuffix) {
        & $dotnet build $Project --output (Join-Path $OutputPath $Framework) --framework $Framework --configuration $Configuration --version-suffix "$VersionSuffix"
    } else {
        & $dotnet build $Project --output (Join-Path $OutputPath $Framework) --framework $Framework --configuration $Configuration
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

function DotNetTest { param([string]$Project, [string]$Framework)
    & $dotnet test $Project --output (Join-Path $OutputPath $Framework) --framework $Framework
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test failed with exit code $LASTEXITCODE"
    }
}

function DotNetPack { param([string]$Project, [string]$Configuration, [string]$VersionSuffix)
    if ($VersionSuffix) {
        & $dotnet pack $Project --output $OutputPath --configuration $Configuration --version-suffix "$VersionSuffix" --no-build
    } else {
        & $dotnet pack $Project --output $OutputPath --configuration $Configuration --no-build
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}

if ($PatchVersion -eq $true) {

    $gitRevision = (git rev-parse HEAD | Out-String).Trim()
    $gitBranch   = (git rev-parse --abbrev-ref HEAD | Out-String).Trim()
    $timestamp   = [DateTime]::UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")

    $assemblyVersion = Get-Content ".\AssemblyVersion.cs" -Raw
    $assemblyVersionWithMetadata = "{0}[assembly: AssemblyMetadata(""CommitHash"", ""{1}"")]`r`n[assembly: AssemblyMetadata(""CommitBranch"", ""{2}"")]`r`n[assembly: AssemblyMetadata(""BuildTimestamp"", ""{3}"")]" -f $assemblyVersion, $gitRevision, $gitBranch, $timestamp

    Set-Content ".\AssemblyVersion.cs" $assemblyVersionWithMetadata -Encoding utf8
}

$projects = @(
    (Join-Path $solutionPath "src\MartinCostello.BrowserStack.Automate\project.json")
)

$testProjects = @(
    (Join-Path $solutionPath "tests\MartinCostello.BrowserStack.Automate.Tests\project.json")
)

$packageProjects = @(
    (Join-Path $solutionPath "src\MartinCostello.BrowserStack.Automate\project.json")
)

$restoreProjects = @(
    (Join-Path $solutionPath "src\MartinCostello.BrowserStack.Automate\project.json"),
	(Join-Path $solutionPath "tests\MartinCostello.BrowserStack.Automate.Tests\project.json")
)

if ($RestorePackages -eq $true) {
    Write-Host "Restoring NuGet packages for $($restoreProjects.Count) projects..." -ForegroundColor Green
    ForEach ($project in $restoreProjects) {
        DotNetRestore $project
    }
}

Write-Host "Building $($projects.Count) projects..." -ForegroundColor Green
ForEach ($project in $projects) {
    DotNetBuild $project $Configuration "netstandard1.3" $VersionSuffix
	DotNetBuild $project $Configuration "net451" $VersionSuffix
}

if ($RunTests -eq $true) {
    Write-Host "Testing $($testProjects.Count) project(s)..." -ForegroundColor Green
    ForEach ($project in $testProjects) {
        DotNetTest $project "netcoreapp1.0"
		DotNetTest $project "net451"
    }
}

if ($CreatePackages -eq $true) {
    Write-Host "Creating $($packageProjects.Count) package(s)..." -ForegroundColor Green
    ForEach ($project in $packageProjects) {
        DotNetPack $project $Configuration $VersionSuffix
    }
}

if ($PatchVersion -eq $true) {
    Set-Content ".\AssemblyVersion.cs" $assemblyVersion.Trim() -Encoding utf8
}
