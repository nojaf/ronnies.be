module AddLocation

open System
open Fable.Core
open Fable.Core.JsInterop
open Elmish
open Feliz.UseElmish
open React
open type React.DSL.DOMProps
open ReactRouterDom
open Firebase
open type Firebase.Hooks.Exports
open ComponentsDSL

type Storage = Storage.Exports
type Firestore = FireStore.Exports

open Iconify
open UseFilePicker

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
    Promise.create (fun resolve _reject ->
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
    |> Promise.eitherEnd addLocation (fun _err -> addLocation Array.empty)

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

[<ExportDefault>]
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

    let onLocationChanges (userLocation, ronnyLocation) =
        JS.console.log (userLocation)
        JS.console.log (ronnyLocation)
        dispatch (UpdateLocation ronnyLocation)

        let isTooFar = distanceBetweenTwoPoints userLocation ronnyLocation > 0.25

        dispatch (UpdateLocationError isTooFar)

    let inputError error =
        error |> Option.map (fun error -> p [] [ str error ]) |> ofOption

    let errorClass error =
        if Option.isSome error then "error" else null

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
                    icon [
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
        h1 [ Key "add-location-title" ] [ str "E nieuwen toevoegen" ]
        match model.CurrentState with
        | State.Loading -> loader [ Key "loader" ]
        | State.Submit -> fragment [ Key "submit" ] [ loader [] ; p [ ClassName "center" ] [ str "Ant opsloan..." ] ]
        | State.UnAuthorized ->
            span [ Key "unauthorized" ] [
                str "Sorry matje, je bent geen patron of niet "
                link [ ReactRouterProp.To "/login" ] [ str "ingelogd" ]
            ]
        | State.SubmitFailed ->
            div [ Key "submit-failed" ; Id "submit-failed" ] [
                h2 [] [ str "Butter, zwoar mislukt zeg 😸!" ]
                code [] [ pre [] [ str model.SubmitError ] ]
                p [] [ str "Je kan later nog eens proberen, maar rekent er niet op 😅" ]
            ]
        | State.Enter ->
            form [
                Key "form-enter"
                OnSubmit (fun ev ->
                    ev.preventDefault ()
                    dispatch Submit
                )
            ] [
                div [ ClassName (errorClass model.Errors.Name) ; Key "name-container" ] [
                    label [ Key "label-name" ] [ str "Naam*" ]
                    input [
                        Key "input-name"
                        Name "name"
                        DefaultValue model.Name
                        OnChange (updateOnChange UpdateName)
                        AutoComplete "off"
                    ]
                    inputError model.Errors.Name
                ]
                div [ ClassName (errorClass model.Errors.Price) ; Key "price-container" ] [
                    label [ Key "label-price" ] [ str "Prijs*" ]
                    div [ Key "price-input-container" ; ClassName "price" ] [
                        input [
                            Key "input-price"
                            Name "price"
                            Type "number"
                            Step "0.01"
                            DefaultValue model.Price
                            OnChange (updateOnChange UpdatePrice)
                        ]
                        select [ OnChange (updateOnChange UpdateCurrency) ] (List.map mapToCurrencyItem currencies)
                    ]
                    inputError model.Errors.Price
                ]
                div [ ClassName (errorClass model.Errors.Location) ; Key "location-container" ] [
                    label [ Key "label-location" ] [ str "Locatie*" ]
                    div [ Key "location-picker-container" ; Id "locationPickerContainer" ] [
                        locationPicker [
                            LocationPickerProp.OnChange onLocationChanges
                            LocationPickerProp.ExistingLocations (
                                model.ExistingLocations
                                |> Array.map (fun exisingLocation ->
                                    exisingLocation.name, (exisingLocation.latitude, exisingLocation.longitude)
                                )
                            )
                        ]
                    ]
                    inputError model.Errors.Location
                ]
                div [ Key "container-is-draft" ] [
                    label [] [ str "Ist van vat?" ]
                    toggle [
                        ToggleProp.TrueLabel "Joat"
                        ToggleProp.FalseLabel "Nint"
                        ToggleProp.OnChange (UpdateIsDraft >> dispatch)
                        ToggleProp.Value model.IsDraft
                        ToggleProp.Disabled false
                    ]
                ]
                div [ Key "container-copatrons" ] [
                    label [ Key "label-copatrons" ] [ str "Co-patrons?" ]
                    p [] [ str "Zin der nog matjes aanwezig?" ]
                    div [ Id "others" ] coPatronsOptions
                    ul [ Id "selectedOthers" ] coPatronsSelected
                ]
                div [ Key "container-take-picture" ; Id "take-picture" ] [
                    label [ Key "take-photo" ] [ str "Foto" ]
                    p [ Key "take-photo-info" ] [ str "Niet verplicht, kan wel leutig zijn." ]
                    for photo in model.Photos do
                        img [ Key $"photo-%s{photo}" ; Src photo ; Width "100%" ]
                    button [ Key "take-photo-btn" ; OnClick onTakePicture ] [
                        span [] [ str "Voeg foto toe" ]
                        icon [ IconProp.Icon "ph:camera-duotone" ; IconProp.Height 24 ; IconProp.Width 24 ]
                    ]
                ]
                div [ Key "container-remarks" ] [
                    label [] [ str "Opmerking" ]
                    textarea [ DefaultValue model.Remark ; OnChange (updateOnChange UpdateRemark) ; Rows 2 ] []
                ]
                div [ Key "container-save" ; ClassName "align-right" ] [
                    input [ Type "submit" ; Class "primary" ; Value "Save!" ]
                ]
                if Browser.Dom.window.location.host.StartsWith "localhost" then
                    pre [ Key "debug" ] [ str (JS.JSON.stringify (model, space = 4)) ]
            ]
    ]
