# Able to receive two params
# 1 - Files source
# 2 - Destination temp folder name and zip name
$source = $args[0]
$destination = $args[1]

# Default source folder
if (!$source) {
    $source = '.\bin\Debug\*'
}

# Default destination folder
if (!$destination) {
    $destination = 'deploy'
}

Remove-Item -Path $destination -Recurse -Force -ErrorAction SilentlyContinue

$pathDestination = $(mkdir $destination).FullName

$pathDestinationLibs = $(mkdir "$destination\libs").FullName

Copy-Item $source $pathDestinationLibs -Recurse -ErrorAction SilentlyContinue

$ShortcutLocation = "$pathDestination\Votacao.lnk"
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut($ShortcutLocation)
$Shortcut.TargetPath = 'powershell.exe'
$Shortcut.Arguments = '-Command "Start-Process .\libs\Votacao.exe -WorkingDirectory .\libs -Verb Runas"'
$Shortcut.Save()

Compress-Archive -Path $pathDestination -DestinationPath "$destination.zip" -Update

Remove-Item -Path $pathDestination -Recurse -Force -ErrorAction SilentlyContinue

$fileZip = $(Get-ChildItem "$destination.zip").FullName
Write-Output ""
Write-Host "Arquivo $fileZip gerado." -ForegroundColor Green
Write-Output ""
