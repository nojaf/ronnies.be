module Ronnies.Client.Views.AddLocationPage.View

open System
open Fable.React
open Ronnies.Domain
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Styles
open Ronnies.Client.Components

type Model = { Name: string; Price: string }

let private useGeolocation: unit -> {| latitude: float
                                       longitude: float
                                       loading: bool |} = import "useGeolocation" "react-use"

type private Viewport =
    { width: string
      height: string
      latitude: float
      longitude: float
      zoom: int }

type private MapClickEvent =
    inherit Browser.Types.Event

    [<Emit("$0.lngLat.reverse()")>]
    abstract LatLng: unit -> (float * float)

type private ReactMapGLProp =
    | [<CompiledName("width")>] MapWidth of string
    | [<CompiledName("height")>] MapHeight of string
    | Latitude of float
    | Longitude of float
    | Zoom of int
    | OnViewportChange of (Viewport -> unit)
    | OnClick of (MapClickEvent -> unit)
    | [<CompiledName("className")>] MapClassName of string
    | MapStyle of string
    | MapboxApiAccessToken of string

let inline private ReactMapGL (props: ReactMapGLProp list) (children: ReactElement seq): Fable.React.ReactElement =
    ofImport "default" "react-map-gl" (keyValueList Fable.Core.CaseRules.LowerFirst props) children

type private MarkerProp =
    | [<CompiledName("key")>] MarkerKey of string
    | [<CompiledName("latitude")>] MarkerLatitude of float
    | [<CompiledName("longitude")>] MarkerLongitude of float
    | OffsetLeft of int
    | OffsetTop of int

let inline private Marker (props: MarkerProp list) (children: ReactElement seq): Fable.React.ReactElement =
    ofImport "Marker" "react-map-gl" (keyValueList Fable.Core.CaseRules.LowerFirst props) children

let UserIcon =
    svg [ HTMLAttr.Custom("xmlns", "http://www.w3.org/2000/svg")
          HTMLAttr.Custom("x", "0px")
          HTMLAttr.Custom("y", "0px")
          SVGAttr.Width 24
          SVGAttr.Height 24
          HTMLAttr.Custom("viewBox", "0 0 172 172")
          Style [ Fill "#000" ] ] [
        g [ HTMLAttr.Custom("fill", "none")
            HTMLAttr.Custom("fillRule", "nonzero")
            HTMLAttr.Custom("stroke", "none")
            HTMLAttr.Custom("strokeWidth", "{1}")
            HTMLAttr.Custom("strokeLinecap", "butt")
            HTMLAttr.Custom("strokeLinejoin", "miter")
            HTMLAttr.Custom("strokeMiterlimit", "{10}")
            HTMLAttr.Custom("strokeDasharray", "{1}")
            HTMLAttr.Custom("strokeDashoffset", "{0}")
            HTMLAttr.Custom("fontFamily", "none")
            HTMLAttr.Custom("fontWeight", "none")
            HTMLAttr.Custom("fontSize", "none")
            HTMLAttr.Custom("textAnchor", "none") ] [
            path [ HTMLAttr.Custom("d", "M0,172v-172h172v172z")
                   HTMLAttr.Custom("fill", "none") ] []
            g [ HTMLAttr.Custom("fill", "#791716") ] [
                path [ HTMLAttr.Custom
                           ("d",
                            "M106.41156,111.89406c-0.51063,-5.54969 -0.30906,-9.41969 -0.30906,-14.48563c2.51281,-1.31687 7.01438,-9.71531 7.76688,-16.81031c1.97531,-0.16125 5.09281,-2.08281 6.00656,-9.68844c0.48375,-4.085 -1.46469,-6.38281 -2.66062,-7.10844c3.21156,-9.66156 9.89,-39.56 -12.33563,-42.65062c-2.29781,-4.01781 -8.15656,-6.04688 -15.76219,-6.04688c-30.46281,0.56438 -34.13125,23.005 -27.45281,48.6975c-1.19594,0.72563 -3.14437,3.02344 -2.66062,7.10844c0.92719,7.60563 4.03125,9.52719 6.00656,9.68844c0.7525,7.095 5.42875,15.49344 7.955,16.81031c0,5.06594 0.18812,8.93594 -0.3225,14.48563c-6.02,16.20562 -46.68188,11.65031 -48.56313,42.90594h130.72c-1.88125,-31.25562 -42.36844,-26.70031 -48.38844,-42.90594z") ] []
            ]
        ]
    ]


