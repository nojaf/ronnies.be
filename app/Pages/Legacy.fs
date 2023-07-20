module Legacy

open System
open Feliz
open React
open React.Props
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

[<ReactComponent>]
let LegacyPage () =
    let viewport, setViewport =
        React.useState<Viewport>
            {|
                latitude = 50.946143
                longitude = 3.138635
                zoom = 6
            |}

    let data, setData = React.useState<LegacyLocation array> (Array.empty)

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
            Marker [
                Key location.id
                MarkerLatitude location.latitude
                MarkerLongitude location.longitude
                OffsetLeft 0
                OffsetTop 0
            ] [
                img [ Src "/images/ronny.png" ; HTMLAttr.Height "20" ; HTMLAttr.Width "20" ]

            ]
        )

    main [ Id "world-map" ] [
        ReactMapGL [
            ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState) :> IProp
            Style [ CSSProp.Height "100vh" ; CSSProp.Width "100vw" ] :> IProp
            ReactMapGLProp.Latitude viewport.latitude
            ReactMapGLProp.Longitude viewport.longitude
            ReactMapGLProp.Zoom viewport.zoom
            ReactMapGLProp.MapStyle "mapbox://styles/mapbox/light-v11"
        ] [ ofArray icons ]
    ]
