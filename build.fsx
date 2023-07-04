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
    runIfOnlySpecified false
}

pipeline "Watch" {
    workingDir __SOURCE_DIRECTORY__
    initialize
    stage "main" {
        paralle
        run "firebase emulators:start --project=ronnies-210509"
        run "dotnet fsi ./app/dev-server.fsx watch"
    }

    runIfOnlySpecified true
}

tryPrintPipelineCommandHelp ()
