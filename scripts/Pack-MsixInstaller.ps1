#! /usr/bin/env pwsh
#Requires -Version 7.4

using namespace System.IO;
using namespace System.IO.Compression;
using namespace System.Xml;

[CmdletBinding()]
param (
  [Parameter(Mandatory)]
  [string]
  $Path
)

$Path = [Path]::GetFullPath($Path)
$Private:Root = Join-Path $PSScriptRoot '..'
$Private:Zip = [ZipFile]::OpenRead($Path)
$Private:ManifestEntry = $Private:Zip.GetEntry('AppxManifest.xml')
$Private:ManifestStream = $Private:ManifestEntry.Open()
$Private:ManifestReader = [StreamReader]::new($Private:ManifestStream)
$Private:Manifest = [xml]$Private:ManifestReader.ReadToEnd()

if ([string]::IsNullOrWhiteSpace($env:MSIX_Version)) {
  $env:MSIX_Version = $Manifest.Package.Identity.Version
}
if ([string]::IsNullOrWhiteSpace($env:MSIX_Arch)) {
  $env:MSIX_Arch = $Manifest.Package.Identity.ProcessorArchitecture
}
$env:MSIX_Arch = $env:MSIX_Arch.ToLower()
$Private:Platforms = @{
  neutral = 'any'
  x86     = 'x86'
  x64     = 'x64'
  arm     = 'arm'
  arm64   = 'arm64'
}

$Private:ArtifactsPath = Join-Path $Root 'artifacts'

dotnet publish $Private:Root `
  --configuration 'Release' `
  --artifacts-path $Private:ArtifactsPath `
  --arch $Private:Platforms[$env:MSIX_Arch] `
  --graph `
  -property:"MsixFilePath=$Path" `
  -property:"Version=$env:MSIX_Version" `
  -property:"FileVersion=$env:MSIX_Version" `
  -property:"InformationalVersion=$env:MSIX_Version"

$Private:InstallerPath = Join-Path $Private:ArtifactsPath 'publish' 'MsixInstaller' "release_win-$($Private:Platforms[$env:MSIX_Arch])" 'MsixInstaller.exe'
$Private:InstallerPath = [Path]::GetFullPath($Private:InstallerPath)
$Private:Hash = Get-FileHash -LiteralPath $Private:InstallerPath
$Private:Algorithm = $Private:Hash.Algorithm
$Private:Checksum = $Private:Hash.Hash
Write-Output "path=$Private:InstallerPath" >> $env:GITHUB_OUTPUT
Write-Output "algorithm=$Private:Algorithm" >> $env:GITHUB_OUTPUT
Write-Output "checksum=$Private:Checksum" >> $env:GITHUB_OUTPUT
