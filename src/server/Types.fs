module Ronnies.Server.Types

open Thoth.Json.Net

type User =
    { Id : string
      Permissions : string list }

type PushNotificationSubscription =
    { Endpoint : string
      Auth : string
      P256DH : string
      Origin : string }

    static member FromBrowserDecoder (origin : string) =
        Decode.object (fun get ->
            { Endpoint = get.Required.Field "endpoint" Decode.string
              Auth = get.Required.At [ "keys"; "auth" ] Decode.string
              P256DH = get.Required.At [ "keys"; "p256dh" ] Decode.string
              Origin = origin })

    static member FromAuth0Decoder =
        Decode.object (fun get ->
            { Endpoint = get.Required.Field "endpoint" Decode.string
              Auth = get.Required.Field "auth" Decode.string
              P256DH = get.Required.Field "p256dh" Decode.string
              Origin = get.Required.Field "origin" Decode.string })

    static member Encoder : Encoder<PushNotificationSubscription> =
        fun (pns : PushNotificationSubscription) ->
            Encode.object [ "endpoint", Encode.string pns.Endpoint
                            "auth", Encode.string pns.Auth
                            "p256dh", Encode.string pns.P256DH
                            "origin", Encode.string pns.Origin ]

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
    { UserId: string
      AppMetaData : AppMetaData }

    static member Decoder : Decoder<Auth0User> =
        Decode.object (fun get ->
            let userId = get.Required.Field "user_id" Decode.string

            let metaData =
                get.Optional.Field "app_metadata" AppMetaData.Decoder
                |> Option.defaultValue ({ PushNotificationSubscriptions = [] })

            { UserId = userId; AppMetaData = metaData })

type PatchAuth0User =
    { AppMetaData : AppMetaData }

    static member Encoder : Encoder<PatchAuth0User> =
        fun user -> Encode.object [ "app_metadata", AppMetaData.Encoder user.AppMetaData ]

type UserInfo =
    { Name : string
      Id : string
      Picture : string }
