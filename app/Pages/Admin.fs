module Admin

open Fable.Core
open React
open type React.DSL.DOMProps
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open ComponentsDSL

[<RequireQualifiedAccess>]
type AddUserState =
    | Initial
    | Error of string
    | Loading
    | Success of string

let AddUser () =
    let name, setName = React.useState<string> ""
    let email, setEmail = React.useState<string> ""
    let state, setState = React.useStateByFunction<AddUserState> AddUserState.Initial

    let onSubmit (e : Browser.Types.Event) =
        e.preventDefault ()
        setState (fun _ -> AddUserState.Loading)

        API.addUser name email
        |> Promise.map (fun _user ->
            setState (fun _ -> AddUserState.Success $"{name} ({email}) was added!")
            setName ""
            setEmail ""
        )
        |> Promise.catchEnd (fun e -> setState (fun _ -> AddUserState.Error e.Message))

    let fields =
        [
            div [ Key "name-container" ] [
                label [] [ str "Naam*" ]
                input [
                    Name "name"
                    Required true
                    DefaultValue name
                    OnChange (fun (ev : Browser.Types.Event) -> setName ev.Value)
                ]
            ]
            div [ Key "email-container" ] [
                label [] [ str "Email*" ]
                input [
                    Name "email"
                    Required true
                    Type "email"
                    DefaultValue email
                    OnChange (fun ev -> setEmail ev.Value)
                ]
            ]
        ]

    let submitButton =
        div [ ClassName "align-right" ; Key "submit-container" ] [
            input [ Type "submit" ; Value "Toevoegen!" ; ClassName "primary" ]
        ]

    form [ OnSubmit onSubmit ] [
        match state with
        | AddUserState.Initial ->
            yield! fields
            yield submitButton
        | AddUserState.Error error ->
            yield! fields
            yield p [ ClassName "error" ] [ str error ]
            yield submitButton
        | AddUserState.Loading -> yield loader []
        | AddUserState.Success msg ->
            yield! fields
            yield submitButton
            yield p [ ClassName "success" ] [ str msg ]
    ]

let AdminPage () =
    let tokenResult, loading, _ = useAuthIdTokenResult<CustomClaims> auth

    main [ Id "admin" ] [
        if loading then
            loader []
        else

        match tokenResult with
        | Some tokenResult when tokenResult.claims.admin ->
            h1 [ Key "title" ] [ str "Admin" ]
            h2 [ Key "add-user-title" ] [ str "Add user" ]
            JSX.create AddUser []
        | _ -> h1 [ Key "unauthorized" ] [ str "Unauthorized" ]
    ]
