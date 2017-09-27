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
$version = [System.DateTime]::Now.ToString("yyyy.MM.dd") + "." + [System.Math]::Round([System.DateTime]::Now.TimeOfDay.TotalMinutes)

$versionXmlFileName = "version.props"
$versionXmlPath =  Join-Path $(Get-Location) $versionXmlFileName
$versionXml = [xml](Get-Content $versionXmlPath)

Write-Host "Setting version to $version"
$versionXml.Project.PropertyGroup.Version = $version

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

git push --tags $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git push --tags"

git push $dryrun | Out-Null
Test-ExitCode $LASTEXITCODE "git push"


