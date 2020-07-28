module Ronnies.Server.Function

open System
open System.Net.Http.Headers
open System.Threading.Tasks
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
open WebPush

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

let private afterEventWasAdded log origin event =
    match event with
    | LocationAdded locationAdded ->
        task {
            let! managementToken = Authentication.getManagementAccessToken log
            let! allSubscriptions = Authentication.getPushNotificationSubscriptions log origin managementToken

            let subject =
                Environment.GetEnvironmentVariable("Vapid_Subject")

            let privateKey =
                Environment.GetEnvironmentVariable("Vapid_PublicKey")

            let publicKey =
                Environment.GetEnvironmentVariable("Vapid_PrivateKey")

            let vapidDetails =
                VapidDetails(subject, privateKey, publicKey)

            let! creatorName =
                NonEmptyString.Read locationAdded.Creator
                |> Authentication.getUserName log managementToken

            let payload =
                Encode.object [ "id", Identifier.Read locationAdded.Id |> Encode.guid
                                "creator", Encode.string creatorName
                                "name",
                                NonEmptyString.Read locationAdded.Name
                                |> Encode.string
                                "type", Encode.string "locationAdded" ]
                |> Encode.toString 4

            let webPushClient = WebPushClient()

            let! _sendPushNotifications =
                allSubscriptions
                |> List.map (fun s ->
                    let ps =
                        PushSubscription(s.Endpoint, s.P256DH, s.Auth)

                    webPushClient.SendNotificationAsync(ps, payload, vapidDetails))
                |> Task.WhenAll

            ()
        } :> Task
    | _ -> System.Threading.Tasks.Task.CompletedTask

let persistEvents log origin userId events =
    task {
        let! addedEvents = EventStore.appendEvents userId events

        let! _afterAddTasks =
            List.map (afterEventWasAdded log origin) events
            |> Task.WhenAll

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
        let origin = req.Headers.["Origin"].ToString()

        let eventsResult =
            Thoth.Json.Net.Decode.fromString (Decode.list Event.Decoder) json

        match eventsResult with
        | Ok events when List.forall (Authentication.mayWriteEvent user.Permissions) events ->
            return! persistEvents log origin user.Id events
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
        let origin = req.Headers.["Origin"].ToString()
        log.LogInformation(sprintf "Origin: %s" origin)

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
        let origin = req.Headers.["Origin"].ToString()
        let userFromToken = req.GetUser log
        let! json = req.ReadAsStringAsync()
        log.LogInformation json
        let! managementToken = Authentication.getManagementAccessToken log

        match Decode.fromString (PushNotificationSubscription.FromBrowserDecoder origin) json with
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
                        log : ILogger)
    =
    log.LogInformation("Start remove-subscription")
    task {
        let origin = req.Headers.["Origin"].ToString()
        let user = req.GetUser log
        let! endpoint = req.ReadAsStringAsync()
        let! managementToken = Authentication.getManagementAccessToken log
        let! existingSubscriptions = Authentication.getUserPushNotificationSubscriptions log managementToken user.Id

        let updatedSubscriptions =
            List.filter (fun s -> not (s.Endpoint = endpoint && s.Origin = origin)) existingSubscriptions

        do! Authentication.updateUserPushNotificationSubscription managementToken user.Id updatedSubscriptions

        return sendText "Subscription removed"
    }

[<FunctionName("get-users")>]
let GetUsers ([<HttpTrigger(AuthorizationLevel.Function, "get", Route = null)>] req : HttpRequest, log : ILogger) =
    log.LogInformation("Start get-users")
    task {
        let! managementToken = Authentication.getManagementAccessToken log
        let! users = Authentication.getAllUserInfo log managementToken
        return sendJson users
    }
