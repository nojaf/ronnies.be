module ReactMapGL

#nowarn "1182"

open Fable.Core
open Fable.Core.JsInterop
open React.Plugin

let useGeolocation
    : {| enableHighAccuracy : bool |}
          -> {|
              latitude : float
              longitude : float
              loading : bool
              error : obj option
          |} =
    import "useGeolocation" "@uidotdev/usehooks"

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
    [<Emit "latitude">]
    static member Latitude (value : float) = "latitude", box value

    [<Emit "longitude">]
    static member Longitude (value : float) = "longitude", box value

    [<Emit "zoom">]
    static member Zoom (value : int) = "zoom", box value

    [<Emit "onClick">]
    static member OnClick (value : MapLayerMouseEvent -> unit) = "onClick", box value

    [<Emit "mapStyle">]
    static member MapStyle (value : string) = "mapStyle", box value

    [<Emit "mapboxAccessToken">]
    static member MapboxAccessToken (value : string) = "mapboxAccessToken", box value

    [<Emit "onMove">]
    static member OnMove (value : {| viewState : Viewport |} -> unit) = "onMove", box value

let mapboxApiAccessToken : string =
    "pk.eyJ1Ijoibm9qYWYiLCJhIjoiY2p6eHV4ODkwMWNoaTNidXRqeGlrb2JpMSJ9.x6fTQsfCfGMKwxpdKxjhMQ"

/// https://visgl.github.io/react-map-gl/docs/upgrade-guide#map
[<JSX("default as Map", "react-map-gl")>]
let reactMapGL (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null

[<RequireQualifiedAccess>]
type MarkerProp =
    [<Emit "latitude">]
    static member MarkerLatitude (value : float) = "latitude", box value

    [<Emit "longitude">]
    static member MarkerLongitude (value : float) = "longitude", box value

    [<Emit "offsetLeft">]
    static member OffsetLeft (value : int) = "offsetLeft", box value

    [<Emit "offsetTop">]
    static member OffsetTop (value : int) = "offsetTop", box value

[<JSX("Marker", "react-map-gl")>]
let marker (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null
