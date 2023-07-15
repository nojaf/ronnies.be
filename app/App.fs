module App

open Fable.Core.JsInterop
open Browser.Dom
open Feliz
open Firebase
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open React
open React.Props
open ReactRouterDom
open UseHooksTs
open Iconify

#if DEBUG
importSideEffects "./out/WebSocketClient.js"
#endif

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
                Icon [ IconProp.Icon "ic:baseline-menu" ; IconProp.Width 24 ; IconProp.Height 24 ]
            ]
            ul [ ClassName menuClass ] [
                yield mkNavLink "/overview" "Overzicht"

                match user with
                | None -> yield mkNavLink "/login" "Inloggen"
                | Some user ->
                    yield mkNavLink "/add-location" "E nieuwen toevoegen"
                    yield mkNavLink "/leaderboard" "Klassement"
                    yield li [ OnClick (fun _ -> setIsMenuOpen false) ] [ LogoutComponent () ]

                    yield
                        li [ Id "user" ; OnClick (fun _ -> setIsMenuOpen false) ] [
                            Icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
                            str user.displayName
                        ]

            // li [] [ NavLink "add-location" [ str "Manifesto" ] ]
            // li [] [ NavLink "add-location" [ str "Bearer" ] ]
            ]
        ]
        Routes [
            Route [ ReactRouterProp.Index true ; ReactRouterProp.Element (Home.HomePage ()) ]
            Route [
                ReactRouterProp.Path "/overview"
                ReactRouterProp.Element (Overview.OverviewPage ())
            ]
            Route [
                ReactRouterProp.Path "/add-location"
                ReactRouterProp.Element (AddLocation.AddLocationPage ())
            ]
            Route [
                ReactRouterProp.Path "/leaderboard"
                ReactRouterProp.Element (Leaderboard.LeaderboardPage ())
            ]
            Route [ ReactRouterProp.Path "/login" ; ReactRouterProp.Element (Login.LoginPage ()) ]
            Route [ ReactRouterProp.Path "*" ; ReactRouterProp.Element (Navigate [ To "/" ]) ]
            Route [
                ReactRouterProp.Path "/detail/:id"
                ReactRouterProp.Element (Home.HomePage ())
            ]
        ]
    ]

ReactDom.render (App (), document.querySelector "body")
