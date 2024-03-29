﻿module AddLocation

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Feliz
open Feliz.UseElmish
open Elmish
open React
open React.Props
open ReactRouterDom
open Firebase
open type Firebase.Hooks.Exports

type Storage = Firebase.Storage.Exports
type Firestore = Firebase.FireStore.Exports

open ReactMapGL
open Iconify
open UseFilePicker
open Components

type LatLng = float * float

[<ReactComponent>]
let LocationPicker
    (props :
        {|
            OnChange : LatLng -> LatLng -> unit
            ExistingLocations : (string * LatLng) array
        |})
    =
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

                props.OnChange
                    (geolocation.latitude, geolocation.longitude)
                    (geolocation.latitude, geolocation.longitude)
        ),
        [| box geolocation.loading |]
    )

    let onMapClick (ev : MapLayerMouseEvent) =
        let lngLat = ev.lngLat
        setRonnyLatitude lngLat.lat
        setRonnyLongitude lngLat.lng
        props.OnChange (userLatitude, userLongitude) (lngLat.lat, lngLat.lng)

    let existingRonnies =
        props.ExistingLocations
        |> Array.map (fun (name, (lat, lng)) ->
            Marker [ MarkerLatitude lat ; MarkerLongitude lng ; Key name ] [
                img [ Src "/images/r-black.png" ; HTMLAttr.Width 24 ; HTMLAttr.Height 24 ]
                strong [] [ str name ]
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
                , (fun error -> JS.console.error (error))
                , (!!{| enableHighAccuracy = true |})
            )
        )

    fragment [] [
        ReactMapGL [
            ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState) :> IProp
            ReactMapGLProp.OnClick onMapClick
            Style [ CSSProp.Height "30vh" ; CSSProp.Width "100%" ]
            ReactMapGLProp.Latitude viewport.latitude
            ReactMapGLProp.Longitude viewport.longitude
            ReactMapGLProp.Zoom viewport.zoom
            ReactMapGLProp.MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"
        ] [
            ofArray existingRonnies
            Marker [
                Key "ronny"
                MarkerLatitude ronnyLatitude
                MarkerLongitude ronnyLongitude
                OffsetTop 0
                OffsetLeft 0
            ] [ img [ Src "/images/ronny.png" ; HTMLAttr.Width 24 ; HTMLAttr.Height 24 ] ]
            Marker [
                Key "user"
                MarkerLatitude userLatitude
                MarkerLongitude userLongitude
                OffsetTop 0
                OffsetLeft 0
            ] [
                Icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
            ]
        ]
        button [ OnClick refreshLocation ] [ str "Refresh locatie" ]
    ]

