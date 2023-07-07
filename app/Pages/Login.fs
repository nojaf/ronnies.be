﻿module Login

open System
open Fable.Core.JsInterop
open Fable.Core
open Browser
open Browser.Types
open Feliz
open React
open React.Props
open Firebase
open type Firebase.Auth.Exports
open ReactRouterDom

let auth : Auth.Auth = import "auth" "../../firebase.config.js"

[<ReactComponent>]
let LoginPage () =
    let email, setEmail =
        let emailFromLocalStorage = window.localStorage.getItem "email"

        if isNullOrUndefined emailFromLocalStorage then
            ""
        else
            emailFromLocalStorage
        |> React.useState

    let error, setError = React.useState false
    let emailSent, setEmailSent = React.useState false
    let navigate = useNavigate ()

    React.useEffect (
        fun () ->
            if isSignInWithEmailLink (auth, window.location.href) then
                // login with email link
                signInWithEmailLink (auth, email, window.location.href)
                |> Promise.iter (fun _ -> navigate "/")
        , Array.empty
    )

    let onSubmit (ev : Event) =
        setError false
        ev.preventDefault ()

        if String.IsNullOrWhiteSpace email || not (email.Contains "@") then
            setError true
        elif isSignInWithEmailLink (auth, window.location.href) then
            // login with email link
            signInWithEmailLink (auth, email, window.location.href)
            |> Promise.map (fun _ ->
                window.localStorage.setItem ("email", email)
                navigate "/"
            )
            |> Promise.catchEnd (fun err ->
                JS.console.error err
                setError true
            )
        else
            sendSignInLinkToEmail (
                auth,
                email,
                {|
                    handleCodeInApp = true
                    url = window.location.href
                |}
            )
            |> Promise.map (fun () ->
                setEmailSent true
                window.localStorage.setItem ("email", email)
            )
            |> Promise.catchEnd (fun err ->
                JS.console.error err
                setError true
            )

    let emailClass = if error then "error" else ""

    main [] [
        form [ Id "login" ; OnSubmit onSubmit ] [
            h1 [] [ str "Inloggen" ]
            if not emailSent then
                input [
                    Type "email"
                    Placeholder "email"
                    AutoComplete "email"
                    OnChange (fun ev -> setEmail ev.Value)
                    Value email
                    ClassName emailClass
                ]

                input [ Type "submit" ; Class "btn primary" ]
            else
                p [] [ str $"Er werd een email verstuurd naar {email}. Via deze log je in." ]
        ]
    ]
