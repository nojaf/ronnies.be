module Ronnies.Client.Components.Navigation

open Browser.Types
open Fable.Core
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

let private Navigation =
    React.functionComponent
        ("Navigation",
         (fun () ->
             let (mobileOpen, setMobileOpen) = React.useState (false)
             let (roles, setRoles) = React.useState ([||])
             let auth0 = useAuth0 ()

             let onLoginClick (ev : MouseEvent) =
                 ev.preventDefault ()
                 auth0.loginWithRedirect () |> Promise.start

             let onLogoutClick (ev : MouseEvent) =
                 ev.preventDefault ()
                 auth0.logout ()

             let loginLink =
                 li [ classNames [ Bootstrap.NavItem ] ] [
                     a [ Href "#"
                         ClassName Bootstrap.NavLink
                         OnClick onLoginClick ] [
                         str "Inloggen"
                     ]
                 ]

             let userElement (user : Auth0.Auth0User) =
                 li [ classNames [ Bootstrap.NavItem ] ] [
                     span [ classNames [ Bootstrap.NavLink
                                         Bootstrap.Active
                                         Bootstrap.DefaultCursor ] ] [
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

             React.useEffect
                 ((fun () ->
                     if not auth0.isLoading && auth0.isAuthenticated then
                         auth0.getIdTokenClaims ()
                         |> Promise.iter (fun claims -> setRoles (claims.roles))),
                  [| box auth0.user
                     box auth0.isLoading |])

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
                 if Array.contains "admin" roles then
                     [ li [ classNames [ Bootstrap.NavItem ]
                            Key "bearerButton"
                            OnClick(fun ev ->
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
                 if Array.contains "admin" roles
                    || Array.contains "editor" roles then
                     [ menuLink "/add-location" "E nieuwen toevoegen" ]
                 else
                     []

             nav [ classNames [ Bootstrap.Navbar
                                Bootstrap.NavbarExpandMd
                                Bootstrap.NavbarDark
                                Bootstrap.BgPrimary ] ] [
                 Link [ To "/"
                        ClassName Bootstrap.NavbarBrand
                        OnClick(fun _ -> setMobileOpen (not mobileOpen)) ] [
                     img [ Src "assets/r-white.png"
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
                         ofList editorItems
                         ofList adminItems
                     ]
                     if not auth0.isLoading then
                         rightNavbar
                 ]
             ]))

exportDefault Navigation