let currencies =
    [
        "EUR", "Euro"
        "GBP", "Brits pond sterling"
        "USD", "Amerikaanse dollar"
        "AED", "VAE-dirham"
        "AFN", "Afghani"
        "ALL", "Albanese lek"
        "AMD", "Armeense dram"
        "ANG", "Antilliaanse gulden"
        "AOA", "Angolese kwanza"
        "ARS", "Argentijnse peso"
        "AUD", "Australische dollar"
        "AWG", "Arubaanse florin"
        "AZN", "Azerbeidzjaanse manat"
        "BAM", "Bosnische inwisselbare mark"
        "BBD", "Barbadiaanse dollar"
        "BDT", "Bengalese taka"
        "BGN", "Nieuwe Bulgaarse lev (sinds 1999)"
        "BHD", "Bahreinse dinar"
        "BIF", "Burundese frank"
        "BMD", "Bermudaanse dollar"
        "BND", "Bruneise dollar"
        "BOB", "Boliviaanse boliviano"
        "BOV", "Boliviaanse Mvdol (aandelen)"
        "BRL", "Braziliaanse real"
        "BSD", "Bahamaanse dollar"
        "BTN", "Bhutaanse ngultrum"
        "BWP", "Botswaanse pula"
        "BYN", "Wit-Russische roebel"
        "BZD", "Belizaanse dollar"
        "CAD", "Canadese dollar"
        "CDF", "Congolese frank"
        "CHF", "Zwitserse frank"
        "CLF", "Chileense Unidades de fomento (aandelen)"
        "CLP", "Chileense peso"
        "CNY", "Chinese renminbi"
        "COP", "Colombiaanse peso"
        "COU", "Colombiaanse unidad de valor real"
        "CRC", "Costa Ricaanse colon"
        "CUC", "Convertibele peso (munteenheid voor toeristen in Cuba)"
        "CUP", "Cubaanse peso"
        "CVE", "Kaapverdische escudo"
        "CZK", "Tsjechische kroon"
        "DJF", "Djiboutiaanse frank"
        "DKK", "Deense kroon"
        "DOP", "Dominicaanse peso"
        "DZD", "Algerijnse dinar"
        "EEK", "Estische kroon"
        "EGP", "Egyptisch pond"
        "ERN", "Eritrese nakfa"
        "ETB", "Ethiopische birr"
        "FJD", "Fiji-dollar"
        "FKP", "Falklandeilands pond"
        "GEL", "Georgische lari"
        "GHS", "Ghanese cedi (Geldig per 1 juli 2007, 1 GHS = 10.000 GHC)"
        "GIP", "Gibraltarees pond"
        "GMD", "Gambiaanse dalasi"
        "GNF", "Guineese frank"
        "GTQ", "Guatemalaanse quetzal"
        "GYD", "Guyaanse dollar"
        "HKD", "Hongkongse dollar"
        "HNL", "Hondurese lempira"
        "HRK", "Kroatische kuna"
        "HTG", "Haïtiaanse gourde"
        "HUF", "Hongaarse forint"
        "IDR", "Indonesische roepia"
        "ILS", "Nieuwe Israëlische sjekel"
        "INR", "Indiase roepie"
        "IQD", "Iraakse dinar"
        "IRR", "Iraanse rial"
        "ISK", "IJslandse kroon"
        "JMD", "Jamaicaanse dollar"
        "JOD", "Jordaanse dinar"
        "JPY", "Japanse yen"
        "KES", "Keniaanse shilling"
        "KGS", "Kirgizische som"
        "KHR", "Cambodjaanse riel"
        "KMF", "Comorese frank"
        "KPW", "Noord-Koreaanse won"
        "KRW", "Zuid-Koreaanse won"
        "KWD", "Koeweitse dinar"
        "KYD", "Kaaimaneilandse dollar"
        "KZT", "Kazachse tenge"
        "LAK", "Laotiaanse kip"
        "LBP", "Libanees pond"
        "LKR", "Sri Lankaanse roepie"
        "LRD", "Liberiaanse dollar"
        "LSL", "Lesothaanse loti"
        "LYD", "Libische dinar"
        "MAD", "Marokkaanse dirham"
        "MDL", "Moldavische leu"
        "MGA", "Malagassische ariary"
        "MKD", "Macedonische denar"
        "MMK", "Myanmarese kyat"
        "MNT", "Mongolische tugrik"
        "MOP", "Macause pataca"
        "MRU", "Mauritaanse ouguiya"
        "MUR", "Mauritiaanse roepie"
        "MVR", "Maldivische rufiyaa"
        "MWK", "Malawische kwacha"
        "MXN", "Mexicaanse peso"
        "MXV", "Mexicaanse Unidad de Inversion (UDI) (Funds code)"
        "MYR", "Maleisische ringgit"
        "MZN", "Mozambikaanse metical"
        "NAD", "Namibische dollar"
        "NGN", "Nigeriaanse naira"
        "NIO", "Nicaraguaanse córdoba Oro"
        "NOK", "Noorse kroon"
        "NPR", "Nepalese roepie"
        "NZD", "Nieuw-Zeelandse dollar"
        "OMR", "Omaanse rial"
        "PAB", "Panamese balboa"
        "PEN", "Nieuwe Peruviaanse sol"
        "PGK", "Papoease kina"
        "PHP", "Filipijnse peso"
        "PKR", "Pakistaanse roepie"
        "PLN", "Poolse zloty"
        "PYG", "Paraguayaanse guaraní"
        "QAR", "Qatarese rial"
        "RON", "Roemeense leu"
        "RSD", "Servische dinar"
        "RUB", "Russische roebel"
        "RWF", "Rwandese frank"
        "SAR", "Saoedi-Arabische riyal"
        "SBD", "Salomon-dollar"
        "SCR", "Seychelse roepie"
        "SDG", "Soedanees pond"
        "SEK", "Zweedse kroon"
        "SGD", "Singaporese dollar"
        "SHP", "Sint-Heleens pond"
        "SLL", "Sierra Leoonse leone"
        "SOS", "Somalische shilling"
        "SSP", "Zuid-Soedanees pond"
        "SRD", "Surinaamse dollar"
        "STN", "Santomese dobra"
        "SYP", "Syrisch pond"
        "SZL", "Swazische lilangeni"
        "THB", "Thaise baht"
        "TJS", "Tadzjiekse somoni"
        "TMT", "Turkmeense manat"
        "TND", "Tunesische dinar"
        "TOP", "Tongaanse pa'anga"
        "TRY", "Nieuwe Turkse lira (sinds 1 januari 2005)"
        "TTD", "Trinidad en Tobagodollar"
        "TWD", "Nieuwe Taiwanese dollar"
        "TZS", "Tanzaniaanse shilling"
        "UAH", "Oekraïense grivna"
        "UGX", "Oegandese shilling"
        "USN", "Amerikaanse dollar (Next day) (aandelen)"
        "USS", "Amerikaanse dollar (Same day) (aandelen)"
        "UYU", "Uruguayaanse peso"
        "UZS", "Oezbeekse sum"
        "VEF", "Venezolaanse bolivar"
        "VND", "Vietnamese dong"
        "VUV", "Vanuatuaanse vatu"
        "WST", "Samoaanse tala"
        "XAF", "CFA-frank BEAC"
        "XCD", "Oost-Caribische dollar"
        "XPD", "1 Troy ounce palladium"
        "XPF", "CFP-frank"
        "XPT", "1 Troy ounce platina"
        "YER", "Jemenitische rial"
        "ZAR", "Zuid-Afrikaanse rand"
        "ZMW", "Zambiaanse kwacha"
    ]

