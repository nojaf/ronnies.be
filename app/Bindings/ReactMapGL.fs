module ReactMapGL

open Fable.Core
open Fable.Core.JsInterop
open React

let useGeolocation
    : unit
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

type MapClickEvent =
    inherit Browser.Types.Event

    [<Emit("$0.lngLat.reverse()")>]
    abstract LatLng : unit -> (float * float)

// TODO: https://visgl.github.io/react-map-gl/docs/get-started
type ReactMapGLProp =
    | Latitude of float
    | Longitude of float
    | Zoom of int
    | OnViewportChange of (Viewport -> unit)
    | OnClick of (MapClickEvent -> unit)
    | [<CompiledName("className")>] MapClassName of string
    | MapStyle of string
    | MapboxAccessToken of string

    interface IProp

let mapboxApiAccessToken : string =
    "pk.eyJ1Ijoibm9qYWYiLCJhIjoiY2p6eHV4ODkwMWNoaTNidXRqeGlrb2JpMSJ9.x6fTQsfCfGMKwxpdKxjhMQ"

// https://visgl.github.io/react-map-gl/docs/upgrade-guide#map
let inline ReactMapGL (props : #IProp seq) (children : ReactElement seq) : ReactElement =
    let allProps =
        [|
            yield MapboxAccessToken mapboxApiAccessToken :> IProp
            for p in props do
                yield p :> IProp
        |]

    ofImport "default" "react-map-gl" allProps children

type MarkerProp =
    | [<CompiledName("key")>] MarkerKey of string
    | [<CompiledName("latitude")>] MarkerLatitude of float
    | [<CompiledName("longitude")>] MarkerLongitude of float
    | OffsetLeft of int
    | OffsetTop of int

    interface IProp

let inline Marker (props : MarkerProp list) (children : ReactElement seq) : ReactElement =
    ofImport "Marker" "react-map-gl" props children

// Todo: use other icon
let UserIcon = str "icon"
