properties {
    Import-Module psake-contrib/teamcity.psm1


    $config = "Debug"    

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

task Test -depends Dotnet-Test{    
}

task Publish -depends Zip-Dotnet-Publish {    
}

# Tasks

task Dotnet-Restore {
    exec { dotnet restore }
}

task Dotnet-Build -depends Dotnet-Restore {
    exec { dotnet build "src/UniversalDbUpdater/" --configuration $config }
}

task Dotnet-Test -depends Dotnet-Restore {
    if ($isTeamCity) {
        TeamCity-TestSuiteStarted mobtima.Common.Test 
        exec { dotnet test "test/UniversalDbUpdater.MsSql.Test/" --configuration $config --result TestResult.UniversalDbUpdater.MsSql.xml }
        TeamCity-TestSuiteFinished mobtima.Common.Test 

        TeamCity-TestSuiteStarted mobtima.Domain.Test 
        exec { dotnet test "test/UniversalDbUpdater.MySql.Test/" --configuration $config --result TestResult.UniversalDbUpdater.MySql.xml }
        TeamCity-TestSuiteFinished mobtima.Domain.Test
    } 
    else {
        exec { dotnet test "test/UniversalDbUpdater.MsSql.Test/" --configuration $config --result TestResult.UniversalDbUpdater.MsSql.xml }
        exec { dotnet test "test/UniversalDbUpdater.MySql.Test/" --configuration $config --result TestResult.UniversalDbUpdater.MySql.xml }
    }    
}

task Set-Version {
    $project = ConvertFrom-Json -InputObject (Gc "src/UniversalDbUpdater/project.json" -Raw)
    $project.version = $version
    $project | ConvertTo-Json -depth 100 | Out-File "src/UniversalDbUpdater/project.json"


    $settings = ConvertFrom-Json -InputObject (Gc "src/UniversalDbUpdater/appsettings.json" -Raw)
    $settings.version = $version
    $settings | ConvertTo-Json -depth 100 | Out-File "src/UniversalDbUpdater/appsettings.json"
}

task Dotnet-Publish -depends Dotnet-Restore, Dotnet-Test, Set-Version {
    exec { dotnet publish "src/UniversalDbUpdater/" --configuration $config }
} 

task Zip-Dotnet-Publish -depends Dotnet-Publish {
    exec {        
        $source = "src/UniversalDbUpdater/bin/$config/netcoreapp1.0/publish"
        $destinationFolder = "dist/"
        $destinationFile = "UniversalDbUpdater-$version.zip"
        $destinationPath = $destinationFolder + $destinationFile        

        If(!(Test-path $destinationFolder)) {            
            mkdir $destinationFolder
        }        

        If(Test-path $destinationPath) {            
            Remove-item $destinationPath
        }

        Add-Type -assembly "system.io.compression.filesystem"
        [io.compression.zipfile]::CreateFromDirectory($Source, $destinationPath) 

        Write-Host "Created $destinationPath"
    }
}