module Ronnies.Server.Function

open System
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net
open Ronnies.Domain
open Ronnies.Server.Authentication
open Ronnies.Server.EventStore

type HttpRequest with

    member this.ReadJson () =
        let json =
            using (new StreamReader(this.Body)) (fun stream -> stream.ReadToEnd())

        json

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

let private sendUnAuthorizedRequest err =
    new HttpResponseMessage(HttpStatusCode.Unauthorized,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let persistEvents userId events =
    task {
        let! addedEvents = EventStore.appendEvents userId events

        let json =
            addedEvents
            |> List.map (fun evRead -> evRead.Version.ToString(), evRead.Data)
            |> Encode.object
            |> Encode.toString 4

        return sendJson json
    }

[<FunctionName("add-events")>]
let AddEvents ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>] req : HttpRequest, log : ILogger) =
    log.LogInformation("Start add-events")
    task {
        let user = req.GetUser log
        let! json = req.ReadAsStringAsync()

        let eventsResult =
            Thoth.Json.Net.Decode.fromString (Decode.list Event.Decoder) json

        match eventsResult with
        | Ok events when List.forall (Authentication.mayWriteEvent user.Permissions) events ->
            return! persistEvents user.Id events
        | Ok event ->
            let msg =
                sprintf "Unauthorized to persist event %s" (getUnionCaseName event)

            return sendUnAuthorizedRequest msg
        | Error err ->
            log.LogError(err)
            return (sendBadRequest "Invalid event json")
    }

[<FunctionName("get-events")>]
let GetEvents ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = null)>] req : HttpRequest, log : ILogger) =
    log.LogInformation("Start get-events")

    task {
        let lastEvent =
            match req.Query.TryGetValue "lastEvent" with
            | true, lastEvent -> lastEvent.ToString() |> (int64) |> Some
            | _ -> None

        let! events = EventStore.getEvents lastEvent

        let json =
            events |> Encode.object |> Encode.toString 4

        return sendJson json
    }

[<FunctionName("add-subscription")>]
let AddSubscription ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>] req : HttpRequest, log : ILogger) =
    log.LogInformation("Start add-subscription")

    task {
        let userFromToken = req.GetUser log
        let! json = req.ReadAsStringAsync()
        log.LogInformation json
        let! managementToken = Authentication.getManagementAccessToken log

        match Decode.fromString PushNotificationSubscription.FromBrowserDecoder json with
        | Ok subscription ->
            let! existingSubscriptions =
                Authentication.getUserPushNotificationSubscriptions log managementToken userFromToken.Id

            if List.forall (fun s -> s.Endpoint <> subscription.Endpoint) existingSubscriptions then
                do! Authentication.updateUserPushNotificationSubscription
                        managementToken
                        userFromToken.Id
                        (subscription :: existingSubscriptions)

            return sendText "Subscription added"

        | Error err -> return sendBadRequest (err.ToString())
    }

[<FunctionName("remove-subscription")>]
let RemoveSubscription ([<HttpTrigger(AuthorizationLevel.Function, "post", Route = null)>] req : HttpRequest,
                        log : ILogger) =
    log.LogInformation("Start remove-subscription")
    task {
        let user = req.GetUser log
        let! endpoint = req.ReadAsStringAsync()
        let! managementToken = Authentication.getManagementAccessToken log
        let! existingSubscriptions = Authentication.getUserPushNotificationSubscriptions log managementToken user.Id

        let updatedSubscriptions =
            List.filter (fun s -> s.Endpoint <> endpoint) existingSubscriptions

        do! Authentication.updateUserPushNotificationSubscription managementToken user.Id updatedSubscriptions
        return sendText "Subscription removed"
    }
