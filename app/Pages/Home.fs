module Home

open System
open Feliz
open React
open React.Props
open Firebase
open ReactMapGL
open Iconify
open ReactRouterDom

[<ReactComponent>]
let HomePage () =
    let querySnapshot, queryLoading, _ = Hooks.Exports.useQuery allRonniesQuery
    let geolocation = useGeolocation {| enableHighAccuracy = true |}

    let viewport, setViewport =
        React.useState<Viewport>
            {|
                latitude = 50.946143
                longitude = 3.138635
                zoom = 16
            |}

    let (routeParams : {| id : string option |}) = useParams ()
    let isModalOpen, setIsModalOpen = React.useState<bool> (routeParams.id.IsSome)
    let detailImageUrl, setDetailImageUrl = React.useState<string option> (None)

    React.useEffect (
        (fun () ->
            if
                not geolocation.loading
                && Option.isNone geolocation.error
                && Option.isNone routeParams.id
            then
                setViewport (
                    {| viewport with
                        latitude = geolocation.latitude
                        longitude = geolocation.longitude
                    |}
                )
        ),
        [| box geolocation.loading |]
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

                    setViewport (
                        {| viewport with
                            latitude = location.latitude
                            longitude = location.longitude
                        |}
                    )

                    match location.photoName with
                    | None -> setDetailImageUrl None
                    | Some photoName ->

                    let storageRef = Storage.Exports.ref (storage, $"locations/{photoName}")

                    Storage.Exports.getDownloadURL (storageRef)
                    |> Promise.iter (Some >> setDetailImageUrl)
                )
        , [| box queryLoading ; box routeParams.id |]
    )

    let icons =
        querySnapshot
        |> Option.map (fun snapshot -> snapshot.docs)
        |> Option.defaultValue Array.empty
        |> Array.map (fun snapshot ->
            let location = snapshot.data ()

            Marker [
                Key snapshot.id
                MarkerLatitude location.latitude
                MarkerLongitude location.longitude
                OffsetLeft 0
                OffsetTop 0
            ] [
                Link [ To ($"/detail/%s{snapshot.id}") ; OnClick (fun _ -> setIsModalOpen true) ] [
                    img [ Src "/images/ronny.png" ; HTMLAttr.Height "20" ; HTMLAttr.Width "20" ]
                ]
            ]
        )

    let userIcon =
        if geolocation.loading || Option.isSome geolocation.error then
            None
        else

        Marker [
            Key "user"
            MarkerLatitude geolocation.latitude
            MarkerLongitude geolocation.longitude
            OffsetTop 0
            OffsetLeft 0
        ] [
            Icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
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

            let image =
                detailImageUrl
                |> Option.map (fun imgUrl -> img [ Src imgUrl ; Alt $"Foto van {detailLocation.name}" ])

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
                    span [ ClassName "close" ; OnClick (fun _ -> setIsModalOpen false) ] [
                        Icon [ IconProp.Icon "ic:outline-close" ; IconProp.Height 36 ; IconProp.Width 36 ]
                    ]
                    h1 [] [ str detailLocation.name ]
                    ofOption image
                    p [] [ strong [] [ str priceText ] ]
                    if detailLocation.isDraft then
                        p [] [ str "Vant vat matje! 🥳" ]
                    else
                        p [] [ str "Niet vant vat 😔" ]
                    if not (String.IsNullOrWhiteSpace detailLocation.remark) then
                        blockquote [] [ str detailLocation.remark ]
                ]
            ]
        )

    main [ Id "world-map" ] [
        ofOption detail
        ReactMapGL [
            ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState) :> IProp
            Style [ CSSProp.Height "100vh" ; CSSProp.Width "100vw" ] :> IProp
            ReactMapGLProp.Latitude viewport.latitude
            ReactMapGLProp.Longitude viewport.longitude
            ReactMapGLProp.Zoom viewport.zoom
            ReactMapGLProp.MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"
        ] [ ofOption userIcon ; ofArray icons ]
    ]
