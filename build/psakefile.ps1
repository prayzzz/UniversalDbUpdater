properties {
    Import-Module psake-contrib/teamcity.psm1

    $config = "Debug"
    $outputFolder = "dist/"

    $date = Get-Date -Format yyyy.MM.dd;
    $seconds = [math]::Round([datetime]::Now.TimeOfDay.TotalMinutes)
    $version = "$date.$seconds"

    $isTeamcity = $env:TEAMCITY_VERSION
    if ($isTeamCity) { TeamCity-SetBuildNumber $version }

    # Change to root directory
    Set-Location "../"

    Write-Host "Building $version"
}

FormatTaskName {
   ""
   ""
   Write-Host "Executing Task: $taskName" -foregroundcolor Cyan
}

# Alias

task Restore -depends Dotnet-Restore {
}

task Build -depends Dotnet-Build {
}

task Test -depends Dotnet-Test {
}

task Pack -depends Dotnet-Pack {
}

task Publish -depends Zip-Dotnet-Publish {
}

# Tasks

task Dotnet-Restore {
    exec { dotnet restore }
}

task Set-Version {
    Apply-Version("src/UniversalDbUpdater/project.json")
    Apply-Version("src/UniversalDbUpdater/appsettings.json")
}

task Dotnet-Build -depends Dotnet-Restore, Set-Version {
    exec { dotnet build "src/UniversalDbUpdater/" --configuration $config }
}

task Dotnet-Test -depends Dotnet-Build {
    Run-Test("UniversalDbUpdater.Test")
}

task Dotnet-Publish -depends Dotnet-Build, Dotnet-Test {
    exec { dotnet publish "src/UniversalDbUpdater/" --configuration $config --no-build }
}

task Dotnet-Pack -depends Dotnet-Build {
    Pack-Project("UniversalDbUpdater")
}

task Zip-Dotnet-Publish -depends Dotnet-Publish {
    exec {
        $source = "src/UniversalDbUpdater/bin/$config/netcoreapp1.0/publish"
        $destinationFile = "UniversalDbUpdater-$version.zip"
        $destinationPath = $outputFolder + $destinationFile

        If(!(Test-path $outputFolder)) {
            mkdir $outputFolder
        }

        If(Test-path $destinationPath) {
            Remove-item $destinationPath
        }

        Add-Type -assembly "system.io.compression.filesystem"
        [io.compression.zipfile]::CreateFromDirectory($Source, $destinationPath)

        Write-Host "Created $destinationPath"
    }
}

function Pack-Project($project){
    exec { dotnet pack "src/$project/" --configuration $config --no-build --output $outputFolder }

    $file = $outputFolder + "$project.$version.nupkg"
    exec { nuget push $file $env:NUGET_APIKEY -Source https://www.nuget.org/api/v2/package }
}

function Apply-Version ($file) {
    $project = ConvertFrom-Json -InputObject (Gc $file -Raw)
    $project.version = $version
    $project | ConvertTo-Json -depth 100 | Out-File $file
}

function Run-Test ($project) {
    if ($isTeamCity) {
        TeamCity-TestSuiteStarted $project
    }

    exec { dotnet test "test/$project/" --configuration $config --result "TestResult.$project.xml" }

    if ($isTeamCity) {
        TeamCity-TestSuiteFinished $project
    }
}