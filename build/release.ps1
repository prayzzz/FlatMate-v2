function Test-ExitCode($code, $command) {
    if (!($code -eq 0)) {
        Write-Host "Error executing $command" -foregroundcolor Red
        Exit $code
    }
}

$isDryRun = $false
$dryrun = If ($isDryRun) {"--dry-run"} Else {""}

If ($isDryRun)
{
    Write-Host "Running in dry mode"
}

$branch = (git symbolic-ref --short -q HEAD) | Out-String
$branch = $branch.Trim()

If (!($branch -eq "master"))
{
    Write-Host "Current branch is not 'master'"
    Exit 1
}


################
## Apply Version
$date = Get-Date -Format yyyy.MM.dd;
$minutes = [math]::Round([datetime]::Now.TimeOfDay.TotalMinutes)
$version = "$date.$minutes"

$versionXmlFileName = "version.props"
$versionXmlPath =  Join-Path $(Get-Location) $versionXmlFileName
$versionXml = [xml](Get-Content $versionXmlPath)

Write-Host "Setting version prefix to $version"
$versionXml.Project.PropertyGroup.VersionPrefix = $version

Write-Host "Resetting version suffix"
$versionXml.Project.PropertyGroup.VersionSuffix = ""

$versionXml.Save($versionXmlPath)


############
## Git Magic
git pull $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git pull"

git add $versionXmlFileName $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git add"

git commit -m "Setting version to $version" $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git commit"

git tag $version $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git tag"

git push $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git push"


