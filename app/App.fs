﻿module App

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open type Firebase.Messaging.Exports
open React
open type React.DSL.DOMProps
open ReactRouterDom
open ComponentsDSL

let inline private importPage path =
    React.``lazy`` (fun () -> importDynamic<JSX.ElementType> path)

let private HomePage = importPage "./Pages/Home.fs"
let private OverviewPage = importPage "./Pages/Overview.fs"
let private AddLocationPage = importPage "./Pages/AddLocation.fs"
let private LeaderboardPage = importPage "./Pages/Leaderboard.fs"
let private RulesPage = importPage "./Pages/Rules.fs"
let private LegacyPage = importPage "./Pages/Legacy.fs"
let private LoginPage = importPage "./Pages/Login.fs"
let private SettingsPage = importPage "./Pages/Settings.fs"
let private AdminPage = importPage "./Pages/Admin.fs"

type MessageData =
    {|
        locationId : string
        locationName : string
        userName : string
    |}

type ToastData = {| message : string ; url : string |}

let App () =
    let toastData, setToastData = React.useState<ToastData option> None

    React.useEffect (
        fun () ->
            messaging ()
            |> Promise.iter (fun messaging ->
                onMessage<MessageData> (
                    messaging,
                    (fun message ->
                        Fable.Core.JS.console.log (message)

                        message.data
                        |> Option.iter (fun data ->
                            setToastData (
                                Some (
                                    {|
                                        message = $"%s{data.locationName} door %s{data.userName}"
                                        url = $"/detail/%s{data.locationId}"
                                    |}
                                )
                            )

                            JS.setTimeout (fun () -> setToastData None) 5000 |> ignore
                        )
                    )
                )
            )
        , [||]
    )

    let loader =
        div [
            Style
                {|
                    position = "fixed"
                    zIndex = 100
                    top = "50%"
                    left = "50%"
                    transform = "translate(-50%,-50%)"
                |}
        ] [ loader [] ]

    browserRouter [] [
        navigation [ Key "navigation" ]
        routes [ Key "routes" ] [
            route [
                ReactRouterProp.Index true
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create HomePage []) ])
            ]
            route [
                ReactRouterProp.Path "/overview"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create OverviewPage []) ])
            ]
            route [
                ReactRouterProp.Path "/add-location"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create AddLocationPage []) ])
            ]
            route [
                ReactRouterProp.Path "/leaderboard"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create LeaderboardPage []) ])
            ]
            route [
                ReactRouterProp.Path "/rules"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create RulesPage []) ])
            ]
            route [
                ReactRouterProp.Path "/legacy"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create LegacyPage []) ])
            ]
            route [
                ReactRouterProp.Path "/login"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create LoginPage []) ])
            ]
            route [
                ReactRouterProp.Path "/settings"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create SettingsPage []) ])
            ]
            route [
                ReactRouterProp.Path "/detail/:id"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create HomePage []) ])
            ]
            route [
                ReactRouterProp.Path "/admin"
                ReactRouterProp.Element (suspense [ Fallback loader ] [ (JSX.create AdminPage []) ])
            ]
        // route [ ReactRouterProp.Path "*" ; ReactRouterProp.Element (navigate [ To "/" ]) ]
        ]
        match toastData with
        | None -> null
        | Some toastData ->
            toast [
                Key "toast"
                ToastProp.OnClick (fun () -> setToastData None)
                ToastProp.Title "Nieuwe ronny plekke toegevoegd"
                ToastProp.Body toastData.message
                ToastProp.Url toastData.url
            ]
    ]

document.addEventListener (
    "DOMContentLoaded",
    fun _ ->
        let root = ReactDom.createRoot (document.querySelector "app")
        root.render (strictMode [] [ JSX.create App [] ])
)
