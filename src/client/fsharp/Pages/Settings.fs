module Ronnies.Client.Pages.Settings

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Browser.Navigator
open Fable.Import
open Fable.React
open Fable.React.Props
open Feliz
open ReactToastify
open Auth0
open Fetch
open Ronnies.Client
open Ronnies.Client.Components.Switch
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page

[<Emit("'serviceWorker' in navigator")>]
let private hasServiceWorker : bool = jsNative

[<Emit("'PushManager' in window")>]
let private hasPushManager : bool = jsNative

let private browserSupportsNotifications = hasPushManager && hasServiceWorker

let private urlB64ToUint8Array (_value : string) : byte array =
    import "urlB64ToUint8Array" "../js/utils.js"

type private PushSubscription =
    abstract endpoint : string
    abstract expirationTime : obj
    abstract options : obj
    abstract subscriptionId : string
    abstract getKey : unit -> JS.ArrayBuffer
    abstract toJSON : unit -> string
    abstract unsubscribe : unit -> JS.Promise<bool>

[<Emit("$0.pushManager.getSubscription()")>]
let private getSubscriptionFromSw sw : JS.Promise<PushSubscription option> = jsNative

[<Emit("$0.pushManager.subscribe({userVisibleOnly: true, applicationServerKey: $1})")>]
let private subscriptWithPushManager
    (sw : ServiceWorkerRegistration)
    (appServerKey : byte array)
    : JS.Promise<PushSubscription>
    =
    jsNative

let private hasSubscription () =
    match navigator.serviceWorker with
    | Some sw ->
        sw.ready
        |> Promise.bind getSubscriptionFromSw
        |> Promise.map Option.isSome
    | None -> Promise.lift false

let private addSubscription token =
    match navigator.serviceWorker with
    | Some sw ->
        sw.ready
        |> Promise.bind (fun sw ->
            promise {
                let! sub = getSubscriptionFromSw sw
                return (sw, sub)
            })
        |> Promise.bind (fun (sw, sub) ->
            match sub with
            | None ->
                let key = urlB64ToUint8Array Config.vapidKey
                subscriptWithPushManager sw key
                |> Promise.bind (fun subscription ->
                    let json =
                        subscription.toJSON () |> JS.JSON.stringify

                    JS.console.log subscription
                    JS.console.log (json)

                    let url =
                        sprintf "%s/api/add-subscription" Config.backendUrl

                    fetch
                        url
                        [ RequestProperties.Method HttpMethod.POST
                          RequestProperties.Body !^json
                          requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                           Config.authHeader token
                                           Config.subscriptionHeader ] ])
                |> Promise.map (fun _ ->
                    infoToast
                        "Notificaties check!"
                        [ ToastPosition ToastPosition.BottomRight
                          HideProgressBar true ])
            | Some sub ->
                printfn "unsubscribed"
                sub.unsubscribe () |> Promise.map ignore)

    | None -> Promise.reject "Geen service worker fwa"

let private removeSubscription token =
    match navigator.serviceWorker with
    | Some sw ->
        sw.ready
        |> Promise.bind getSubscriptionFromSw
        |> Promise.bind (fun sub ->
            match sub with
            | None -> Promise.lift ()
            | Some subscription ->
                subscription.unsubscribe ()
                |> Promise.bind (fun _ ->
                    let url =
                        sprintf "%s/api/remove-subscription" Config.backendUrl

                    fetch
                        url
                        [ RequestProperties.Method HttpMethod.POST
                          RequestProperties.Body !^subscription.endpoint
                          requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                           Config.authHeader token
                                           Config.subscriptionHeader ] ])
                |> Promise.map (fun _ ->
                    infoToast
                        "Notificaties uitgezet!"
                        [ ToastPosition ToastPosition.BottomRight
                          HideProgressBar true ]))
    | None -> Promise.lift ()

let private Settings =
    React.functionComponent
        ("SettingsPage",
         (fun () ->
             let eventCtx = React.useContext (eventContext)

             let clearCacheHandler _ =
                 eventCtx.ClearCache()
                 |> Promise.iter (fun () ->
                     infoToast
                         "Cache reset!"
                         [ ToastPosition ToastPosition.BottomRight
                           HideProgressBar true ])

             let auth0 = useAuth0 ()
             let (token, setToken) = React.useState ("")
             let roles = useRoles ()

             React.useEffect
                 ((fun () ->
                     if auth0.isAuthenticated then
                         auth0.getAccessTokenSilently ()
                         |> Promise.iter setToken),
                  [| box auth0.isAuthenticated |])

             let (notifications, setNotifications) = React.useState (false)

             React.useEffectOnce (fun () ->
                 hasSubscription ()
                 |> Promise.iter setNotifications)

             let updateNotifications value =
                 if value <> notifications then
                     if value then
                         addSubscription token
                     else
                         removeSubscription token
                     |> Promise.iter (fun _ -> setNotifications value)

             page [] [
                 h1 [] [ str "Settings" ]
                 button [ classNames [ Bootstrap.Btn
                                       Bootstrap.BtnOutlinePrimary
                                       Bootstrap.My4 ]
                          OnClick clearCacheHandler ] [
                     str "Reset cache"
                 ]
                 h4 [] [ str "Notificaties?" ]
                 if browserSupportsNotifications
                    && roles.IsEditorOrAdmin then
                     Switch
                         { TrueLabel = "Aan"
                           FalseLabel = "Uit"
                           OnChange = updateNotifications
                           Value = notifications }
                 else
                     div [ classNames [ Bootstrap.Alert
                                        Bootstrap.AlertWarning ] ] [
                         str "Je browser ondersteunt geen notificaties"
                     ]
             ]))

exportDefault Settings
