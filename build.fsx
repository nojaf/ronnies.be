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
            workingDir (__SOURCE_DIRECTORY__ </> "Functions")
            run
                "dotnet fable ./Functions.fsproj -e .js --watch --fableLib \"@fable-org/fable-library-js\" --noCache --test:MSBuildCracker"
        }
        stage "app" {
            workingDir (__SOURCE_DIRECTORY__ </> "App")
            // run
            //     "dotnet fable ./App.fsproj -e .jsx --watch -o ./out --fableLib \"@fable-org/fable-library-js\" --noReflection --exclude \"Nojaf.Fable.React.Plugin\" --test:MSBuildCracker"
            run "bun run dev"
            paralle
        }
        paralle
    }
    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()
