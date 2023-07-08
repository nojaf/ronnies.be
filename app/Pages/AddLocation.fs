module AddLocation

open System
open Fable.Core.JsInterop
open Fable.Import
open Feliz
open Feliz.UseElmish
open Elmish
open React
open React.Props
open ReactRouterDom
open Firebase
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open ReactMapGL
open Iconify
open ReactWebcam
open Components

type LatLng = float * float

[<ReactComponent>]
let LocationPicker
    (props :
        {|
            OnChange : LatLng -> LatLng -> unit
            ExistingLocations : (string * LatLng) list
        |})
    =
    let geolocation = useGeolocation ()
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
        |> List.map (fun (name, (lat, lng)) ->
            Marker [ MarkerLatitude lat ; MarkerLongitude lng ; MarkerKey name ] [
                img [ Src "/images/r-black.png" ]
                strong [] [ str name ]
            ]
        )

    ReactMapGL [
        ReactMapGLProp.OnMove (fun ev -> setViewport ev.viewState) :> IProp
        ReactMapGLProp.OnClick onMapClick
        Style [ CSSProp.Height "30vh" ; CSSProp.Width "100%" ]
        ReactMapGLProp.Latitude viewport.latitude
        ReactMapGLProp.Longitude viewport.longitude
        ReactMapGLProp.Zoom viewport.zoom
        ReactMapGLProp.MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"
    ] [
        ofList existingRonnies
        Marker [
            MarkerKey "ronny"
            MarkerLatitude ronnyLatitude
            MarkerLongitude ronnyLongitude
            OffsetTop 0
            OffsetLeft 0
        ] [ img [ Src "/images/ronny.png" ; HTMLAttr.Width 24 ; HTMLAttr.Height 24 ] ]
        Marker [
            MarkerKey "user"
            MarkerLatitude userLatitude
            MarkerLongitude userLongitude
            OffsetTop 0
            OffsetLeft 0
        ] [
            Icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
        ]
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
        HidePhoto : bool
        Photo : string
        CurrentState : State
    }

    member this.HasPhoto = String.IsNullOrWhiteSpace this.Photo |> not

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
    | UpdatePhoto of string
    | HidePhoto

let init _ : Model * Cmd<Msg> =
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
        HidePhoto = false
        Photo = ""
        // Loading the logged in user and the list of users
        CurrentState = State.Loading
    },
    Cmd.none

let isValidNumber (v : string) : bool = emitJsExpr v "!isNaN($0)"

let update msg model =
    match msg with
    | UpdateUserId uid -> { model with UserId = uid }, Cmd.OfPromise.perform API.getUsers uid Msg.UsersLoaded
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

        let cmd =
            if errors.HasErrors then
                Cmd.none
            else
                Browser.Dom.window.alert "valid vent"
                Cmd.none

        { model with Errors = errors }, cmd
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
    | UpdatePhoto base64String -> { model with Photo = base64String }, Cmd.none
    | HidePhoto -> { model with HidePhoto = true }, Cmd.none

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

let auth : Auth.Auth = import "auth" "../../firebase.config.js"

[<ReactComponent>]
let AddLocationPage () =
    let navigate = useNavigate ()
    let user, isUserLoading, _ = useAuthState auth
    let tokenResult, isTokenResultLoading, _ = useAuthIdTokenResult auth
    let model, dispatch = React.useElmish (init, update, Array.empty)

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

    let webcamRef = React.useRef<WebcamRef> null

    let capture =
        React.useCallback (
            fun () ->
                printfn "callback"

                if not (isNullOrUndefined webcamRef.current) then
                    let screenShot = webcamRef.current.getScreenshot ()
                    Fable.Core.JS.console.log screenShot
                    dispatch (Msg.UpdatePhoto screenShot)
            , [| !!webcamRef ; dispatch |]
        )

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

        if model.HasPhoto then
            dispatch (Msg.UpdatePhoto "")
        else
            capture ()

    main [ Id "add-location" ] [
        h1 [] [ str "E nieuwen toevoegen" ]
        match model.CurrentState with
        | State.Loading -> Loader ()
        | State.Submit -> str "Ant opsloan..."
        | State.UnAuthorized ->
            span [] [
                str "Sorry matje, je bent geen patron of niet "
                Link [ To "/login" ] [ str "ingelogd" ]
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
                    input [ DefaultValue model.Name ; updateOnChange UpdateName ]
                    inputError model.Errors.Name
                ]
                div [ yield! errorClass model.Errors.Price ] [
                    label [] [ str "Prijs*" ]
                    div [ ClassName "price" ] [
                        input [
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
                                ExistingLocations = []
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
                if not model.HidePhoto then
                    div [ Id "take-picture" ] [
                        label [] [ str "Foto" ]
                        p [] [ str "Niet verplicht, kan wel leutig zijn." ]
                        if model.HasPhoto then
                            img [ Src model.Photo ; HTMLAttr.Width "100%" ]
                        else
                            Webcam [
                                WebcamProp.Audio false :> IProp
                                HTMLAttr.Width "100%"
                                Ref webcamRef
                                WebcamProp.ScreenshotFormat "image/png"
                                WebcamProp.VideoConstraints {| facingMode = {| ideal = "user" |} |}
                                WebcamProp.OnUserMediaError (fun () -> dispatch Msg.HidePhoto)
                            ]
                        button [ OnClick onTakePicture ] [
                            span [] [
                                str (
                                    if String.IsNullOrWhiteSpace model.Photo then
                                        "Neem foto"
                                    else
                                        "Herneem foto"
                                )
                            ]
                            Icon [ IconProp.Icon "ph:camera-duotone" ; IconProp.Height 24 ; IconProp.Width 24 ]
                        ]
                    ]
                div [] [
                    label [] [ str "Opmerking" ]
                    textarea [ DefaultValue model.Remark ; updateOnChange UpdateRemark ; Rows 2 ] []
                ]
                input [ Type "submit" ; Class "primary" ; Value "Save!" ]
                pre [] [ str (Fable.Core.JS.JSON.stringify (model, space = 4)) ]
            ]
    ]

(*
    TODO:
        - [ ] Take a picture?
        - [ ] Submit to firebase
        - [ ] Load other locations
*)
