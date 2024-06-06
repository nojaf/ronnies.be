module Settings

open Fable.Core.JsInterop
open Fable.Core
open Browser
open Browser.Types
open React
open type React.DSL.DOMProps
open type Firebase.Hooks.Exports
open type Firebase.FireStore.Exports
open type Firebase.Messaging.Exports
open Ronnies.Shared
open StyledComponents
open ComponentsDSL

[<RequireQualifiedAccess>]
type LogEntry =
    | Info of string
    | Error of string

[<Literal>]
let VAPID_KEY =
    "BFh5svrvg2AivizTTNse7B4DYRqJ7lk9vg2IN13T_sbn3ONdE1A0Z8eoyu28r24a7u82giEkHARB09Qt7zBtNLI"

[<Literal>]
let FCM_TOKEN_COLLECTION = "fcmTokens"

[<Emit("'serviceWorker' in navigator")>]
let hasServiceWorker : bool = jsNative

[<Emit("'PushManager' in window")>]
let hasPushManager : bool = jsNative

let browserSupportsNotifications = hasPushManager && hasServiceWorker

let getTokenSnapshot (uid : uid) =
    let tokenRef = doc (firestore, FCM_TOKEN_COLLECTION, uid)
    getDoc<FCMTokenData> tokenRef

let getFcmToken () =
    promise {
        let! messaging = messaging ()
        return! getToken (messaging, {| vapidKey = VAPID_KEY |})
    }

[<RequireQualifiedAccess>]
type private EnableNotifications =
    | Unknown
    | Yes of fcmToken : string
    | No

let rec private requestNotificationsPermissions
    (addLog : LogEntry -> unit)
    (uid : uid)
    : JS.Promise<EnableNotifications>
    =
    promise {
        let! permission = emitJsExpr<JS.Promise<string>> () "Notification.requestPermission()"

        if permission = "granted" then
            addLog (LogEntry.Info "Notification.requestPermission granted")
            return! saveMessageDeviceToken addLog uid
        else
            addLog (LogEntry.Error $"Unable to get permission to notify, got %s{permission}")
            return EnableNotifications.No
    }

and private saveMessageDeviceToken (addLog : LogEntry -> unit) (uid : uid) : JS.Promise<EnableNotifications> =
    try
        promise {
            let! fcmToken = getFcmToken ()

            match fcmToken with
            | None -> return! requestNotificationsPermissions addLog uid
            | Some fcmToken ->
                addLog (LogEntry.Info $"Got FCM device token: %s{fcmToken}")
                let! docSnapshot = getTokenSnapshot uid
                let tokenRef = docSnapshot.ref

                if docSnapshot.exists () then
                    addLog (LogEntry.Info $"getTokenSnapshot for %s{uid} exists")

                    let tokens =
                        let data = docSnapshot.data ()
                        [| yield fcmToken ; yield! data.tokens |] |> Array.distinct

                    do! updateDoc (tokenRef, {| tokens = tokens |})
                    addLog (LogEntry.Info "tokenSnapshot updated")
                    return EnableNotifications.Yes fcmToken
                else
                    do! setDoc (tokenRef, {| tokens = [| fcmToken |] |})
                    addLog (LogEntry.Info "tokenSnapshot created")
                    return EnableNotifications.Yes fcmToken

        }
    with ex ->
        console.error ("Unable to get messaging token.", ex)
        Promise.lift EnableNotifications.No

let StyledMain : JSX.ElementType =
    mkStyleComponent
        "main"
        """
h3 { margin-top: var(--spacing-400); }

code {
    margin-top:  var(--spacing-200);
    display: block;
    word-break: break-all;
}

ul li.error {
    color: firebrick;
}
"""

[<ExportDefault>]
let SettingsPage () =
    let user, isUserLoading, _ = useAuthState auth
    let tokenResult, _, _ = useAuthIdTokenResult auth

    let value, setValue =
        React.useState<EnableNotifications> EnableNotifications.Unknown

    let logs, setLogs = React.useStateByFunction (Array.empty)

    let addLog entry =
        setLogs (fun logs ->
            if Array.isEmpty logs then
                Array.singleton entry
            else
                Array.insertAt (logs.Length) entry logs
        )

    let onChange (v : bool) =
        match user with
        | None -> ()
        | Some user ->

        if v then
            requestNotificationsPermissions addLog user.uid |> Promise.iter setValue
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
                            let! v = requestNotificationsPermissions addLog user.uid
                            setValue v
                        else
                            addLog (LogEntry.Info $"tokenSnapshot exists for %s{user.uid}")
                            let! fcmToken = getFcmToken ()

                            match fcmToken with
                            | None ->
                                addLog (LogEntry.Info $"fcmToken does not exist")
                                ()
                            | Some fcmToken ->

                            addLog (LogEntry.Info $"fcmToken does exist %s{fcmToken}")

                            let tokenData = tokenSnapshot.data ()
                            let hasToken = Array.contains fcmToken tokenData.tokens

                            if hasToken then
                                setValue (EnableNotifications.Yes fcmToken)
                            else
                                setValue EnableNotifications.No
                    }
                    |> Promise.catch (fun err ->
                        addLog (LogEntry.Error $"Error in on-mount token, %A{err}")
                        ()
                    )
                    |> Promise.start

                else
                    JS.console.log "Not a member"
            | _ -> ()
        , [| box tokenResult ; box isUserLoading |]
    )

    let adminInfo =
        match tokenResult with
        | Some tokenResult when tokenResult.claims?``admin`` ->
            div [ Key "admin" ] [
                h3 [ Key "header" ] [ str "Test notification" ]
                button [
                    Key "button"
                    OnClick (fun ev ->
                        ev.preventDefault ()
                        API.testNotification () |> Promise.start
                    )
                ] [ str "Send!" ]
                match value with
                | EnableNotifications.Yes fcmToken ->
                    code [ Key "fcmToken" ] [ str "Current fcmToken: " ; br [] ; str fcmToken ]
                | _ -> null
                h3 [ Key "log-header" ] [ str "Logs" ]
                ul [ Key "logs" ] [
                    for (idx, log) in Array.indexed logs do
                        match log with
                        | LogEntry.Info info -> yield li [ Key $"log-%i{idx}" ] [ str info ]
                        | LogEntry.Error error -> yield li [ Key $"log-%i{idx}" ; ClassName "error" ] [ str error ]
                ]
            ]
        | _ -> null

    styledComponent StyledMain [
        h1 [ Key "title" ] [ str "Settings" ]
        match value with
        | EnableNotifications.Unknown -> loader [ Key "loader" ]
        | enableNotifications ->

        if browserSupportsNotifications then
            h2 [ Key "notifications-title" ] [ str "Notificaties?" ]

            if not browserSupportsNotifications then
                p [ Key "no-notifications-support" ] [ em [] [ str "Je browser ondersteunt geen notificaties." ] ]

            let toggleValue =
                match enableNotifications with
                | EnableNotifications.Yes _ -> true
                | _ -> false

            toggle [
                ToggleProp.TrueLabel "Aan"
                ToggleProp.FalseLabel "Uit"
                ToggleProp.OnChange onChange
                ToggleProp.Value toggleValue
                ToggleProp.Disabled false
                Key "toggle-notifications"
            ]

            adminInfo
    ]
