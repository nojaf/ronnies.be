module ReactMapGL

open Fable.Core.JsInterop
open React
open React.Plugin
open React.DSL

let useGeolocation
    : {| enableHighAccuracy : bool |}
          -> {|
              latitude : float
              longitude : float
              loading : bool
              error : obj option
          |} =
    import "useGeolocation" "react-use"

type Viewport =
    {|
        latitude : float
        longitude : float
        zoom : int
    |}

type LngLat =
    abstract lng : float
    abstract lat : float

type MapLayerMouseEvent =
    inherit Browser.Types.Event

    abstract lngLat : LngLat

// TODO: https://visgl.github.io/react-map-gl/docs/get-started
[<RequireQualifiedAccess>]
type ReactMapGLProp =
    | Latitude of float
    | Longitude of float
    | Zoom of int
    | OnClick of (MapLayerMouseEvent -> unit)
    | MapStyle of string
    | MapboxAccessToken of string
    | OnMove of ({| viewState : Viewport |} -> unit)

    interface IProp

let mapboxApiAccessToken : string =
    "pk.eyJ1Ijoibm9qYWYiLCJhIjoiY2p6eHV4ODkwMWNoaTNidXRqeGlrb2JpMSJ9.x6fTQsfCfGMKwxpdKxjhMQ"

/// https://visgl.github.io/react-map-gl/docs/upgrade-guide#map
[<JSX("default", "react-map-gl")>]
let reactMapGL (props : IProp seq) (children : ReactElement seq) : ReactElement = null
// let allProps =
//     [|
//         yield ReactMapGLProp.MapboxAccessToken mapboxApiAccessToken :> IProp
//         for p in props do
//             yield p :> IProp
//     |]

// ofImport "default" "react-map-gl" allProps children

type MarkerProp =
    | [<CompiledName("latitude")>] MarkerLatitude of float
    | [<CompiledName("longitude")>] MarkerLongitude of float
    | OffsetLeft of int
    | OffsetTop of int

    interface IProp

[<JSX("Marker", "react-map-gl")>]
let marker (props : IProp list) (children : ReactElement seq) : ReactElement =
    jsxTransformFallback (import "Marker" "react-map-gl") props children
