module Ronnies.Client.Components.Navigation

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open Auth0
open Ronnies.Client.Styles

let To url = HTMLAttr.Custom("to", url)

let Link (props : IHTMLProp seq) (children : ReactElement seq) =
    ofImport "Link" "@reach/router" (keyValueList Fable.Core.CaseRules.LowerFirst props) children

let useNavigate () : string -> unit = import "useNavigate" "@reach/router"

[<ReactComponent>]
let private Navigation () =

    let (mobileOpen, setMobileOpen) = React.useState (false)
    let auth0 = useAuth0 ()
    let roles = useRoles ()

    let onLoginClick (ev : MouseEvent) =
        ev.preventDefault ()
        auth0.loginWithRedirect () |> Promise.start

    let onLogoutClick (ev : MouseEvent) =
        ev.preventDefault ()

        auth0.logout { returnTo = Browser.Dom.window.location.origin }
        |> ignore

    let loginLink =
        li [ classNames [ Bootstrap.NavItem ] ] [
            a [ Href "#"
                ClassName Bootstrap.NavLink
                OnClick onLoginClick ] [
                str "Inloggen"
            ]
        ]

    let userElement (user : Auth0User) =
        li [ classNames [ Bootstrap.NavItem ] ] [
            Link [ classNames [ Bootstrap.NavLink
                                Bootstrap.Active ]
                   To "/settings"
                   OnClick(fun _ -> setMobileOpen (false)) ] [
                img [ Src user.picture
                      Style [ MarginTop "-2px" ]
                      classNames [ Bootstrap.Avatar ]
                      Alt "user avatar" ]
                str user.nickname
            ]
        ]

    let logoutLink =
        li [ classNames [ Bootstrap.NavItem ] ] [
            a [ Href "#"
                ClassName Bootstrap.NavLink
                OnClick onLogoutClick ] [
                str "logout"
            ]
        ]

    let rightNavbar =
        ul [ ClassName Bootstrap.NavbarNav ] [
            if not auth0.isAuthenticated then
                loginLink
            else
                yield! [ logoutLink; userElement auth0.user ]
        ]

    let menuLink path label =
        li [ classNames [ Bootstrap.NavItem ]
             Key(sprintf "menu-%s" path) ] [
            Link [ To path
                   ClassName Bootstrap.NavLink
                   OnClick(fun _ -> setMobileOpen (false)) ] [
                str label
            ]
        ]

    let logAuthToken () =
        if not auth0.isLoading && auth0.isAuthenticated then
            auth0.getAccessTokenSilently ()
            |> Promise.iter (fun token -> Fable.Core.JS.console.log token)

    let adminItems =
        if roles.IsAdmin then
            [ li [ classNames [ Bootstrap.NavItem ]
                   Key "bearerButton"
                   OnClick
                       (fun ev ->
                           ev.preventDefault ()
                           logAuthToken ()) ] [
                a [ Href "#"
                    ClassName Bootstrap.NavLink ] [
                    str "Bearer"
                ]
              ] ]
        else
            []

    let editorItems =
        if roles.IsEditorOrAdmin then
            [ menuLink "/add-location" "E nieuwen toevoegen"
              menuLink "/leaderboard" "Klassement"
              menuLink "/rules" "Manifesto" ]
        else
            []

    nav [ classNames [ Bootstrap.Navbar
                       Bootstrap.NavbarExpandMd
                       Bootstrap.NavbarDark
                       Bootstrap.BgPrimary ] ] [
        Link [ To "/"
               ClassName Bootstrap.NavbarBrand
               OnClick(fun _ -> setMobileOpen false) ] [
            img [ Src "/assets/r-white.png"
                  Alt "logo ronnies.be" ]
        ]
        button [ ClassName Bootstrap.NavbarToggler
                 OnClick(fun _ -> setMobileOpen (not mobileOpen)) ] [
            span [ ClassName Bootstrap.NavbarTogglerIcon ] []
        ]
        div [ classNames [ Bootstrap.Collapse
                           Bootstrap.NavbarCollapse
                           if mobileOpen then
                               Bootstrap.Show ] ] [
            ul [ classNames [ Bootstrap.NavbarNav
                              Bootstrap.MrAuto ] ] [
                menuLink "/overview" "Overzicht"
                ofList editorItems
                ofList adminItems
            ]
            if not auth0.isLoading then
                rightNavbar
        ]
    ]

exportDefault Navigation
