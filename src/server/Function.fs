module Ronnies.Server.Function

open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open Ronnies.Server
open Ronnies.Server.Authentication
open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net

module Task =
    let lift = System.Threading.Tasks.Task.FromResult

module Result =
    let either = function
        | Ok t
        | Error t -> t

    let ofOption =
        function
        | Some x -> Ok x
        | None -> Error()

let private sendJson json =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

let private sendText text =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text"))

let private sendInternalError err =
    new HttpResponseMessage(HttpStatusCode.InternalServerError,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let private sendBadRequest err =
    new HttpResponseMessage(HttpStatusCode.BadRequest,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let private unAuthorized() =
    new HttpResponseMessage(HttpStatusCode.Unauthorized)

let private decodeEvents (body: Stream) userId =
    let json = using (new StreamReader(body)) (fun stream -> stream.ReadToEnd())
    Decode.fromString (Decode.list EventStore.decodeEvent) json
    |> Result.map (fun events -> userId, events)
    |> Result.mapError (sendInternalError >> Task.lift)

let private validateEvents (userId, events) =
    let errors =
        events
        |> List.choose (fun event ->
            Validation.getValidationErrors event
            |> Option.map (fun errs ->
                Encode.object
                    [ "event", EventStore.encodeEvent event
                      "errors", ((List.map Encode.string errs) |> Encode.list) ]))

    match errors with
    | [] ->
        Ok(userId, events)
    | _ ->
        let responseBody = Encode.list errors |> Encode.toString 4
        Error(sendBadRequest responseBody |> Task.lift)

let persistEvents (userId, events) =
    task {
        do! EventStore.appendEvents userId events
        return sendText "Events persisted"
    }

[<FunctionName("AddEvents")>]
let AddEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request...")
    task {
        let! userId = req.Authenticate(log)
        let! response = Result.ofOption userId
                        |> Result.mapError (unAuthorized >> Task.lift)
                        |> Result.bind (decodeEvents req.Body)
                        |> Result.bind (validateEvents)
                        |> Result.map (persistEvents)
                        |> Result.either
        return response
    }


[<FunctionName("GetEvents")>]
let GetEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request.........")

    task {
        let! events = EventStore.getEvents()
        let json =
            Encode.list events |> Encode.toString 4
        return sendJson json
    }
