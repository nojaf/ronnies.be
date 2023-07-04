module App

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Browser.Dom
open Fable.React
open Fable.React.Props
open Feliz

#if DEBUG
importSideEffects "./out/WebSocketClient.js"
#endif

let useMediaQuery (query : string) : bool = import "useMediaQuery" "usehooks-ts"
let useState (initial : 'T) : 'T * ('T -> unit) = import "useState" "react"
let useEffect (callback : unit -> unit) (deps : obj array) : unit = import "useEffect" "react"

[<RequireQualifiedAccess>]
type IconProps =
    | Icon of string
    | Width of int
    | Height of int

    interface IProp

let inline Icon (props : IProp seq) =
    ofImport "Icon" "@iconify/react" (keyValueList CaseRules.LowerFirst props) Array.empty

[<ReactComponent>]
let App () =
    let isTablet = useMediaQuery "screen and (min-width: 960px)"
    let isMenuOpen, setIsMenuOpen = useState true

    useEffect
        (fun () ->
            if isTablet then
                setIsMenuOpen false
        )
        [| isTablet |]

    let menuClass = (if isMenuOpen then "show" else "")

    fragment [] [
        nav [] [
            img [ Src "/images/r-white.png" ]

            button [
                OnClick (fun ev ->
                    ev.preventDefault ()
                    setIsMenuOpen (not isMenuOpen)
                )
            ] [
                Icon [ IconProps.Icon "ic:baseline-menu" ; IconProps.Width 24 ; IconProps.Height 24 ]
            ]
            ul [ ClassName menuClass ] [
                li [ ClassName "active" ] [ str "Overzicht" ]
                li [] [ str "E nieuwen toevoegen" ]
                li [] [ str "Klassement" ]
                li [] [ str "Manifesto" ]
                li [] [ str "Bearer" ]
            ]
        ]
        main [] [ h1 [] [ str "meh" ] ]
    ]

ReactDom.render (App (), document.querySelector "body")
