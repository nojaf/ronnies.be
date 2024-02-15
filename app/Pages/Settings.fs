module Settings

open Fable.Core.JsInterop
open Fable.Core
open Browser
open Browser.Types
open Feliz
open React
open React.DSL
open React.DSL.Props
open type Firebase.Hooks.Exports
open type Firebase.FireStore.Exports
open type Firebase.Messaging.Exports
open ComponentsDSL

[<Literal>]
let VAPID_KEY =
    "BFh5svrvg2AivizTTNse7B4DYRqJ7lk9vg2IN13T_sbn3ONdE1A0Z8eoyu28r24a7u82giEkHARB09Qt7zBtNLI"

[<Literal>]
let FCM_TOKEN_COLLECTION = "fcmTokens"

type FCMTokenData = {| tokens : string array |}

[<Emit("'serviceWorker' in navigator")>]
let hasServiceWorker : bool = jsNative

[<Emit("'PushManager' in window")>]
let hasPushManager : bool = jsNative

let browserSupportsNotifications = hasPushManager && hasServiceWorker

let registerServiceWorker () =
    match navigator.serviceWorker with
    | None -> Promise.reject (exn "Service worker not available")
    | Some serviceWorker ->
        let options : ServiceWorkerRegistrationOptions =
            emitJsExpr () "{ \"type\": \"module\" }"

        serviceWorker.register ("/firebase-messaging-sw.js", options)

let getTokenSnapshot (uid : uid) =
    let tokenRef = doc (firestore, FCM_TOKEN_COLLECTION, uid)
    getDoc<FCMTokenData> (tokenRef)

let getFcmToken () =
    promise {
        let! messaging = messaging ()
        let! registration = registerServiceWorker ()

        return!
            getToken (
                messaging,
                {|
                    vapidKey = VAPID_KEY
                    serviceWorkerRegistration = registration
                |}
            )
    }

[<RequireQualifiedAccess>]
type EnableNotifications =
    | Unknown
    | Yes
    | No

let rec requestNotificationsPermissions (uid : uid) : JS.Promise<EnableNotifications> =
    promise {
        let! permission = emitJsExpr<JS.Promise<string>> () "Notification.requestPermission()"

        if permission = "granted" then
            return! saveMessagineDeviceToken uid
        else
            console.log $"Unable to get permission to notify, got %s{permission}"
            return EnableNotifications.No
    }

and saveMessagineDeviceToken (uid : uid) : JS.Promise<EnableNotifications> =
    try
        promise {
            let! fcmToken = getFcmToken ()

            match fcmToken with
            | None -> return! requestNotificationsPermissions uid
            | Some fcmToken ->
                console.log ("Got FCM device token:", fcmToken)
                let! docSnapshot = getTokenSnapshot uid
                let tokenRef = docSnapshot.ref

                if docSnapshot.exists () then
                    let tokens =
                        let data = docSnapshot.data ()
                        [| yield fcmToken ; yield! data.tokens |] |> Array.distinct

                    do! updateDoc (tokenRef, {| tokens = tokens |})
                    return EnableNotifications.Yes
                else
                    do! setDoc (tokenRef, {| tokens = [| fcmToken |] |})
                    return EnableNotifications.Yes

        }
    with ex ->
        console.error ("Unable to get messaging token.", ex)
        Promise.lift EnableNotifications.No

let SettingsPage () =
    let user, isUserLoading, _ = useAuthState auth
    let tokenResult, _, _ = useAuthIdTokenResult auth

    let value, setValue =
        React.useState<EnableNotifications> EnableNotifications.Unknown

    let onChange (v : bool) =
        match user with
        | None -> ()
        | Some user ->

        if v then
            requestNotificationsPermissions user.uid |> Promise.iter setValue
        else
            // Remove the current token from the user
            promise {
                let! docSnapshot = getTokenSnapshot user.uid

                if not (docSnapshot.exists ()) then
                    ()
                else

                let! fcmToken = getFcmToken ()

                match fcmToken with
                | None -> ()
                | Some fcmToken ->
                    let tokenData = docSnapshot.data ()
                    let tokens = tokenData.tokens |> Array.filter (fun t -> t <> fcmToken)
                    do! updateDoc (docSnapshot.ref, {| tokens = tokens |})
                    setValue EnableNotifications.No

            }
            |> Promise.start

    React.useEffect (
        fun () ->
            match user, tokenResult with
            | Some user, Some tokenResult ->
                if tokenResult.claims?``member`` then
                    promise {
                        let! tokenSnapshot = getTokenSnapshot user.uid

                        if not (tokenSnapshot.exists ()) then
                            let! v = requestNotificationsPermissions user.uid
                            setValue v
                        else
                            let! fcmToken = getFcmToken ()

                            match fcmToken with
                            | None -> ()
                            | Some fcmToken ->

                            let tokenData = tokenSnapshot.data ()
                            let hasToken = Array.contains fcmToken tokenData.tokens

                            if hasToken then
                                setValue EnableNotifications.Yes
                            else
                                setValue EnableNotifications.No
                    }
                    |> Promise.start

                else
                    JS.console.log "Not a member"
            | _ -> ()
        , [| box tokenResult ; box isUserLoading |]
    )

    main [ Id "settings" ] [
        h1 [ Key "title" ] [ str "Settings" ]
        if browserSupportsNotifications && value <> EnableNotifications.Unknown then
            h2 [ Key "notifications-title" ] [ str "Notificaties?" ]

            if not browserSupportsNotifications then
                p [ Key "no-notifications-support" ] [ em [] [ str "Je browser ondersteunt geen notificaties." ] ]

            toggle [
                ToggleProp.TrueLabel "Aan" :> IProp
                ToggleProp.FalseLabel "Uit"
                ToggleProp.OnChange onChange
                ToggleProp.Value (value = EnableNotifications.Yes)
                ToggleProp.Disabled false
                Key "toggle-notifications"
            ]
    ]
