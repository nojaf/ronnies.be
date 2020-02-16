#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"
#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open System.IO

let root = __SOURCE_DIRECTORY__
let clientDir = Path.Combine(root,"src","client-react")
let serverDir = Path.Combine(root, "src", "server")
let serverProject = "Ronnies.Server.fsproj"
let serverProjectPath = Path.Combine(serverDir, serverProject)
let artifactPath = Path.Combine(__SOURCE_DIRECTORY__, "artifacts")
let wwwroot = Path.Combine(artifactPath,  "wwwroot")
let sln = Path.Combine(__SOURCE_DIRECTORY__, "ronnies.be.sln")


Target.create "Clean"
    (fun _ ->
        !! "src/server/bin" ++ "src/server/obj" |> Seq.iter Shell.cleanDir
        Shell.cleanDir artifactPath
    )

Target.create "Install"
    (fun _ -> DotNet.restore (DotNet.Options.withWorkingDirectory root) sln)

Target.create "InstallClient"
    (fun _ ->
        Yarn.installPureLock (fun o -> { o with WorkingDirectory = clientDir })
    )

Target.create "BuildClient" (fun _ ->
    Yarn.exec "build" (fun opt -> { opt with WorkingDirectory = clientDir })
)

Target.create "Build"
    (fun _ ->

    if not (File.exists wwwroot) then
        Shell.mkdir wwwroot

    Shell.copyRecursive "src/RonniesClient/output/" wwwroot false |> ignore

    DotNet.publish (fun p ->
        { p with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some artifactPath
        })
        serverProjectPath
)

Target.create "Watch" (fun _ ->
    let fableWatch =
        async {
            Yarn.exec "start" (fun opt -> { opt with WorkingDirectory = clientDir;  })
        }
        
    let serverWatch =
        async {
            let fcswatch =
                Command.RawCommand("fcswatch", Arguments.OfArgs ["--project-file"; serverProjectPath; "--logger-level"; "normal"])
                |> CreateProcess.fromCommand
                |> CreateProcess.withWorkingDirectory serverDir
                |> Proc.startAndAwait
                |> Async.Ignore
            do! fcswatch
        }

    Async.Parallel [fableWatch; serverWatch]
    |> Async.RunSynchronously
    |> ignore
)

// Build order
"Clean" ==> "Install" ==> "InstallClient" ==> "BuildClient" ==> "Build"

"Watch"
    <== [ "InstallClient" ]

// start build
Target.runOrDefault "Build"
