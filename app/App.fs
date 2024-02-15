module App

open Fable.Core.JsInterop
open Browser.Dom
open Firebase
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open React
open React.DSL
open React.DSL.Props
open ReactRouterDom
open UseHooksTs
open Iconify

let LogoutComponent () =
    let navigate = useNavigate ()

    let logoutHandler (ev : Browser.Types.Event) =
        ev.preventDefault ()
        signOut auth |> Promise.iter (fun () -> navigate "/")

    a [ Href "#" ; OnClick logoutHandler ] [ str "uitloggen" ]

let private HomePage = React.``lazy`` (fun () -> importDynamic<obj> "./Pages/Home")

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
            if not (System.String.IsNullOrWhiteSpace id) then
                Id id
        ] [ navLink [ To too ; Key too ] [ content ] ]

    let mkNavLink too text = mkNavLinkAux too "" (str text)

    browserRouter [] [
        nav [] [
            link [ To "/" ; OnClick (fun _ -> setIsMenuOpen false) ] [ img [ Src "/images/r-white.png" ] ]
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

                    yield li [ Key "logout" ; OnClick (fun _ -> setIsMenuOpen false) ] [ ofComponent LogoutComponent ]

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
                ReactRouterProp.Element (suspense [ Fallback (p [] [ str "fallback" ]) ] [ (ofComponent HomePage) ])
            ]
            route [
                ReactRouterProp.Path "/overview"
                ReactRouterProp.Element (Overview.OverviewPage ())
            ]
            route [
                ReactRouterProp.Path "/add-location"
                ReactRouterProp.Element (ofComponent AddLocation.AddLocationPage)
            ]
            route [
                ReactRouterProp.Path "/leaderboard"
                ReactRouterProp.Element (ofComponent Leaderboard.LeaderboardPage)
            ]
            route [
                ReactRouterProp.Path "/rules"
                ReactRouterProp.Element (ofComponent Rules.RulesPage)
            ]
            route [
                ReactRouterProp.Path "/legacy"
                ReactRouterProp.Element (ofComponent Legacy.LegacyPage)
            ]
            route [
                ReactRouterProp.Path "/login"
                ReactRouterProp.Element (ofComponent Login.LoginPage)
            ]
            route [
                ReactRouterProp.Path "/settings"
                ReactRouterProp.Element (ofComponent Settings.SettingsPage)
            ]
            route [
                ReactRouterProp.Path "/detail/:id"
                ReactRouterProp.Element (suspense [ Fallback (p [] [ str "fallback" ]) ] [ (ofComponent HomePage) ])
            ]
            route [
                ReactRouterProp.Path "/admin"
                ReactRouterProp.Element (ofComponent Admin.AdminPage)
            ]
            // route [ ReactRouterProp.Path "*" ; ReactRouterProp.Element (navigate [ To "/" ]) ]
        ]
    ]

document.addEventListener (
    "DOMContentLoaded",
    fun _ ->
        let root = ReactDom.createRoot (document.querySelector "app")
        root.render (strictMode [] [ ofComponent App ])
)
