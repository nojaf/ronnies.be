module Ronnies.Server.Function

open System
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

let private notFound () =
    let json =
        Encode.string "Not found" |> Encode.toString 4

    new HttpResponseMessage(HttpStatusCode.NotFound,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

let private afterEventWasAdded
    (allSubscriptions : PushNotificationSubscription list)
    (allUsers : Map<string, UserInfo>)
    event
    =
    match event with
    | LocationAdded locationAdded ->
        task {
            let subject =
                Environment.GetEnvironmentVariable("Vapid_Subject")

            let privateKey =
                Environment.GetEnvironmentVariable("Vapid_PublicKey")

            let publicKey =
                Environment.GetEnvironmentVariable("Vapid_PrivateKey")

            let vapidDetails =
                VapidDetails(subject, privateKey, publicKey)

            let creatorName =
                NonEmptyString.Read locationAdded.Creator
                |> fun creator -> Map.find creator allUsers
                |> fun u -> u.Name

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

let private persistEvents log origin userId events =
    task {
        let! addedEvents = EventStore.appendEvents userId events
        let! managementToken = Authentication.getManagementAccessToken log
        let! allSubscriptions = Authentication.getPushNotificationSubscriptions log origin managementToken
        let! allUsers = Authentication.getAllUserInfo log managementToken

        let! _afterAddTasks =
            List.map (afterEventWasAdded allSubscriptions allUsers) events
            |> Task.WhenAll

        let json =
            addedEvents
            |> List.map (fun evRead -> evRead.Version.ToString(), evRead.Data)
            |> Encode.object
            |> Encode.toString 4

        return sendJson json
    }


let private addEvents (log : ILogger) (req : HttpRequest) =
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

let private getEvents (log : ILogger) (req : HttpRequest) =
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

let private addSubscription (log : ILogger) (req : HttpRequest) =
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

let private removeSubscription (log : ILogger) (req : HttpRequest) =
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

let private getUsers (log : ILogger) (req : HttpRequest) =
    log.LogInformation("Start get-users")
    task {
        let! managementToken = Authentication.getManagementAccessToken log
        let! users = Authentication.getAllUserInfo log managementToken

        let json =
            Map.toList users
            |> List.map (fun (k, u) ->
                k,
                Encode.object [ "name", Encode.string u.Name
                                "picture", Encode.string u.Picture ])
            |> Encode.object
            |> Encode.toString 4

        return sendJson json
    }

let private getUser (log : ILogger) (id : string) =
    log.LogInformation(sprintf "Start get-user {%s}" id)
    task {
        let! managementToken = Authentication.getManagementAccessToken log
        let! user = Authentication.getUserInfo log managementToken id
        return sendJson user
    }

let ping (log : ILogger) =
    log.LogInformation "pinged"
    Encode.object [ "value", Encode.string "pong"
                    "createdUTC", Encode.datetimeOffset (DateTimeOffset.UtcNow) ]
    |> Encode.toString 4
    |> sendJson

let private (|GetUserRoute|_|) (path : string) =
    if System.Text.RegularExpressions.Regex.IsMatch(path, "\\/users\\/(\\S)+") then
        let id = path.Replace("/users/", String.Empty)
        Some id
    else
        None

[<FunctionName("ronnies")>]
let Ronnies ([<HttpTrigger(AuthorizationLevel.Function, "get", "post", "delete", Route = "{*any}")>] req : HttpRequest,
             log : ILogger)
    =
    log.LogInformation("Entering Ronnies backend")

    task {
        let path = req.Path.Value.ToLower()
        let method = req.Method.ToUpper()
        log.LogInformation (sprintf "%s %s" method path)

        match method, path with
        | "GET", "/events" -> return! getEvents log req
        | "POST", "/events" -> return! addEvents log req
        | "POST", "/subscriptions" -> return! addSubscription log req
        | "DELETE", "/subscriptions" -> return! removeSubscription log req
        | "GET", "/users" -> return! getUsers log req
        | "GET", GetUserRoute (id) -> return! getUser log id
        | "GET", "/ping" -> return ping log
        | _ -> return notFound ()
    }