let private LocationPicker =
    React.functionComponent (fun () ->
        let geolocation = useGeolocation ()
        let (userLatitude, setUserLatitude) = React.useState (50.946139)
        let (userLongitude, setUserLongitude) = React.useState (3.138671)
        let (ronnyLatitude, setRonnyLatitude) = React.useState (userLatitude)
        let (ronnyLongitude, setRonnyLongitude) = React.useState (userLongitude)

        let (viewport, setViewport) =
            React.useState
                ({ width = "100%"
                   height = "100%"
                   latitude = userLatitude
                   longitude = userLongitude
                   zoom = 16 })

        React.useEffect
            ((fun () ->
                if not geolocation.loading then
                    setUserLatitude (geolocation.latitude)
                    setUserLongitude (geolocation.longitude)
                    setViewport
                        ({ width = "100%"
                           height = "100%"
                           latitude = geolocation.latitude
                           longitude = geolocation.longitude
                           zoom = 16 })
                    setRonnyLatitude (geolocation.latitude)
                    setRonnyLongitude (geolocation.longitude)),
             [| box geolocation.loading |])

        let onMapClick (ev: MapClickEvent) =
            let (lat, lng) = ev.LatLng()
            setRonnyLatitude lat
            setRonnyLongitude lng

        if geolocation.loading then
            loading "Locatie ant zoeken"
        else
            ReactMapGL [ OnViewportChange setViewport
                         OnClick onMapClick
                         MapWidth viewport.width
                         MapHeight viewport.height
                         Latitude viewport.latitude
                         Longitude viewport.longitude
                         Zoom viewport.zoom
                         MapClassName "add-location-map"
                         MapStyle "mapbox://styles/nojaf/ck7846xt30p791inshughvnc0"
                         MapboxApiAccessToken
                             "pk.eyJ1Ijoibm9qYWYiLCJhIjoiY2p6eHV4ODkwMWNoaTNidXRqeGlrb2JpMSJ9.x6fTQsfCfGMKwxpdKxjhMQ" ] [
                Marker [ MarkerKey "ronny"
                         MarkerLatitude ronnyLatitude
                         MarkerLongitude ronnyLongitude
                         OffsetTop 0
                         OffsetLeft 0 ] [
                    img [ Src "/assets/ronny.png"
                          HTMLAttr.Width 24
                          HTMLAttr.Height 24 ]
                ]
                Marker [ MarkerKey "user"
                         MarkerLatitude userLatitude
                         MarkerLongitude userLongitude
                         OffsetTop 0
                         OffsetLeft 0 ] [
                    UserIcon
                ]
            ])

let AddLocationPage =
    React.functionComponent (fun () ->
        div [ classNames [ Bootstrap.Container
                           Bootstrap.P3 ] ] [
            h1 [] [ str "E nieuwen toevoegen" ]
            form [ classNames [ Bootstrap.ColMd6
                                Bootstrap.P0 ] ] [
                div [ ClassName Bootstrap.FormGroup ] [
                    label [] [ str "Naam*" ]
                    input [ ClassName Bootstrap.FormControl ]
                ]
                div [ ClassName Bootstrap.FormGroup ] [
                    label [] [ str "Prijs*" ]
                    div [ ClassName Bootstrap.InputGroup ] [
                        input [ ClassName Bootstrap.FormControl ]
                        div [ ClassName Bootstrap.InputGroupAppend ] [
                            button [ classNames [ Bootstrap.Btn
                                                  Bootstrap.BtnOutlineSecondary
                                                  Bootstrap.DropdownToggle ] ] [ str "EUR" ]
                            div [ ClassName Bootstrap.DropdownMenu ] [
                                a [Href "#"; ClassName Bootstrap.DropdownItem; Key "eur" ] [ str "EUR" ]
                                a [Href "#"; ClassName Bootstrap.DropdownItem; Key "usd" ] [ str "USD" ]
                            ]
                        ]
                    ]
                ]
                div [] [
                    label [] [ str "Locatie*" ]
                    br []
                    div [ Id "locationPickerContainer" ] [
                        LocationPicker()
                    ]
                ]
                div [ ClassName Bootstrap.FormGroup ] [
                    label [] [ str "Ist van vat?" ]
                    br []
                    div [ ClassName Bootstrap.BtnGroup ] [
                        button [ classNames [ Bootstrap.Btn
                                              Bootstrap.BtnPrimary ] ] [
                            str "Nint"
                        ]
                        button [ classNames [ Bootstrap.Btn
                                              Bootstrap.BtnOutlinePrimary ] ] [
                            str "Joat"
                        ]
                    ]
                ]
                div [ ClassName Bootstrap.FormGroup ] [
                    label [] [ str "Opmerking" ]
                    textarea [ ClassName Bootstrap.FormControl ] []
                ]
                div [ classNames [ Bootstrap.TextRight
                                   Bootstrap.Pb2 ] ] [
                    button [ classNames [ Bootstrap.Btn
                                          Bootstrap.BtnPrimary ] ] [
                        str "Save!"
                    ]
                ]
            ]
        ])