type Errors =
    {
        Name : string option
        Price : string option
        Location : string option
    }

    member this.HasErrors = this.Name.IsSome || this.Price.IsSome || this.Location.IsSome

    static member Empty =
        {
            Name = None
            Price = None
            Location = None
        }

[<RequireQualifiedAccess>]
type State =
    | Loading
    | Enter
    | UnAuthorized
    | Submit
    | SubmitFailed

type Model =
    {
        UserId : string
        Name : string
        Price : string
        Currency : string
        Latitude : float
        Longitude : float
        IsDraft : bool
        Remark : string
        Errors : Errors
        Users : API.User array
        OtherUsers : string Set
        Photos : string array
        ExistingLocations : RonnyLocation array
        SubmitError : string
        CurrentState : State
    }

type Msg =
    | UpdateUserId of string
    | IsUnauthorized
    | UpdateName of string
    | UpdatePrice of string
    | UpdateCurrency of string
    | UpdateLocation of float * float
    | UpdateIsDraft of bool
    | UpdateRemark of string
    | UpdateLocationError of isError : bool
    | Submit
    | UsersLoaded of API.User array
    | ToggleUser of uid : string
    | UpdatePhotos of string array
    | ExistingLocationsLoaded of RonnyLocation array
    | SubmitFailed of error : string

let init _ : Model * Cmd<Msg> =
    let cmd =
        Cmd.ofEffect (fun dispatch ->
            Firestore.getDocs<RonnyLocation> allRonniesQuery
            |> Promise.map (fun querySnapshot ->
                querySnapshot.docs
                |> Array.map (fun snapshot -> snapshot.data ())
                |> Msg.ExistingLocationsLoaded
                |> dispatch
            )
            |> Promise.start
        )

    {
        UserId = ""
        Name = ""
        Price = ""
        Currency = "EUR"
        Latitude = 0.
        Longitude = 0.
        IsDraft = false
        Remark = ""
        Errors = Errors.Empty
        Users = Array.empty
        OtherUsers = Set.empty
        Photos = Array.empty
        SubmitError = ""
        // Loading the logged in user and the list of users
        CurrentState = State.Loading
        ExistingLocations = Array.empty
    },
    cmd

let isValidNumber (v : string) : bool = emitJsExpr v "!isNaN($0)"

