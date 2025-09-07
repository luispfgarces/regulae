param ([switch] $OnlyRunUnitTests)

$globalTools = dotnet tool list -g
$reportGeneratorTool = $globalTools | Select-String -Pattern "dotnet-reportgenerator-globaltool"

If (!$reportGeneratorTool) {
    dotnet tool install dotnet-reportgenerator-globaltool --global --ignore-failed-sources
}

dotnet restore .\regulae.sln

$reportTimestamp = [DateTime]::UtcNow.ToString("yyyyMMdd_hhmmss")
$currentDir = (Get-Location).Path
$coverageDir = "$currentDir\\coverage-outputs\\$reportTimestamp\\"

if ($OnlyRunUnitTests) {
    dotnet test .\regulae.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1 --filter 'Category=Unit' -v normal
} else {
    dotnet test .\regulae.sln --collect:"XPlat Code Coverage" --results-directory:"$coverageDir" -m:1 -v normal
}

reportgenerator -reports:"$($coverageDir)*\\*.xml" -targetdir:"$($coverageDir)" -reporttypes:Cobertura

$coverageFile = "$($coverageDir)*.xml"
$coverageReportFolder = "$($currentDir)\\coverage-outputs\\report\\"
reportgenerator -reports:$coverageFile -targetdir:$coverageReportFolder
# start "$($coverageReportFolder)index.htm"