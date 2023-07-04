module App

open Fable.Core.JsInterop
open Browser.Dom
open Fable.React
open Fable.React.Props
open Feliz
open Bindings

#if DEBUG
importSideEffects "./out/WebSocketClient.js"
#endif

[<ReactComponent>]
let App () =
    let isTablet = useMediaQuery "screen and (min-width: 960px)"
    let isMenuOpen, setIsMenuOpen = React.useState false

    React.useEffect
        (fun () ->
            if isTablet then
                setIsMenuOpen false
        )
        [| isTablet |]

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
                mkNavLink "/overview" "Overzicht"
                mkNavLink "/add-location" "E nieuwen toevoegen"
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
            ]
        ]
    ]

ReactDom.render (App (), document.querySelector "body")
