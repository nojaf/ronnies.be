module Ronnies.Server.Authentication

open System
open System.Net.Http
open System.Text
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks
open Thoth.Json.Net
open Ronnies.Domain

type User =
    { Id : string
      Permissions : string list }

type HttpRequest with

    member this.GetUser (logger : ILogger) : User =
        let authorizationHeader =
            this.Headers.["Authorization"].ToString()

        let token =
            authorizationHeader.Replace("Bearer ", System.String.Empty).Replace("bearer ", System.String.Empty)

        let handler =
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()

        let tokenContent = handler.ReadJwtToken(token)
        let id = tokenContent.Subject

        let permissions =
            tokenContent.Claims
            |> Seq.choose (fun claim ->
                if claim.Type = "permissions" then
                    Some claim.Value
                else
                    None)
            |> Seq.toList

        let user = { Id = id; Permissions = permissions }

#if DEBUG
        logger.LogDebug(sprintf "User: %A" user)
#endif

        user

let mayWriteEvent permissions event =
    match event with
    | LocationAdded _ -> Seq.contains "write:location" permissions

let private clientId =
    Environment.GetEnvironmentVariable("Auth0Management_ClientId")

let private clientSecret =
    Environment.GetEnvironmentVariable("Auth0Management_ClientSecret")

let private audience =
    Environment.GetEnvironmentVariable("Auth0Management_Audience")

let managementAPIRoot =
    Environment.GetEnvironmentVariable("Auth0Management_APIRoot")

let getManagementAccessToken (log : ILogger) =
    task {
        let json =
            Encode.object [ "client_id", Encode.string clientId
                            "client_secret", Encode.string clientSecret
                            "audience", Encode.string audience
                            "grant_type", Encode.string "client_credentials" ]
            |> Encode.toString 4

        let url =
            sprintf "%s/oauth/token" managementAPIRoot

        let! json =
            Http.AsyncRequestString(url, body = TextRequest json, headers = [ ContentType HttpContentTypes.Json ])

        let accessTokenDecoder =
            Decode.object (fun get -> get.Required.Field "access_token" Decode.string)

        match Decode.fromString accessTokenDecoder json with
        | Ok token -> return token
        | Error err ->
            log.LogError("Invalid response from Auth0 management token API")
            failwithf "Error while decoding: %A" err
            return System.String.Empty
    }

type PushNotificationSubscription =
    { Endpoint : string
      Auth : string
      P256DH : string }

    static member FromBrowserDecoder =
        Decode.object (fun get ->
            { Endpoint = get.Required.Field "endpoint" Decode.string
              Auth = get.Required.At [ "keys"; "auth" ] Decode.string
              P256DH = get.Required.At [ "keys"; "p256dh" ] Decode.string })

    static member FromAuth0Decoder =
        Decode.object (fun get ->
            { Endpoint = get.Required.Field "endpoint" Decode.string
              Auth = get.Required.Field "auth" Decode.string
              P256DH = get.Required.Field "p256dh" Decode.string })

    static member Encoder : Encoder<PushNotificationSubscription> =
        fun (pns : PushNotificationSubscription) ->
            Encode.object [ "endpoint", Encode.string pns.Endpoint
                            "auth", Encode.string pns.Auth
                            "p256dh", Encode.string pns.P256DH ]

type AppMetaData =
    { PushNotificationSubscriptions : PushNotificationSubscription list }

    static member Decoder : Decoder<AppMetaData> =
        Decode.object (fun get ->
            let subs =
                get.Optional.Field
                    "pushNotificationSubscriptions"
                    (Decode.list PushNotificationSubscription.FromAuth0Decoder)
                |> Option.defaultValue []

            { PushNotificationSubscriptions = subs })

    static member Encoder : Encoder<AppMetaData> =
        fun amd ->
            Encode.object
                [ "pushNotificationSubscriptions",
                  List.map PushNotificationSubscription.Encoder amd.PushNotificationSubscriptions
                  |> Encode.list ]

type Auth0User =
    { AppMetaData : AppMetaData }

    static member Decoder : Decoder<Auth0User> =
        Decode.object (fun get ->
            let metaData =
                get.Optional.Field "app_metadata" AppMetaData.Decoder
                |> Option.defaultValue ({ PushNotificationSubscriptions = [] })

            { AppMetaData = metaData })

    static member Encoder : Encoder<Auth0User> =
        fun user -> Encode.object [ "app_metadata", AppMetaData.Encoder user.AppMetaData ]

let getUserPushNotificationSubscriptions (log : ILogger) managementToken userId =
    task {
        let url =
            sprintf "%s/api/v2/users/%s" managementAPIRoot userId

        let! response =
            Http.AsyncRequestString(url, headers = [ "Authorization", (sprintf "Bearer %s" managementToken) ])

        match Decode.fromString Auth0User.Decoder response with
        | Ok user -> return user.AppMetaData.PushNotificationSubscriptions
        | Error error ->
            log.LogError(sprintf "Failed to decode user from auth0, %A" error)
            return []
    }

let updateUserPushNotificationSubscription managementToken userId subscriptions =
    task {
        let url =
            sprintf "%s/api/v2/users/%s" managementAPIRoot userId

        let json =
            { AppMetaData = { PushNotificationSubscriptions = subscriptions } }
            |> Auth0User.Encoder
            |> Encode.toString 4

        let! _response =
            Http.AsyncRequest
                (url,
                 httpMethod = HttpMethod.Patch,
                 headers =
                     [ "Authorization", (sprintf "Bearer %s" managementToken)
                       ContentType HttpContentTypes.Json ],
                 body = TextRequest json)

        ()
    }
