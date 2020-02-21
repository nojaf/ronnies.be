#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"
#load ".fake/build.fsx/intellisense_lazy.fsx"
#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.JavaScript
open System.IO
open Fantomas
open Fantomas.FormatConfig

let root = __SOURCE_DIRECTORY__
let clientPath = Path.Combine(root,"src","client")
let serverPath = Path.Combine(root, "src", "server")
let serverProject = "Ronnies.Server.fsproj"
let serverProjectPath = Path.Combine(serverPath, serverProject)
let artifactPath = Path.Combine(__SOURCE_DIRECTORY__, "artifacts")
let wwwroot = Path.Combine(artifactPath,  "wwwroot")
let sln = Path.Combine(__SOURCE_DIRECTORY__, "ronnies.be.sln")
let yarnSetParams = (fun (c: Yarn.YarnParams) -> { c with WorkingDirectory = clientPath })
let fsharpFiles = !!"src/**/*.fs" -- "src/**/obj/**" -- "src/**/node_modules/**" -- "src/**/.fable/**"
let javaScriptFiles = ["src/**/*.js";"src/*.js"]

let fantomasConfig =
    match CodeFormatter.ReadConfiguration(root </> "fantomas-config.json") with
    | Success c -> c
    | _ -> failwith "Cannot parse fantomas-config.json"

module Azure =
    let az parameters =
        let azPath = ProcessUtils.findPath [] "az"
        CreateProcess.fromRawCommand azPath parameters
        |> Proc.run
        |> ignore

    let func parameters =
        let funcPath = ProcessUtils.findPath [] "func"
        CreateProcess.fromRawCommand funcPath parameters
        |> CreateProcess.withWorkingDirectory serverPath
        |> Proc.run
        |> ignore

Target.create "Clean"
    (fun _ ->
        !! "src/server/bin" ++ "src/server/obj" |> Seq.iter Shell.cleanDir
        Shell.cleanDir artifactPath
    )

Target.create "Format" (fun _ ->
    fsharpFiles
    |> FakeHelpers.formatCode fantomasConfig
    |> Async.RunSynchronously
    |> printfn "Formatted F# files: %A"

    javaScriptFiles
    |> List.iter (fun js -> Yarn.exec (sprintf "prettier %s --write" js) yarnSetParams))

Target.create "CheckCodeFormat" (fun _ ->
    let result =
        fsharpFiles
        |> FakeHelpers.checkCode fantomasConfig
        |> Async.RunSynchronously

    if result.IsValid then
        Trace.log "No files need formatting with Fantomas"
    elif result.NeedsFormatting then
        Trace.log "The following files need formatting:"
        List.iter Trace.log result.Formatted
        failwith "Some files need formatting, check output for more info"
    else
        Trace.logf "Errors while formatting: %A" result.Errors

    javaScriptFiles
    |> List.iter (fun js -> Yarn.exec (sprintf "prettier %s --check" js) yarnSetParams))

Target.create "Install"
    (fun _ -> DotNet.restore (DotNet.Options.withWorkingDirectory root) sln)

Target.create "InstallClient"
    (fun _ ->
        Yarn.installPureLock (fun o -> { o with WorkingDirectory = clientPath })
    )

Target.create "BuildClient" (fun _ ->
    Yarn.exec "build" (fun opt -> { opt with WorkingDirectory = clientPath })
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
    let fableOutput output =
        Trace.tracefn "%s" output
        if output = "fable: Watching..." then Yarn.exec "start" yarnSetParams

    let fableError output =
        Trace.traceErrorfn "\n%s\n" output

    let compileFable =
        CreateProcess.fromRawCommand Yarn.defaultYarnParams.YarnFilePath [ "fable"; "-d"; "--watch" ]
        |> CreateProcess.withWorkingDirectory clientPath
        |> CreateProcess.redirectOutput
        |> CreateProcess.withOutputEventsNotNull fableOutput fableError
        |> Proc.startAndAwait
        |> Async.Ignore

    let stopFunc() = System.Diagnostics.Process.GetProcessesByName("func") |> Seq.iter (fun p -> p.Kill())

    let rec startFunc() =
        let dirtyWatcher: System.IDisposable ref = ref null

        let watcher =
            !!(serverPath </> "*.fs") ++ (serverPath </> "*.fsproj")
            |> ChangeWatcher.run (fun changes ->
                printfn "FILE CHANGE %A" changes
                if !dirtyWatcher <> null then
                    (!dirtyWatcher).Dispose()
                    stopFunc()
                    startFunc())

        dirtyWatcher := watcher

        Azure.func ["start"]

    let runAzureFunction = async { startFunc() }

    Async.Parallel [ runAzureFunction; compileFable ]
    |> Async.Ignore
    |> Async.RunSynchronously)


// Build order
"InstallClient" ==> "Format"

"Clean" ==> "Install" ==> "InstallClient" ==> "BuildClient" ==> "Build"

"Watch"
    <== [ "InstallClient" ]

// start build
Target.runOrDefault "Build"
