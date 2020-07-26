module Ronnies.Client.Views.AddLocationPage.View

open System
open Fable.Core.JsInterop
open Fable.Import
open Fable.React
open Fable.React.Props
open Feliz
open Feliz.UseElmish
open Elmish
open Ronnies.Domain
open Ronnies.Client.Styles
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Components.LocationPicker
open Ronnies.Client.Components.Navigation

let private currencies =
    [ "EUR", "Euro"
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
      "GBP", "Brits pond sterling"
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
      "USD", "Amerikaanse dollar"
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
      "ZMW", "Zambiaanse kwacha" ]

type private Model =
    { Name : string
      Price : string
      Currency : string
      Latitude : float
      Longitude : float
      IsDraft : bool
      Remark : string
      Errors : Map<string, string list> }

let private init _ =
    { Name = ""
      Price = ""
      Currency = "EUR"
      Latitude = 0.
      Longitude = 0.
      IsDraft = false
      Remark = ""
      Errors = Map.empty },
    []

type private Msg =
    | UpdateName of string
    | UpdatePrice of string
    | UpdateCurrency of string
    | UpdateLocation of float * float
    | UpdateIsDraft of bool
    | UpdateRemark of string
    | UpdateLocationError of isError : bool
    | Submit

let private update onSubmit msg model =
    match msg with
    | UpdateName n -> ({ model with Name = n } : Model), Cmd.none
    | UpdatePrice p -> { model with Price = p }, Cmd.none
    | UpdateCurrency c -> { model with Currency = c }, Cmd.none
    | UpdateLocation (lat, lng) ->
        { model with
              Latitude = lat
              Longitude = lng },
        Cmd.none
    | UpdateIsDraft d -> { model with IsDraft = d }, Cmd.none
    | UpdateRemark r -> { model with Remark = r }, Cmd.none
    | UpdateLocationError isError ->
        let errors =
            if isError then
                model.Errors
                |> Map.add "distance" [ "De gekozen locatie is te ver van jouw locatie! Das de bedoeling niet veugel." ]
            else
                Map.remove "distance" model.Errors

        { model with Errors = errors }, Cmd.none
    | Submit when (Map.containsKey "distance" model.Errors) ->
        model, Cmd.none
    | Submit ->
        let id = Identifier.Create()

        let remark =
            if String.IsNullOrWhiteSpace model.Remark then
                None
            else
                Some model.Remark

        let result =
            AddLocation.Parse
                id
                model.Name
                model.Latitude
                model.Longitude
                model.Price
                model.Currency
                model.IsDraft
                remark
                DateTimeOffset.Now
                "nojaf"

        match result with
        | Success locationAdded ->
            printfn "valid location %A" locationAdded
            { model with Errors = Map.empty }, Cmd.ofSub (fun _ -> onSubmit locationAdded)
        | Failure validationErrors ->
            let mapError errorType =
                match errorType with
                | InvalidLatitude
                | InvalidLongitude -> "Toch iets raars met je locatie"
                | EmptyString -> "Veld mag niet leeg zijn"
                | NegativeNumber
                | InvalidNumber -> "Geen geldige prijs"
                | _ -> "Hmm, bugje in de software"

            let errors =
                let errs =
                    // edge cases, distance is not validated in the Domain
                    match Map.tryFind "distance" model.Errors with
                    | Some locationErrors -> [ "distance", locationErrors ] |> Map.ofList
                    | None -> Map.empty

                validationErrors
                |> List.groupBy fst
                |> List.fold (fun acc (key, errors) -> Map.add key (List.map (snd >> mapError) errors) acc) errs

            { model with Errors = errors }, Cmd.none

let private mapToCurrencyItem (currencyCode, description) =
    option [ ClassName Bootstrap.DropdownItem
             Key currencyCode
             Value currencyCode
             Title description ] [
        str currencyCode
    ]

let private deg2rad deg = deg * Math.PI / 180.0
let private rad2deg rad = (rad / Math.PI * 180.0)

let private distanceBetweenTwoPoints (latA, lngA) (latB, lngB) =
    if latA = latB && lngA = lngB then
        0.
    else
        let theta = lngA - lngB

        let dist =
            Math.Sin(deg2rad (latA))
            * Math.Sin(deg2rad (latB))
            + (Math.Cos(deg2rad (latA))
               * Math.Cos(deg2rad (latB))
               * Math.Cos(deg2rad (theta)))
            |> Math.Acos
            |> rad2deg
            |> (*) (60. * 1.1515 * 1.609344)

        dist

