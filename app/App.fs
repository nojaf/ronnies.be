module App

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open React
open type React.DSL.DOMProps
open ReactRouterDom
open UseHooksTs
open Iconify
open StyledComponents
open ComponentsDSL

let StyledNav : JSX.ElementType =
    mkStyleComponent
        "nav"
        """
& {

}
"""

let LogoutComponent () =
    let navigate = useNavigate ()

    let logoutHandler (ev : Browser.Types.Event) =
        ev.preventDefault ()
        signOut auth |> Promise.iter (fun () -> navigate "/")

    a [ Href "#" ; OnClick logoutHandler ] [ str "uitloggen" ]

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

let App () =
    let isTablet = useMediaQuery "screen and (min-width: 960px)"
    let isMenuOpen, setIsMenuOpen = React.useState false
    let user, _, _ = useAuthState auth
    let tokenResult, _, _ = useAuthIdTokenResult<CustomClaims> auth

    React.useEffect (
        fun () ->
            if isTablet then
                setIsMenuOpen false
        , [| isTablet |]
    )

    let menuClass = (if isMenuOpen then "show" else "")

    let mkNavLinkAux too id content =
        li [
            Key too
            OnClick (fun _ -> setIsMenuOpen false)
            Id (if System.String.IsNullOrWhiteSpace id then null else id)
        ] [ navLink [ ReactRouterProp.To too ; Key too ] [ content ] ]

    let mkNavLink too text = mkNavLinkAux too "" (str text)
    let loader = loader []

    browserRouter [] [
        nav [] [
            link [ ReactRouterProp.To "/" ; OnClick (fun _ -> setIsMenuOpen false) ] [
                img [ Src "/images/r-white.png" ]
            ]
            button [
                OnClick (fun ev ->
                    ev.preventDefault ()
                    setIsMenuOpen (not isMenuOpen)
                )
            ] [
                icon [
                    Key "mobile-menu-icon"
                    IconProp.Icon "ic:baseline-menu"
                    IconProp.Width 24
                    IconProp.Height 24
                ]
            ]
            ul [ ClassName menuClass ] [
                yield mkNavLink "/overview" "Overzicht"
                yield mkNavLink "/legacy" "De vorige keer"

                match user with
                | None -> yield mkNavLink "/login" "Inloggen"
                | Some user ->
                    yield mkNavLink "/add-location" "E nieuwen toevoegen"
                    yield mkNavLink "/leaderboard" "Klassement"
                    yield mkNavLink "/rules" "Manifesto"

                    match tokenResult with
                    | Some tokenResult when tokenResult.claims.admin -> yield mkNavLink "/admin" "Admin"
                    | _ -> ()

                    yield li [ Key "logout" ; OnClick (fun _ -> setIsMenuOpen false) ] [ JSX.create LogoutComponent [] ]

                    yield
                        mkNavLinkAux
                            "/settings"
                            "user"
                            (fragment [] [
                                icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
                                str user.displayName
                            ])
            ]
        ]
        routes [] [
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
    ]

document.addEventListener (
    "DOMContentLoaded",
    fun _ ->
        let root = ReactDom.createRoot (document.querySelector "app")
        root.render (strictMode [] [ JSX.create App [] ])
)