/// Source: https://gist.github.com/ORESoftware/ba5d03f3e1826dc15d5ad2bcec37f7bf?permalink_comment_id=3518821#gistcomment-3518821
let resizeImage base64String maxWidth maxHeight =
    Promise.create (fun resolve reject ->
        let img = Browser.Dom.Image.Create ()
        img.src <- base64String

        img.addEventListener (
            "load",
            fun _ ->
                let canvas =
                    Browser.Dom.document.createElement "canvas" :?> Browser.Types.HTMLCanvasElement

                let mutable width = img.width
                let mutable height = img.height

                if width > height then
                    if width > maxWidth then
                        height <- height * (float maxWidth / float width)
                        width <- float maxWidth
                else if height > maxHeight then
                    width <- width * (float maxHeight / float height)
                    height <- float maxHeight

                canvas.width <- width
                canvas.height <- height
                let ctx = canvas.getContext "2d"
                ctx?drawImage (img, 0, 0, width, height)
                resolve (canvas.toDataURL ())
        )
    )

let submitLocation (navigate : string -> unit) (model : Model) (dispatch : Msg -> unit) =
    let storagePromise =
        let uploadPhoto base64Photo =
            promise {
                let fileName = emitJsExpr () "`locations/${Date.now()}.png`"
                let! resized = resizeImage base64Photo 1024 640
                let imageRef = Storage.ref (storage, fileName)
                let! res = Fetch.fetch resized []
                let! blob = res.blob ()
                let! _snapShot = Storage.uploadBytes (imageRef, blob)
                return Some imageRef.name
            }

        model.Photos
        |> Array.map uploadPhoto
        |> Promise.all
        |> Promise.map (Array.choose id)

    let addLocation (photoNames : string array) : unit =
        let locations = Firestore.collection (firestore, Constants.Locations)

        let locationData =
            {|
                name = model.Name
                price = JS.parseFloat model.Price
                currency = model.Currency
                latitude = model.Latitude
                longitude = model.Longitude
                isDraft = model.IsDraft
                userId = model.UserId
                otherUserIds = Seq.toArray model.OtherUsers
                photoNames = photoNames
                remark = model.Remark
                date = JS.Constructors.Date.Create () |> FireStore.TimestampStatic.fromDate
            |}

        Firestore.addDoc<RonnyLocation> (locations, locationData)
        |> Promise.eitherEnd
            (fun docRef -> navigate $"/detail/{docRef.id}")
            (fun err ->
                JS.console.error err
                dispatch (Msg.SubmitFailed err.Message)
            )

    storagePromise
    |> Promise.eitherEnd addLocation (fun err -> addLocation Array.empty)

let update (navigate : string -> unit) msg model =
    match msg with
    | UpdateUserId uid -> { model with UserId = uid }, Cmd.OfPromise.perform API.getUsers () Msg.UsersLoaded
    | IsUnauthorized ->
        { model with
            CurrentState = State.UnAuthorized
        },
        Cmd.none
    | UpdateName n -> ({ model with Name = n } : Model), Cmd.none
    | UpdatePrice p -> { model with Price = p }, Cmd.none
    | UpdateCurrency c -> { model with Currency = c }, Cmd.none
    | UpdateLocation (lat, lng) ->
        { model with
            Latitude = lat
            Longitude = lng
        },
        Cmd.none
    | UpdateIsDraft d -> { model with IsDraft = d }, Cmd.none
    | UpdateRemark r -> { model with Remark = r }, Cmd.none
    | UpdateLocationError isError ->
        let errors =
            if isError then
                { model.Errors with
                    Location = Some "De gekozen locatie is te ver van jouw locatie! Das de bedoeling niet veugel."
                }
            else
                { model.Errors with Location = None }

        { model with Errors = errors }, Cmd.none
    | Submit ->
        let errors =
            {
                Name =
                    if String.IsNullOrWhiteSpace model.Name then
                        Some "Naam is verplicht"
                    else
                        None
                Price =
                    if String.IsNullOrWhiteSpace model.Price then
                        Some "Prijs is verplicht"
                    elif not (isValidNumber model.Price) then
                        Some "Prijs is precies geen getal"
                    else
                        None
                Location =
                    if not ((model.Longitude >= -180.0) && (model.Longitude <= 180.)) then
                        Some "De lengtegraad is lik roar"
                    elif not ((model.Latitude >= -90.0) && (model.Latitude <= 90.0)) then
                        Some "De breedtegraad is lik roar"
                    else
                        model.Errors.Location
            }

        let model =
            if errors.HasErrors then
                { model with Errors = errors }
            else
                { model with
                    CurrentState = State.Submit
                }

        let cmd =
            if errors.HasErrors then
                Cmd.none
            else
                Cmd.ofEffect (submitLocation navigate model)

        model, cmd
    | UsersLoaded users ->
        { model with
            Users = users
            CurrentState = State.Enter
        },
        Cmd.none
    | ToggleUser uid ->
        if model.OtherUsers.Contains uid then
            { model with
                OtherUsers = Set.remove uid model.OtherUsers
            },
            Cmd.none
        else
            { model with
                OtherUsers = Set.add uid model.OtherUsers
            },
            Cmd.none
    | UpdatePhotos base64Strings -> { model with Photos = base64Strings }, Cmd.none
    | ExistingLocationsLoaded existingLocations ->
        { model with
            ExistingLocations = existingLocations
        },
        Cmd.none
    | SubmitFailed error ->
        { model with
            CurrentState = State.SubmitFailed
            SubmitError = error
        },
        Cmd.none

