module Home

open System
open Fable.Core
open React
open React.DSL
open React.DSL.Props
open Firebase
open ReactMapGL
open Iconify
open ReactRouterDom

[<ExportDefault>]
let HomePage () =
    let querySnapshot, queryLoading, _ = Hooks.Exports.useQuery allRonniesQuery
    let geolocation = useGeolocation {| enableHighAccuracy = true |}

    let viewport, setViewport =
        React.useStateByFunction<Viewport>
            {|
                latitude =
#if DEBUG
                    50.88848984137849
#else
                    50.946143
#endif
                longitude =
#if DEBUG
                    2.812351584135115
#else
                    3.138635
#endif
                zoom = 16
            |}

    let (routeParams : {| id : string option |}) = useParams ()
    let isModalOpen, setIsModalOpen = React.useState<bool> (routeParams.id.IsSome)
    let detailImageUrls, setDetailImageUrls = React.useState<string array> Array.empty

    React.useEffect (
        (fun () ->
            if
                not geolocation.loading
                && Option.isNone geolocation.error
                && Option.isNone routeParams.id
            then
                setViewport (fun viewport ->
                    {| viewport with
                        latitude = geolocation.latitude
                        longitude = geolocation.longitude
                    |}
                )
        ),
        [| box geolocation.loading ; box geolocation.error ; box routeParams.id |]
    )

    let tryFindDetail detailId =
        querySnapshot
        |> Option.bind (fun querySnapshot ->
            querySnapshot.docs |> Array.tryFind (fun snapshot -> snapshot.id = detailId)
        )

    React.useEffect (
        fun () ->
            if not queryLoading then
                routeParams.id
                |> Option.bind tryFindDetail
                |> Option.iter (fun snapshot ->
                    let location = snapshot.data ()

                    setViewport (fun viewport ->
                        {| viewport with
                            latitude = location.latitude
                            longitude = location.longitude
                        |}
                    )

                    location.photoNames
                    |> Array.map (fun photoName ->
                        let storageRef = Storage.Exports.ref (storage, $"locations/{photoName}")
                        Storage.Exports.getDownloadURL (storageRef)
                    )
                    |> Promise.all
                    |> Promise.iter setDetailImageUrls
                )
        , [| box queryLoading ; box routeParams.id |]
    )

    let icons =
        querySnapshot
        |> Option.map (fun snapshot -> snapshot.docs)
        |> Option.defaultValue Array.empty
        |> Array.map (fun snapshot ->
            let location = snapshot.data ()

            marker [
                Key snapshot.id
                MarkerLatitude location.latitude
                MarkerLongitude location.longitude
                OffsetLeft 0
                OffsetTop 0
            ] [
                link [
                    Key $"link-%s{snapshot.id}"
                    To ($"/detail/%s{snapshot.id}")
                    OnClick (fun _ -> setIsModalOpen true)
                ] [ img [ Src "/images/ronny.png" ; Height "20" ; Width "20" ] ]
            ]
        )

    let userIcon =
        if geolocation.loading || Option.isSome geolocation.error then
            None
        else

        marker [
            Key "user"
            MarkerLatitude geolocation.latitude
            MarkerLongitude geolocation.longitude
            OffsetTop 0
            OffsetLeft 0
        ] [
            icon [
                Key "user-icon"
                IconProp.Icon "clarity:user-line"
                IconProp.Height 24
                IconProp.Width 24
            ]
        ]
        |> Some

    let detail =
        if not isModalOpen then
            None
        else

        routeParams.id
        |> Option.bind tryFindDetail
        |> Option.map (fun snapshot ->
            let detailLocation = snapshot.data ()

            let images =
                detailImageUrls
                |> Array.mapi (fun idx imgUrl ->
                    img [ Key $"img-%i{idx}" ; Src imgUrl ; Alt $"Foto van {detailLocation.name}" ]
                )

            let priceText =
                if detailLocation.currency = "EUR" then
                    $"Prijs €%.2f{detailLocation.price}"
                elif detailLocation.currency = "USD" then
                    $"Prijs $%.2f{detailLocation.price}"
                elif detailLocation.currency = "GBP" then
                    $"Prijs £%.2f{detailLocation.price}"
                else
                    $"Prijs {detailLocation.price} {detailLocation.currency}"

            div [ Id "detail" ] [
                div [] [
                    span [ Key "close" ; ClassName "close" ; OnClick (fun _ -> setIsModalOpen false) ] [
                        icon [
                            Key "close-icon"
                            IconProp.Icon "ic:outline-close"
                            IconProp.Height 36
                            IconProp.Width 36
                        ]
                    ]
                    h1 [ Key "detail-title" ] [ str detailLocation.name ]
                    yield! images
                    p [ Key "detail-price" ] [ strong [] [ str priceText ] ]
                    p [ Key "draft" ] [

                        str (
                            if detailLocation.isDraft then
                                "Vant vat matje! 🥳"
                            else
                                "Niet vant vat 😔"
                        )
                    ]
                    if not (String.IsNullOrWhiteSpace detailLocation.remark) then
                        blockquote [ Key "remark" ] [ str detailLocation.remark ]
                ]
            ]
        )

    main [ Id "world-map" ] [
        ofOption detail
        reactMapGL [
            ReactMapGLProp.MapboxAccessToken mapboxApiAccessToken
            ReactMapGLProp.OnMove (fun ev -> setViewport (fun _ -> ev.viewState))
            Style {| height = "100vh" ; width = "100vw" |}
            ReactMapGLProp.Latitude viewport.latitude
            ReactMapGLProp.Longitude viewport.longitude
            ReactMapGLProp.Zoom viewport.zoom
            ReactMapGLProp.MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"
        ] [ ofOption userIcon ; yield! icons ]
    ]
