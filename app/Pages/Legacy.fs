module Legacy

open System
open React
open type React.DSL.DOMProps
open ReactMapGL

type LegacyLocation =
    {|
        id : string
        name : string
        latitude : float
        longitude : float
        price : float
        currency : string
        isDraft : bool
        remark : string option
        created : DateTime
        creator : string
    |}

let LegacyPage () =
    let viewport, setViewport =
        React.useState<Viewport>
            {|
                latitude = 50.946143
                longitude = 3.138635
                zoom = 6
            |}

    let data, setData = React.useState<LegacyLocation array> Array.empty

    React.useEffect (
        fun () ->
            Fetch.fetch "/legacy.json" []
            |> Promise.bind (fun res -> res.json<LegacyLocation array> ())
            |> Promise.iter setData

        , Array.empty
    )

    let icons =
        data
        |> Array.map (fun location ->
            marker [
                Key location.id
                MarkerProp.MarkerLatitude location.latitude
                MarkerProp.MarkerLongitude location.longitude
                MarkerProp.OffsetLeft 0
                MarkerProp.OffsetTop 0
            ] [
                img [
                    Key $"%s{location.id}-image"
                    Src "/images/ronny.png"
                    Height "20"
                    Width "20"
                ]
            ]
        )

    main [ Id "world-map" ] [
        reactMapGL
            [
                ReactMapGLProp.MapboxAccessToken mapboxApiAccessToken
                ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState)
                Style {| height = "100vh" ; width = "100vw" |}
                ReactMapGLProp.Latitude viewport.latitude
                ReactMapGLProp.Longitude viewport.longitude
                ReactMapGLProp.Zoom viewport.zoom
                ReactMapGLProp.MapStyle "mapbox://styles/mapbox/light-v11"
            ]
            icons
    ]