let mapToCurrencyItem (currencyCode, description) =
    option [ ClassName "" ; Key currencyCode ; Value currencyCode ; Title description ] [ str currencyCode ]

let deg2rad deg = deg * Math.PI / 180.0
let rad2deg rad = (rad / Math.PI * 180.0)

let distanceBetweenTwoPoints (latA, lngA) (latB, lngB) =
    if latA = latB && lngA = lngB then
        0.
    else
        let theta = lngA - lngB

        let dist =
            Math.Sin (deg2rad latA) * Math.Sin (deg2rad latB)
            + (Math.Cos (deg2rad latA) * Math.Cos (deg2rad latB) * Math.Cos (deg2rad theta))
            |> Math.Acos
            |> rad2deg
            |> (*) (60. * 1.1515 * 1.609344)

        dist

[<ReactComponent>]
let AddLocationPage () =
    let navigate = useNavigate ()
    let user, isUserLoading, _ = useAuthState auth
    let tokenResult, isTokenResultLoading, _ = useAuthIdTokenResult auth
    let model, dispatch = React.useElmish (init, update navigate, Array.empty)

    React.useEffect (
        fun () ->
            match user, tokenResult with
            | Some user, Some tokenResult ->
                if not tokenResult.claims?``member`` then
                    dispatch Msg.IsUnauthorized
                else
                    dispatch (Msg.UpdateUserId user.uid)
            | None, None when (isUserLoading && isTokenResultLoading) -> dispatch Msg.IsUnauthorized
            | _ -> ()

        , [| box tokenResult ; box user |]
    )

    let filePickerResult =
        useFilePicker
            {|
                readAs = "DataURL"
                accept = "image/*"
                multiple = true
                limitFilesConfig = {| max = 3 |}
                maxFileSize = 10
                onFilesSuccessfullySelected =
                    fun files ->
                        files.filesContent
                        |> Array.map (fun f -> f.content)
                        |> Msg.UpdatePhotos
                        |> dispatch
            |}

    let updateOnChange msg =
        fun (ev : Browser.Types.Event) -> ev.Value |> msg |> dispatch
        |> OnChange

    let onLocationChanges userLocation ronnyLocation =
        dispatch (UpdateLocation ronnyLocation)

        let isTooFar = distanceBetweenTwoPoints userLocation ronnyLocation > 0.25

        dispatch (UpdateLocationError isTooFar)

    let inputError error =
        error |> Option.map (fun error -> p [] [ str error ]) |> ofOption

    let errorClass error : IHTMLProp seq =
        match error with
        | None -> []
        | Some _ -> [ ClassName "error" ]

    let coPatronsOptions =
        model.Users
        |> Array.filter (fun u -> not (model.OtherUsers.Contains u.uid))
        |> Array.sortBy (fun u -> u.displayName)
        |> Array.map (fun u ->
            button [
                Key u.uid
                OnClick (fun ev ->
                    ev.preventDefault ()
                    dispatch (Msg.ToggleUser u.uid)
                )
            ] [ str u.displayName ]
        )

    let coPatronsSelected =
        model.OtherUsers
        |> Seq.toArray
        |> Array.map (fun uid ->
            let displayName =
                model.Users
                |> Seq.pick (fun u -> if u.uid = uid then Some u.displayName else None)

            li [ Key uid ] [
                span [] [ str displayName ]
                button [
                    ClassName "danger"
                    OnClick (fun ev ->
                        ev.preventDefault ()
                        dispatch (Msg.ToggleUser uid)
                    )
                ] [
                    Icon [
                        IconProp.Icon "iconamoon:trash-duotone"
                        IconProp.Width 16
                        IconProp.Height 16
                    ]
                ]
            ]
        )

    let onTakePicture (ev : Browser.Types.MouseEvent) =
        ev.preventDefault ()
        filePickerResult.openFilePicker ()

    main [ Id "add-location" ] [
        h1 [] [ str "E nieuwen toevoegen" ]
        match model.CurrentState with
        | State.Loading -> Loader ()
        | State.Submit -> fragment [] [ Loader () ; p [ ClassName "center" ] [ str "Ant opsloan..." ] ]
        | State.UnAuthorized ->
            span [] [
                str "Sorry matje, je bent geen patron of niet "
                Link [ To "/login" ] [ str "ingelogd" ]
            ]
        | State.SubmitFailed ->
            div [ Id "submit-failed" ] [
                h2 [] [ str "Butter, zwoar mislukt zeg 😸!" ]
                code [] [ pre [] [ str model.SubmitError ] ]
                p [] [ str "Je kan later nog eens proberen, maar rekent er niet op 😅" ]
            ]
        | State.Enter ->
            form [
                OnSubmit (fun ev ->
                    ev.preventDefault ()
                    dispatch Submit
                )
            ] [
                div [ yield! errorClass model.Errors.Name ] [
                    label [] [ str "Naam*" ]
                    input [ Name "name" ; DefaultValue model.Name ; updateOnChange UpdateName ]
                    inputError model.Errors.Name
                ]
                div [ yield! errorClass model.Errors.Price ] [
                    label [] [ str "Prijs*" ]
                    div [ ClassName "price" ] [
                        input [
                            Name "price"
                            Type "number"
                            Step "0.01"
                            DefaultValue model.Price
                            updateOnChange UpdatePrice
                        ]
                        select [ updateOnChange UpdateCurrency ] [ ofList (List.map mapToCurrencyItem currencies) ]
                    ]
                    inputError model.Errors.Price
                ]
                div [ yield! errorClass model.Errors.Location ] [
                    label [] [ str "Locatie*" ]
                    div [ Id "locationPickerContainer" ] [
                        LocationPicker
                            {|
                                OnChange = onLocationChanges
                                ExistingLocations =
                                    model.ExistingLocations
                                    |> Array.map (fun exisingLocation ->
                                        exisingLocation.name, (exisingLocation.latitude, exisingLocation.longitude)
                                    )
                            |}
                    ]
                    inputError model.Errors.Location
                ]
                div [] [
                    label [] [ str "Ist van vat?" ]
                    Toggle
                        {|
                            TrueLabel = "Joat"
                            FalseLabel = "Nint"
                            OnChange = (UpdateIsDraft >> dispatch)
                            Value = model.IsDraft
                            Disabled = false
                        |}
                ]
                div [] [
                    label [] [ str "Co-patrons?" ]
                    p [] [ str "Zin der nog matjes aanwezig?" ]
                    div [ Id "others" ] [ ofArray coPatronsOptions ]
                    ul [ Id "selectedOthers" ] [ ofArray coPatronsSelected ]
                ]
                div [ Id "take-picture" ] [
                    label [] [ str "Foto" ]
                    p [] [ str "Niet verplicht, kan wel leutig zijn." ]
                    for photo in model.Photos do
                        img [ Src photo ; HTMLAttr.Width "100%" ]
                    button [ OnClick onTakePicture ] [
                        span [] [ str "Voeg foto toe" ]
                        Icon [ IconProp.Icon "ph:camera-duotone" ; IconProp.Height 24 ; IconProp.Width 24 ]
                    ]
                ]
                div [] [
                    label [] [ str "Opmerking" ]
                    textarea [ DefaultValue model.Remark ; updateOnChange UpdateRemark ; Rows 2 ] []
                ]
                div [ ClassName "align-right" ] [ input [ Type "submit" ; Class "primary" ; Value "Save!" ] ]
            // if Browser.Dom.window.location.host.StartsWith ("localhost") then
            //     pre [] [ str (Fable.Core.JS.JSON.stringify (model, space = 4)) ]
            ]
    ]
