module App

open Fable.Core.JsInterop
open Browser.Dom
open Fable.React
open Fable.React.Props
open Feliz
open Firebase
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open Bindings

#if DEBUG
importSideEffects "./out/WebSocketClient.js"
#endif

let auth : Auth.Auth = import "auth" "../firebase.config.js"

[<ReactComponent>]
let LogoutComponent () =
    let navigate = useNavigate ()

    let logoutHandler (ev : Browser.Types.Event) =
        ev.preventDefault ()
        signOut auth |> Promise.iter (fun () -> navigate "/")

    a [ Href "#" ; OnClick logoutHandler ] [ str "uitloggen" ]

[<ReactComponent>]
let App () =
    let isTablet = useMediaQuery "screen and (min-width: 960px)"
    let isMenuOpen, setIsMenuOpen = React.useState false
    let user, _, _ = useAuthState auth

    React.useEffect (
        fun () ->
            if isTablet then
                setIsMenuOpen false
        , [| isTablet |]
    )

    let menuClass = (if isMenuOpen then "show" else "")

    let mkNavLink too text =
        li [ OnClick (fun _ -> setIsMenuOpen false) ] [ NavLink [ To too ] [ str text ] ]

    BrowserRouter [
        nav [] [
            Link [ To "/" ; OnClick (fun _ -> setIsMenuOpen false) ] [ img [ Src "/images/r-white.png" ] ]
            button [
                OnClick (fun ev ->
                    ev.preventDefault ()
                    setIsMenuOpen (not isMenuOpen)
                )
            ] [
                Icon [ IconProps.Icon "ic:baseline-menu" ; IconProps.Width 24 ; IconProps.Height 24 ]
            ]
            ul [ ClassName menuClass ] [
                yield mkNavLink "/overview" "Overzicht"
                yield mkNavLink "/add-location" "E nieuwen toevoegen"
                match user with
                | None -> yield mkNavLink "/login" "Inloggen"
                | Some user ->
                    yield li [ OnClick (fun _ -> setIsMenuOpen false) ] [ LogoutComponent () ]

                    yield
                        li [ Id "user" ; OnClick (fun _ -> setIsMenuOpen false) ] [
                            Icon [
                                IconProps.Icon "clarity:user-line"
                                IconProps.Height 24
                                IconProps.Width 24
                            ]
                            str user.displayName
                        ]

            // li [] [ NavLink "add-location" [ str "Klassement" ] ]
            // li [] [ NavLink "add-location" [ str "Manifesto" ] ]
            // li [] [ NavLink "add-location" [ str "Bearer" ] ]
            ]
        ]
        main [] [
            Routes [
                Route
                    {|
                        index = true
                        element = h1 [] [ str "Home" ]
                    |}
                Route
                    {|
                        path = "/overview"
                        element = h1 [] [ str "Overview" ]
                    |}
                Route
                    {|
                        path = "/add-location"
                        element = h1 [] [ str "Add new" ]
                    |}
                Route
                    {|
                        path = "/login"
                        element = Login.LoginPage ()
                    |}
            ]
        ]
    ]

ReactDom.render (App (), document.querySelector "body")
