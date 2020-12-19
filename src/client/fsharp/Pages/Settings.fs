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
open Ronnies.Client.Components.Loading
open Ronnies.Client.Components.Switch
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page

[<Emit("'serviceWorker' in navigator")>]
let private hasServiceWorker : bool = jsNative

[<Emit("'PushManager' in window")>]
let private hasPushManager : bool = jsNative

let private browserSupportsNotifications = hasPushManager && hasServiceWorker

let private urlB64ToUint8Array (value : string) : byte array =
    Fable.Core.JsInterop.emitJsStatement value """
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
    """

type private PushSubscription =
    abstract endpoint : string
    abstract expirationTime : obj
    abstract options : obj
    abstract subscriptionId : string
    abstract getKey : unit -> JS.ArrayBuffer
    abstract toJSON : unit -> string
    abstract unsubscribe : unit -> JS.Promise<bool>

[<Emit("$0.pushManager.getSubscription()")>]
let private getSubscriptionFromSw _sw : JS.Promise<PushSubscription option> = jsNative

[<Emit("$0.pushManager.subscribe({userVisibleOnly: true, applicationServerKey: $1})")>]
let private subscriptWithPushManager
    (_sw : ServiceWorkerRegistration)
    (_appServerKey : byte array)
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
        |> Promise.bind
            (fun sw ->
                promise {
                    let! sub = getSubscriptionFromSw sw
                    return (sw, sub)
                })
        |> Promise.bind
            (fun (sw, sub) ->
                match sub with
                | None ->
                    let key = urlB64ToUint8Array Common.vapidKey

                    subscriptWithPushManager sw key
                    |> Promise.bind
                        (fun subscription ->
                            let json =
                                subscription.toJSON () |> JS.JSON.stringify

                            JS.console.log subscription
                            JS.console.log (json)

                            let url =
                                sprintf "%s/subscriptions" Common.backendUrl

                            fetch
                                url
                                [ RequestProperties.Method HttpMethod.POST
                                  RequestProperties.Body !^json
                                  requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                                   Common.authHeader token
                                                   Common.subscriptionHeader ] ])
                    |> Promise.map (fun _ -> infoToast "Notificaties check!")
                | Some sub ->
                    printfn "unsubscribed"
                    sub.unsubscribe () |> Promise.map ignore)
        |> Promise.catchEnd
            (fun err ->
                JS.console.error err
                errorToast "Notificaties aanzetten niet echt gelukt")

    | None -> ()

let private removeSubscription token =
    match navigator.serviceWorker with
    | Some sw ->
        sw.ready
        |> Promise.bind getSubscriptionFromSw
        |> Promise.bind
            (fun sub ->
                match sub with
                | None -> Promise.lift ()
                | Some subscription ->
                    subscription.unsubscribe ()
                    |> Promise.bind
                        (fun _ ->
                            let url =
                                sprintf "%s/subscriptions" Common.backendUrl

                            fetch
                                url
                                [ RequestProperties.Method HttpMethod.DELETE
                                  RequestProperties.Body !^subscription.endpoint
                                  requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                                   Common.authHeader token
                                                   Common.subscriptionHeader ] ])
                    |> Promise.map (fun _ -> infoToast "Notificaties uitgezet!"))
        |> Promise.catchEnd
            (fun err ->
                JS.console.error err
                errorToast "Notificaties uitzetten niet echt gelukt")
    | None -> ()

let private Settings =
    React.functionComponent (
        "SettingsPage",
        (fun () ->
            let (isLoading, setIsLoading) = React.useState (false)
            let eventCtx = React.useContext (eventContext)

            let clearCacheHandler _ =
                setIsLoading true

                eventCtx.ClearCache()
                |> Promise.iter
                    (fun () ->
                        setIsLoading false
                        infoToast "Cache reset!")

            let auth0 = useAuth0 ()
            let (token, setToken) = React.useState ("")
            let roles = useRoles ()

            React.useEffect (
                (fun () ->
                    if auth0.isAuthenticated then
                        auth0.getAccessTokenSilently ()
                        |> Promise.iter setToken),
                [| box auth0.isAuthenticated |]
            )

            let (notifications, setNotifications) = React.useState (false)

            React.useEffectOnce
                (fun () ->
                    hasSubscription ()
                    |> Promise.iter setNotifications)

            let updateNotifications value =
                if value <> notifications then
                    setIsLoading true

                    if value then
                        addSubscription token
                    else
                        removeSubscription token

                    setIsLoading false
                    setNotifications value

            page [] [
                h1 [] [ str "Settings" ]
                button [ classNames [ Bootstrap.Btn
                                      Bootstrap.BtnOutlinePrimary
                                      Bootstrap.My4 ]
                         OnClick clearCacheHandler
                         Disabled isLoading ] [
                    str "Reset cache"
                ]
                h4 [] [ str "Notificaties?" ]
                if browserSupportsNotifications
                   && roles.IsEditorOrAdmin then
                    Switch
                        { TrueLabel = "Aan"
                          FalseLabel = "Uit"
                          OnChange = updateNotifications
                          Disabled = isLoading
                          Value = notifications }
                else
                    div [ classNames [ Bootstrap.Alert
                                       Bootstrap.AlertWarning ] ] [
                        str "Je browser ondersteunt geen notificaties"
                    ]
                if isLoading then
                    loading "Syncen met de server..."
            ])
    )

exportDefault Settings