let private AddLocationPage =
    React.functionComponent
        ("AddLocationPage",
         (fun () ->
             let eventCtx = React.useContext (eventContext)
             let navigate = useNavigate ()
             let (isSubmitting, setIsSubmitting) = React.useState(false)

             let onSubmit (addEvent : AddLocation) =
                 setIsSubmitting(true)
                 eventCtx.AddEvents [ Event.LocationAdded addEvent ]
                 |> Promise.iter (fun _ -> navigate "/")

             let model, dispatch =
                 React.useElmish (init, update onSubmit, Array.empty)

             let updateOnChange msg =
                 fun (ev : Browser.Types.Event) -> ev.Value |> msg |> dispatch
                 |> OnChange

             let onLocationChanges userLocation ronnyLocation =
                 dispatch (UpdateLocation ronnyLocation)

                 let isTooFar =
                     distanceBetweenTwoPoints userLocation ronnyLocation > 0.25

                 dispatch (UpdateLocationError isTooFar)

             let locationError =
                 Map.tryFind "distance" model.Errors
                 |> Option.map (fun errors ->
                     div [ classNames [ Bootstrap.Alert
                                        Bootstrap.AlertDanger ] ] [
                         str (String.concat "\n" errors)
                     ])

             let hasErrors key = Map.containsKey key model.Errors

             let inputErrors key =
                 Map.tryFind key model.Errors
                 |> Option.map (fun errors ->
                     div [ ClassName Bootstrap.InvalidFeedback ] [
                         String.concat "\n" errors |> str
                     ])
                 |> ofOption

             div [ ClassName "page" ] [
                 div [ classNames [ Bootstrap.Container
                                    Bootstrap.P3 ] ] [
                     h1 [] [ str "E nieuwen toevoegen" ]
                     if isSubmitting then
                        Ronnies.Client.Components.Loading.loading "ant opslaan..."
                     else
                         form [ classNames [ Bootstrap.ColMd6
                                             Bootstrap.P0 ]
                                OnSubmit(fun ev ->
                                    ev.preventDefault ()
                                    dispatch Submit) ] [
                             div [ ClassName Bootstrap.FormGroup ] [
                                 label [] [ str "Naam*" ]
                                 input [ classNames [ Bootstrap.FormControl
                                                      if hasErrors "name" then
                                                          Bootstrap.IsInvalid ]
                                         DefaultValue model.Name
                                         updateOnChange UpdateName ]
                                 inputErrors "name"
                             ]
                             div [ ClassName Bootstrap.FormGroup ] [
                                 label [] [ str "Prijs*" ]
                                 div [ ClassName Bootstrap.InputGroup ] [
                                     input [ classNames [ Bootstrap.FormControl
                                                          if hasErrors "price" then
                                                              Bootstrap.IsInvalid ]
                                             Type "number"
                                             Step "0.01"
                                             DefaultValue model.Price
                                             updateOnChange UpdatePrice ]

                                     div [ ClassName Bootstrap.InputGroupAppend ] [
                                         select [ classNames [ Bootstrap.CustomSelect ]
                                                  updateOnChange UpdateCurrency
                                                  Style [ Background "none"
                                                          BorderTopLeftRadius "0"
                                                          BorderBottomLeftRadius "0" ] ] [
                                             ofList (List.map mapToCurrencyItem currencies)
                                         ]
                                     ]
                                     inputErrors "price"
                                 ]
                             ]
                             div [] [
                                 label [] [ str "Locatie*" ]
                                 br []
                                 div [ Id "locationPickerContainer" ] [
                                     LocationPicker
                                         ({ OnChange = onLocationChanges
                                            ExistingLocations = [] })
                                 ]
                                 ofOption locationError
                             ]
                             div [ ClassName Bootstrap.FormGroup ] [
                                 label [] [ str "Ist van vat?" ]
                                 br []
                                 div [ ClassName Bootstrap.BtnGroup ] [
                                     button [ classNames [ Bootstrap.Btn
                                                           if not model.IsDraft then
                                                               Bootstrap.BtnPrimary
                                                           else
                                                               Bootstrap.BtnOutlinePrimary ]
                                              OnClick(fun ev ->
                                                  ev.preventDefault ()
                                                  dispatch (UpdateIsDraft false)) ] [
                                         str "Nint"
                                     ]
                                     button [ classNames [ Bootstrap.Btn
                                                           if model.IsDraft then
                                                               Bootstrap.BtnPrimary
                                                           else
                                                               Bootstrap.BtnOutlinePrimary ]
                                              OnClick(fun ev ->
                                                  ev.preventDefault ()
                                                  dispatch (UpdateIsDraft true)) ] [
                                         str "Joat"
                                     ]
                                 ]
                             ]
                             div [ ClassName Bootstrap.FormGroup ] [
                                 label [] [ str "Opmerking" ]
                                 textarea [ ClassName Bootstrap.FormControl
                                            DefaultValue model.Remark
                                            updateOnChange UpdateRemark ] []
                             ]
                             div [ classNames [ Bootstrap.TextRight
                                                Bootstrap.Pb2 ] ] [
                                 button [ classNames [ Bootstrap.Btn
                                                       Bootstrap.BtnPrimary ] ] [
                                     str "Save!"
                                 ]
                             ]
                         ]
                     ]
             ]))

exportDefault AddLocationPage
