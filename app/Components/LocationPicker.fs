module Components.LocationPicker

open Fable.Core
open Fable.Core.JsInterop
open React
open type React.DSL.DOMProps
open ReactMapGL
open Iconify

type LocationPickerProps =
    {|
        onChange : LatLng * LatLng -> unit
        existingLocations : (string * LatLng) array
    |}

let LocationPicker (props : LocationPickerProps) =
    let geolocation = useGeolocation {| enableHighAccuracy = true |}
    let userLatitude, setUserLatitude = React.useState 50.946139
    let userLongitude, setUserLongitude = React.useState 3.138671
    let ronnyLatitude, setRonnyLatitude = React.useState userLatitude
    let ronnyLongitude, setRonnyLongitude = React.useState userLongitude

    let viewport, setViewport =
        React.useState<Viewport>
            {|
                latitude = userLatitude
                longitude = userLongitude
                zoom = 16
            |}

    React.useEffect (
        (fun () ->
            if not geolocation.loading then
                setUserLatitude geolocation.latitude
                setUserLongitude geolocation.longitude

                setViewport
                    {|
                        latitude = geolocation.latitude
                        longitude = geolocation.longitude
                        zoom = 16
                    |}

                setRonnyLatitude geolocation.latitude
                setRonnyLongitude geolocation.longitude

                props.onChange (
                    (geolocation.latitude, geolocation.longitude),
                    (geolocation.latitude, geolocation.longitude)
                )
        ),
        [| box geolocation.loading |]
    )

    let onMapClick (ev : MapLayerMouseEvent) =
        let lngLat = ev.lngLat
        setRonnyLatitude lngLat.lat
        setRonnyLongitude lngLat.lng
        props.onChange ((userLatitude, userLongitude), (lngLat.lat, lngLat.lng))

    let existingRonnies =
        props.existingLocations
        |> Array.map (fun (name, (lat, lng)) ->
            marker [
                Key $"existing-%f{lat}-%f{lng}"
                MarkerProp.MarkerLatitude lat
                MarkerProp.MarkerLongitude lng
            ] [
                img [
                    Key $"existing-img-%f{lat}-%f{lng}"
                    Src "/images/r-black.png"
                    Width 24
                    Height 24
                ]
                strong [ Key $"existing-name-%f{lat}-%f{lng}" ] [ str name ]
            ]
        )

    let refreshLocation (ev : Browser.Types.Event) =
        ev.preventDefault ()

        Browser.Navigator.navigator.geolocation
        |> Option.iter (fun geolocation ->
            geolocation.getCurrentPosition (
                fun position ->
                    JS.console.log ("Got position", position)
                    setUserLatitude position.coords.latitude
                    setUserLongitude position.coords.longitude
                , (fun error -> JS.console.error error)
                , (!!{| enableHighAccuracy = true |})
            )
        )

    fragment [] [
        reactMapGL [
            ReactMapGLProp.MapboxAccessToken mapboxApiAccessToken
            ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState)
            ReactMapGLProp.OnClick onMapClick
            Style {| height = "30vh" ; width = "100%" |}
            ReactMapGLProp.Latitude viewport.latitude
            ReactMapGLProp.Longitude viewport.longitude
            ReactMapGLProp.Zoom viewport.zoom
            ReactMapGLProp.MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"
        ] [
            yield! existingRonnies
            marker [
                Key "ronny"
                MarkerProp.MarkerLatitude ronnyLatitude
                MarkerProp.MarkerLongitude ronnyLongitude
                MarkerProp.OffsetTop 0
                MarkerProp.OffsetLeft 0
            ] [
                img [ Key "marker-ronny-image" ; Src "/images/ronny.png" ; Width 24 ; Height 24 ]
            ]
            marker [
                Key "user"
                MarkerProp.MarkerLatitude userLatitude
                MarkerProp.MarkerLongitude userLongitude
                MarkerProp.OffsetTop 0
                MarkerProp.OffsetLeft 0
            ] [
                icon [
                    Key "user-icon"
                    IconProp.Icon "clarity:user-line"
                    IconProp.Height 24
                    IconProp.Width 24
                ]
            ]
        ]
        button [ ClassName "ghost" ; OnClick refreshLocation ] [ str "Refresh locatie" ]
    ]
