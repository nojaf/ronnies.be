module Ronnies.Client.Components.Navigation

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Styles

let To url = HTMLAttr.Custom("to", url)

let Link (props : IHTMLProp seq) (children : ReactElement seq) =
    ofImport "Link" "@reach/router" (keyValueList Fable.Core.CaseRules.LowerFirst props) children

let private Navigation =
    React.functionComponent
        ("Navigation",
         (fun () ->
             let (mobileOpen, setMobileOpen) = React.useState (false)

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
                         li [ classNames [ Bootstrap.NavItem ] ] [
                             Link [ To "/add-location"
                                    ClassName Bootstrap.NavLink
                                    OnClick(fun _ -> setMobileOpen (false)) ] [
                                 str "E nieuwen toevoegen"
                             ]
                         ]
                     ]
                 ]
             ]))

exportDefault Navigation
