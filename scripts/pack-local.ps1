param(
  [string]$GameDist = "",
  [string]$OutDir = "publish"
)

$ErrorActionPreference = "Stop"

if ($GameDist -ne "" -and (Test-Path "$GameDist\index.html")) {
  Write-Host "Refreshing wwwroot from $GameDist"
  if (Test-Path wwwroot) { Remove-Item -Recurse -Force wwwroot }
  Copy-Item -Recurse $GameDist wwwroot
}

if (-not (Test-Path "wwwroot\index.html")) {
  throw "wwwroot/index.html missing. Pass -GameDist path\to\NEXTICON-FC\dist or sync wwwroot first."
}

dotnet publish src\NextIconFc.WebView\NextIconFc.WebView.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -o $OutDir

New-Item -ItemType Directory -Force -Path "$OutDir\wwwroot" | Out-Null
Copy-Item -Path "wwwroot\*" -Destination "$OutDir\wwwroot" -Recurse -Force
Write-Host "Done: $OutDir\NextIconFC.exe"
