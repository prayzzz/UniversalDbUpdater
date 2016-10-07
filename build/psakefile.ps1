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
    Apply-Version("src/UniversalDbUpdater.Common/project.json")
    Apply-Version("src/UniversalDbUpdater.MsSql/project.json")
    Apply-Version("src/UniversalDbUpdater.MySql/project.json")
}

task Dotnet-Build -depends Dotnet-Restore, Set-Version {
    exec { dotnet build "src/UniversalDbUpdater/" --configuration $config }
}

task Dotnet-Test -depends Dotnet-Build {
    Run-Test("UniversalDbUpdater.MsSql.Test")
    Run-Test("UniversalDbUpdater.MySql.Test")    
}

task Dotnet-Publish -depends Dotnet-Build, Dotnet-Test {
    exec { dotnet publish "src/UniversalDbUpdater/" --configuration $config --no-build }
}

task Dotnet-Pack -depends Dotnet-Build {
    exec { dotnet pack "src/UniversalDbUpdater/" --configuration $config --no-build --output $outputFolder }
    $file = $outputFolder + "UniversalDbUpdater.$version.nupkg"
    exec { nuget push $file $env:MYGET_APIKEY -Source https://www.myget.org/F/russianbee/api/v2/package }

    exec { dotnet pack "src/UniversalDbUpdater.Common/" --configuration $config --no-build --output $outputFolder }
    $file = $outputFolder + "UniversalDbUpdater.Common.$version.nupkg"
    exec { nuget push $file $env:MYGET_APIKEY -Source https://www.myget.org/F/russianbee/api/v2/package }

    exec { dotnet pack "src/UniversalDbUpdater.MsSql/" --configuration $config --no-build --output $outputFolder }
    $file = $outputFolder + "UniversalDbUpdater.MsSql.$version.nupkg"
    exec { nuget push $file $env:MYGET_APIKEY -Source https://www.myget.org/F/russianbee/api/v2/package }

    exec { dotnet pack "src/UniversalDbUpdater.MySql/" --configuration $config --no-build --output $outputFolder }
    $file = $outputFolder + "UniversalDbUpdater.MySql.$version.nupkg"
    exec { nuget push $file $env:MYGET_APIKEY -Source https://www.myget.org/F/russianbee/api/v2/package }
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

function Apply-Version ($file) {
    $project = ConvertFrom-Json -InputObject (Gc $file -Raw)
    $project.version = $version
    $project | ConvertTo-Json -depth 100 | Out-File $file
}

function Run-Test ($project) {
    if ($isTeamCity) {
        TeamCity-TestSuiteStarted $project 
    }
    
    exec { dotnet test "test/$project/" --configuration $config --no-build --result "TestResult.$project.xml" }

    if ($isTeamCity) {
        TeamCity-TestSuiteFinished $project 
    }        
}