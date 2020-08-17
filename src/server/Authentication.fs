[<RequireQualifiedAccess>]
module Ronnies.Server.Authentication

open System
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks
open Thoth.Json.Net
open Ronnies.Domain
open Ronnies.Server.Types

let getUser (logger : ILogger) (req : HttpRequest) : User =
    let authorizationHeader = req.Headers.["Authorization"].ToString()

    let token =
        authorizationHeader.Replace("Bearer ", String.Empty).Replace("bearer ", String.Empty)

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
    | LocationCancelled _ -> Seq.contains "delete:location" permissions
    | LocationNoLongerSellsRonnies _ -> Seq.contains "write:location" permissions

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
            return String.Empty
    }

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
            |> PatchAuth0User.Encoder
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

let private allSubscriptionsDecoder origin =
    Decode.list Auth0User.Decoder
    |> Decode.map (fun users ->
        users
        |> List.choose (fun (user : Auth0User) ->
            let subs =
                user.AppMetaData.PushNotificationSubscriptions

            if List.isEmpty subs then
                None
            else
                Some(user, subs)))

let getPushNotificationSubscriptions (log : ILogger) origin managementToken =
    task {
        let url =
            sprintf "%s/api/v2/users?per_page=100&fields=user_id,app_metadata" managementAPIRoot

        let! users = Http.AsyncRequestString(url, headers = [ "Authorization", (sprintf "Bearer %s" managementToken) ])

        let subscriptions =
            Decode.fromString (allSubscriptionsDecoder origin) users

        match subscriptions with
        | Ok subs -> return subs
        | Error err ->
            log.LogError(sprintf "Invalid response from Auth0 management token API, %A" err)
            return List.empty
    }

let private userNameDecoder (get : Decode.IGetters) =
    let givenName =
        get.Optional.Field "given_name" Decode.string

    let familyName =
        get.Optional.Field "family_name" Decode.string

    match givenName, familyName with
    | Some g, Some f -> sprintf "%s %c" g f.[0]
    | _ -> get.Required.Field "nickname" Decode.string

let getUserName (log : ILogger) managementToken userId =
    task {
        let url =
            sprintf "%s/api/v2/users/%s" managementAPIRoot userId

        let! response =
            Http.AsyncRequestString(url, headers = [ "Authorization", (sprintf "Bearer %s" managementToken) ])

        let decodeUserName = Decode.object userNameDecoder

        match Decode.fromString decodeUserName response with
        | Ok userName -> return userName
        | Error error ->
            log.LogError(sprintf "Failed to decode username from auth0, %A" error)
            return "???"
    }

let private userInfoDecoder =
    Decode.object (fun get ->
        { Name = userNameDecoder get
          Id = get.Required.Field "user_id" Decode.string
          Picture = get.Required.Field "picture" Decode.string })

let getAllUserInfo (log : ILogger) managementToken =
    task {
        let url =
            sprintf "%s/api/v2/users" managementAPIRoot

        let! response =
            Http.AsyncRequestString(url, headers = [ "Authorization", (sprintf "Bearer %s" managementToken) ])

        let userResult =
            match Decode.fromString (Decode.list userInfoDecoder) response with
            | Ok users -> users |> List.map (fun u -> u.Id, u) |> Map.ofList

            | Error err ->
                log.LogError(sprintf "Failed to decode users from auth0, %A" err)
                Map.empty

        return userResult
    }

let getUserInfo (log : ILogger) managementToken id =
    task {
        let url =
            sprintf "%s/api/v2/users/%s" managementAPIRoot id

        let! response =
            Http.AsyncRequestString(url, headers = [ "Authorization", (sprintf "Bearer %s" managementToken) ])

        let userResult =
            match Decode.fromString userInfoDecoder response with
            | Ok user ->
                Encode.object [ "name", Encode.string user.Name
                                "picture", Encode.string user.Picture ]
                |> Encode.toString 4

            | Error err ->
                log.LogError(sprintf "Failed to decode users from auth0, %A" err)
                String.Empty

        return userResult
    }
