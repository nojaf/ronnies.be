#r "nuget: Fun.Build, 0.3.8"
#r "nuget: Fake.IO.FileSystem, 6.0.0"

open System
open System.IO
open Fake.IO
open Fake.IO.FileSystemOperators
open Fun.Build

let initialize = stage "Init" { run "dotnet tool restore" }

pipeline "Build" {
    workingDir __SOURCE_DIRECTORY__
    initialize
    runIfOnlySpecified true
}

pipeline "Watch" {
    workingDir __SOURCE_DIRECTORY__
    initialize
    stage "main" {
        stage "emulator" {
            workingDir __SOURCE_DIRECTORY__
            run "firebase emulators:start --project=ronnies-210509"
        }
        stage "functions" {
            workingDir (__SOURCE_DIRECTORY__ </> "functions")
            run "bun i"
            run
                "dotnet fable ./Functions.fsproj -e .js --watch --fableLib \"@fable-org/fable-library-js\" --noCache --test:MSBuildCracker"
        }
        stage "app" {
            workingDir (__SOURCE_DIRECTORY__ </> "app")
            envVars [ "VITE_PLUGIN_FABLE_DEBUG", "1" ]
            run "bun i"
            run "bun run dev"
        }
        paralle
    }
    runIfOnlySpecified false
}

pipeline "Deploy" {
    workingDir __SOURCE_DIRECTORY__
    initialize
    stage "restore" { run "dotnet restore -tl" }
    stage "frontend" {
        workingDir (__SOURCE_DIRECTORY__ </> "app")
        run "bun i"
        run "bun run build"
    }
    stage "backend" {
        workingDir (__SOURCE_DIRECTORY__ </> "functions")
        run "bun i"
        run "dotnet fable -e .js --fableLib \"@fable-org/fable-library-js\" --noCache -c Release --test:MSBuildCracker"
    }
    stage "firebase" { run "firebase deploy --only hosting,functions" }
    runIfOnlySpecified true
}

tryPrintPipelineCommandHelp ()
