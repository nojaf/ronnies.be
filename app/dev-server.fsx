#r "nuget: Suave, 2.6.2"
#r "nuget: FSharp.Data.Adaptive, 1.2.13"
#r "nuget: CliWrap, 3.6.0"
#r "nuget: Fake.IO.FileSystem, 6.0.0"

open System
open System.Collections.Concurrent
open System.IO
open System.Text
open System.Net
open System.Threading
open System.Threading.Tasks
open CliWrap
open CliWrap.EventStream
open FSharp.Data.Adaptive
open Suave
open Suave.Logging
open Suave.Files
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

let exitTask = TaskCompletionSource<unit> ()
let cts = new CancellationTokenSource ()

Console.CancelKeyPress.Add (fun _ ->
    printfn "Goodbye"
    cts.Cancel ()
    exitTask.SetResult ()
)

let dist = __SOURCE_DIRECTORY__ </> "dist"

let port = 4000us

let fableArgs =
    "./App.fsproj -e .js -o ./out --fableLib fable-library --noReflection"

let build () =
    Shell.rm_rf dist

    Cli
        .Wrap("dotnet")
        .WithArguments($"fable {fableArgs}  -e .js")
        .WithWorkingDirectory(__SOURCE_DIRECTORY__)
        .WithStandardOutputPipe(PipeTarget.ToDelegate (printfn "%A"))
        .ExecuteAsync()
        .Task.Wait ()

    !!(__SOURCE_DIRECTORY__ </> "**/*.js")
    |> GlobbingPattern.setBaseDir __SOURCE_DIRECTORY__
    |> Shell.copyFilesWithSubFolder dist

    [ "index.html" ; "style.css" ; "favicon.ico" ]
    |> List.map (fun file -> __SOURCE_DIRECTORY__ </> file)
    |> Shell.copy dist

    // Update importmap to use esm.sh
    let indexHtml = Path.Combine (dist, "index.html")

    let indexHtmlContent =
        (File.ReadAllText indexHtml)
            .Replace ("http://localhost:9004", "https://esm.sh")

    File.WriteAllText (indexHtml, indexHtmlContent)

let serveFiles =
    [
        GET >=> path "/" >=> file (__SOURCE_DIRECTORY__ </> "index.html")
        GET >=> browseHome
        // SPA route
        file (__SOURCE_DIRECTORY__ </> "index.html")
    ]

let args =
    fsi.CommandLineArgs
    |> Array.skipWhile (fun arg -> arg.EndsWith ".fsx")
    |> Array.map String.toLowerInvariant
    |> Array.toList

match args with
| [ "help" ]
| [ "--help" ] ->
    printfn "Available commands: build, preview, watch, fable"
    exit 0
| [ "fable" ] ->
    Cli
        .Wrap("dotnet")
        .WithArguments($"fable {fableArgs}  -e .js")
        .WithWorkingDirectory(__SOURCE_DIRECTORY__)
        .ExecuteAsync()
        .Task.Wait ()
| [ "build" ] -> 
    build ()
    exit 0
| [ "preview" ] ->
    build ()

    let conf =
        { defaultConfig with
            cancellationToken = cts.Token
            homeFolder = Some (__SOURCE_DIRECTORY__ </> "dist")
            compressedFilesFolder = Some (Path.GetTempPath ())
            bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ]
        }

    let _, suaveServer = startWebServerAsync conf (choose serveFiles)

    Async.Start (suaveServer, cts.Token)
    exitTask.Task.Wait ()
| []
| [ "watch" ] ->
    let fableHandle =
        Cli
            .Wrap("dotnet")
            .WithArguments($"fable watch {fableArgs} --define DEBUG")
            .WithWorkingDirectory(__SOURCE_DIRECTORY__)
            .Observe (cancellationToken = cts.Token)
        |> Observable.subscribe (fun ev ->
            match ev with
            | :? StartedCommandEvent as startedEvent -> printfn $"Fable Started: %i{startedEvent.ProcessId}"
            | :? StandardOutputCommandEvent as stdOutEvent -> printfn $"Fable: %s{stdOutEvent.Text}"
            | :? ExitedCommandEvent as exitedEvent -> printfn $"Fable Exited: %A{exitedEvent.ExitCode}"
            | :? StandardErrorCommandEvent as stdErrEvent -> printfn $"Fable ERR: %s{stdErrEvent.Text}"
            | _ -> ()
        )

    let connectedClients = ConcurrentDictionary<WebSocket, unit> ()

    let ws (webSocket : WebSocket) (context : HttpContext) =
        context.runtime.logger.info (Message.eventX $"New websocket connection")
        connectedClients.TryAdd (webSocket, ()) |> ignore

        socket {
            let mutable loop = true

            while loop do
                let! msg = webSocket.read ()

                match msg with
                | Text, data, true ->
                    let str = Encoding.UTF8.GetString data
                    context.runtime.logger.info (Message.eventX $"Received %s{str} from client")
                    let response = sprintf "response to %s" str
                    let byteResponse = response |> Encoding.UTF8.GetBytes |> ByteSegment
                    do! webSocket.send Text byteResponse true

                | Close, _, _ ->
                    context.runtime.logger.info (Message.eventX "Closing connection")
                    connectedClients.TryRemove webSocket |> ignore
                    let emptyResponse = [||] |> ByteSegment
                    do! webSocket.send Close emptyResponse true
                    loop <- false

                | _ -> ()
        }

    let broadCastReload (msg : string) =
        let msg = msg |> Encoding.UTF8.GetBytes |> ByteSegment

        connectedClients.Keys
        |> Seq.map (fun client ->
            async {
                let! _ = client.send Text msg true
                ()
            }
        )
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously

    let cssWatcher =
        AdaptiveFile
            .GetLastWriteTime(Path.Combine (__SOURCE_DIRECTORY__, "style.css"))
            .AddCallback (fun _ -> broadCastReload "style.css")

    let indexHtmlWatcher =
        AdaptiveFile
            .GetLastWriteTime(Path.Combine (__SOURCE_DIRECTORY__, "index.html"))
            .AddCallback (fun _ -> broadCastReload "full")

    let jsWatcher =
        AdaptiveDirectory.GetFiles (__SOURCE_DIRECTORY__, pattern = ".*\\.js", recursive = true)
        |> ASet.map (fun file -> AdaptiveFile.GetLastWriteTime file.FullName)
        |> ASet.flattenA
        |> ASet.tryMax
        |> fun aval -> aval.AddCallback (fun _ -> broadCastReload "full")

    let server = choose [ path "/ws" >=> handShake ws ; yield! serveFiles ]

    let conf =
        { defaultConfig with
            cancellationToken = cts.Token
            homeFolder = Some __SOURCE_DIRECTORY__
            compressedFilesFolder = Some (Path.GetTempPath ())
            bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ]
        }

    let _, suaveServer = startWebServerAsync conf server
    Async.Start (suaveServer, cts.Token)
    printfn $"Suave server started on http://localhost:%i{port}"

    cts.Token.Register (fun _ ->
        cssWatcher.Dispose ()
        indexHtmlWatcher.Dispose ()
        jsWatcher.Dispose ()
        fableHandle.Dispose ()
    )
    |> ignore

    exitTask.Task.Wait ()
| args -> failwithf "invalid args: %A" args
