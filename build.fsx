#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"
#nowarn "52"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.JavaScript
open System.IO

let root = __SOURCE_DIRECTORY__
let clientPath = Path.Combine(root,"src","client")
let serverPath = Path.Combine(root, "src", "server")
let serverProject = "server.fsproj"
let serverProjectPath = Path.Combine(serverPath, serverProject)
let artifactPath = Path.Combine(__SOURCE_DIRECTORY__, "artifacts")
let wwwroot = Path.Combine(artifactPath,  "wwwroot")
let sln = Path.Combine(__SOURCE_DIRECTORY__, "ronnies.be.sln")
let yarnSetParams = (fun (c: Yarn.YarnParams) -> { c with WorkingDirectory = clientPath })
let fsharpFiles = !!"src/**/*.fs" -- "src/**/obj/**" -- "src/**/node_modules/**" -- "src/**/.fable/**"
let javaScriptFiles = ["snowpack.config.js"]

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
        Shell.rm_rf (clientPath </> ".fable")
        Shell.rm_rf (clientPath </> ".build")
        Shell.rm_rf (clientPath </> "build")
        Shell.rm_rf (clientPath </> "src" </> "bin")
    )

Target.create "Format" (fun _ ->
    let result = DotNet.exec id "fantomas" "src -r"
    if not result.OK then
        printfn "Errors while formatting all files: %A" result.Messages
        
    javaScriptFiles
    |> List.iter (fun js -> Yarn.exec (sprintf "prettier %s --write" js) yarnSetParams))

Target.create "FormatChanged" (fun _ ->
    Fake.Tools.Git.FileStatus.getChangedFilesInWorkingCopy "." "HEAD"
    |> Seq.choose (fun (_, file) ->
        let ext = Path.GetExtension(file)

        if file.StartsWith("src")
           && (ext = ".fs" || ext = ".fsi") then
            Some file
        else
            None)
    |> Seq.map (fun file -> async {
        let result = DotNet.exec id "fantomas" file
        if not result.OK then
            printfn "Problem when formatting %s:\n%A" file result.Errors
    })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

Target.create "CheckFormat" (fun _ ->
    let result =
        DotNet.exec id "fantomas" "src -r --check"

    if result.ExitCode = 0 then
        Trace.log "No files need formatting"
    elif result.ExitCode = 99 then
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
    Yarn.exec "build" yarnSetParams
)

Target.create "DeployClient" (fun _ -> Yarn.exec "deploy" yarnSetParams)

Target.create "BuildServer"
    (fun _ ->

    DotNet.publish (fun p ->
        { p with
            Configuration = DotNet.BuildConfiguration.Release
            OutputPath = Some (artifactPath </> "server")
        })
        serverProjectPath
)

Target.create "Watch" (fun _ ->
    let compileFable = async {
        let binFolder = clientPath </> "src" </> "bin"
        let shouldInitialize = 
            not (Directory.Exists(binFolder))
            || Seq.isEmpty (Directory.EnumerateFiles(binFolder))

        if shouldInitialize then 
            printfn "initial Fable compile"
            Yarn.exec "setup" yarnSetParams

        Yarn.exec "start" yarnSetParams
    }

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

        Azure.func ["start"; "--cors"; "*"]

    let runAzureFunction = async { startFunc() }

    Async.Parallel [ runAzureFunction;  compileFable ]
    |> Async.Ignore
    |> Async.RunSynchronously)

Target.create "PrepareRelease" ignore

// Build order
"InstallClient" ==> "Format"

"Clean" ==> "Install" ==> "InstallClient" ==> "BuildClient"

"Clean" ==> "BuildServer"

"Clean" ==> "BuildClient" ==> "DeployClient"

"PrepareRelease"
    <== [ "BuildClient" ; "BuildServer" ]

// start build
Target.runOrDefault "Build"
