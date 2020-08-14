module Ronnies.Domain

open System
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type ValidationErrorType =
    | InvalidLatitude
    | InvalidLongitude
    | EmptyString
    | InvalidStringLength of expected : int * actual : int
    | InvalidNumber
    | NegativeNumber
    | InvalidGuidString
    | EmptyGuid

type ValidationError = string * ValidationErrorType

let private curry f a b = f (a, b)

type ValidationResult<'a, 'b> =
    | Success of 'a
    | Failure of 'b list

module ValidationResult =
    let map f xResult =
        match xResult with
        | Success x -> Success(f x)
        | Failure errs -> Failure errs


    let lift x = Success x

    let apply fResult xResult =
        match fResult, xResult with
        | Success f, Success x -> Success(f x)
        | Failure errs, Success _ -> Failure errs
        | Success _, Failure errs -> Failure errs
        | Failure errs1, Failure errs2 -> Failure(List.concat [ errs1; errs2 ])

    let bind f xResult =
        match xResult with
        | Success x -> f x
        | Failure errs -> Failure errs

let private (<!>) = ValidationResult.map
let private (<*>) = ValidationResult.apply

module Decode =
    let failWithDomainErrors<'t, 'e> (errors : 'e list) : string -> JsonValue -> Result<'t, DecoderError> =
        sprintf "Invalid value according to the domain: %A" errors
        |> Decode.fail

type NonEmptyString =
    | NonEmptyString of string

    static member Read (NonEmptyString v) = v

    static member Parse (v : string) =
        if String.IsNullOrWhiteSpace(v) then
            Failure [ EmptyString ]
        else
            Success(NonEmptyString v)

type Identifier =
    private
    | Identifier of Guid

    static member Create () = Identifier(Guid.NewGuid())

    static member Read (Identifier (identifier)) = identifier

    static member Parse (v : string) =
        match Guid.TryParse(v) with
        | true, v -> Identifier v |> Success
        | false, _ -> Failure [ InvalidGuidString ]

    static member Parse (v : Guid) = Identifier(v)

type Latitude =
    | Latitude of float

    static member Read (Latitude v) = v

    static member Parse (v : float) =
        if (v >= -90.0) && (v <= 90.0) then
            Latitude v |> Success
        else
            Failure [ InvalidLatitude ]

type Longitude =
    private
    | Longitude of float

    static member Read (Longitude v) = v

    static member Parse (v : float) =
        if (v >= -180.0) && (v <= 180.) then
            Longitude v |> Success
        else
            Failure [ InvalidLongitude ]

type Location =
    private
    | Location of Latitude * Longitude

    static member Read (Location (Latitude (lat), Longitude (lng))) = lat, lng

    static member Parse lat lng =
        curry Location
        <!> Latitude.Parse lat
        <*> Longitude.Parse lng

type ThreeLetterString =
    private
    | ThreeLetterString of string

    static member Read (ThreeLetterString (s)) = s

    static member Parse s =
        if String.IsNullOrWhiteSpace s then
            Failure [ EmptyString ]
        elif s.Length <> 3 then
            Failure [ InvalidStringLength(3, s.Length) ]
        else
            Success(ThreeLetterString(s))

type Currency =
    private
    | Currency of decimal * ThreeLetterString

    static member Read (Currency (v, ThreeLetterString (t))) = v, t

    static member Parse (value : string) (currencyType : string) =
        match Decimal.TryParse(value.Replace(",", ".")) with
        | false, _ -> Failure [ InvalidNumber ]
        | true, value ->
            if value <= 0m then
                Failure [ NegativeNumber ]
            else
                ThreeLetterString.Parse currencyType
                |> ValidationResult.map (fun currencyType -> Currency(value, currencyType))

let private mapValidationError
    propertyName
    (v : ValidationResult<'a, ValidationErrorType>)
    : ValidationResult<'a, ValidationError>
    =
    match v with
    | Success s -> Success s
    | Failure errors ->
        errors
        |> List.map (fun e -> propertyName, e)
        |> Failure

type AddLocation =
    { Id : Identifier
      Name : NonEmptyString
      Location : Location
      Price : Currency
      IsDraft : bool
      Remark : string option
      Created : DateTimeOffset
      Creator : NonEmptyString }

    static member Parse id name lat lng price currency isDraft remark created creator =
        let createFn =
            fun id name location price isDraft remark created creator ->
                { Id = id
                  Name = name
                  Location = location
                  Price = price
                  IsDraft = isDraft
                  Remark = remark
                  Created = created
                  Creator = creator }

        let validateGuid =
            function
            | Identifier (emptyGuid) when (emptyGuid = Guid.Empty) -> Failure [ ValidationErrorType.EmptyGuid ]
            | id -> Success id

        createFn
        <!> (validateGuid >> mapValidationError "id") id
        <*> (NonEmptyString.Parse >> mapValidationError "name") name
        <*> (fun lat lng ->
                Location.Parse lat lng
                |> mapValidationError "location")
                lat
                lng
        <*> (fun p c -> Currency.Parse p c |> mapValidationError "price") price currency
        <*> (ValidationResult.lift
             >> mapValidationError "draft") isDraft
        <*> (ValidationResult.lift
             >> mapValidationError "remark") remark
        <*> (ValidationResult.lift
             >> mapValidationError "created") created
        <*> (NonEmptyString.Parse
             >> mapValidationError "creator") creator

    static member Encoder : Encoder<AddLocation> =
        fun addLocation ->
            let (Identifier (id)) = addLocation.Id
            let (Location (Latitude (lat), Longitude (lng))) = addLocation.Location
            let (Currency (price, ThreeLetterString (currency))) = addLocation.Price
            let (NonEmptyString (creator)) = addLocation.Creator

            Encode.object [ "id", Encode.guid id
                            "name", Encode.string (NonEmptyString.Read addLocation.Name)
                            "location", Encode.tuple2 Encode.float Encode.float (lat, lng)
                            "price", Encode.tuple2 Encode.decimal Encode.string (price, currency)
                            "isDraft", Encode.bool addLocation.IsDraft
                            "remark", Encode.option Encode.string addLocation.Remark
                            "created", Encode.datetimeOffset addLocation.Created
                            "creator", Encode.string creator ]

    static member Decoder : Decoder<AddLocation> =
        Decode.object (fun get ->
            let lat, lng =
                get.Required.Field "location" (Decode.tuple2 Decode.float Decode.float)

            let price, currency =
                get.Required.Field "price" (Decode.tuple2 Decode.string Decode.string)

            (get.Required.Field "id" Decode.guid
             |> Identifier.Parse),
            get.Required.Field "name" Decode.string,
            lat,
            lng,
            price,
            currency,
            (get.Required.Field "isDraft" Decode.bool),
            (get.Optional.Field "remark" (Decode.string)),
            (get.Required.Field "created" Decode.datetimeOffset),
            (get.Required.Field "creator" Decode.string))
        |> Decode.andThen (fun (id, name, lat, lng, price, currency, isDraft, remark, created, creator) ->
            let result =
                AddLocation.Parse id name lat lng price currency isDraft remark created creator

            match result with
            | Success addLocation -> Decode.succeed addLocation
            | Failure errors -> Decode.failWithDomainErrors errors)

type LocationAddedNotification = unit

type Event =
    | LocationAdded of AddLocation
    | LocationCancelled of Identifier
    | LocationNoLongerSellsRonnies of Identifier

    static member Encoder : Encoder<Event> =
        fun event ->
            match event with
            | LocationAdded addLocation ->
                Encode.array [| Encode.string "locationAdded"
                                AddLocation.Encoder addLocation |]
            | LocationCancelled id ->
                Encode.array [| Encode.string "locationCancelled"
                                (Identifier.Read >> Encode.guid) id |]
            | LocationNoLongerSellsRonnies id ->
                Encode.array [| Encode.string "locationNoLongerSellsRonnies"
                                (Identifier.Read >> Encode.guid) id |]

    static member Decoder : Decoder<Event> =
        Decode.index 0 Decode.string
        |> Decode.andThen (fun caseName ->
            match caseName with
            | "locationAdded" ->
                Decode.index 1 AddLocation.Decoder
                |> Decode.map LocationAdded
            | "locationCancelled" ->
                Decode.index 1 Decode.guid
                |> Decode.map (fun id -> Identifier.Parse id |> LocationCancelled)
            | "locationNoLongerSellsRonnies" ->
               Decode.index 1 Decode.guid
                |> Decode.map (fun id -> Identifier.Parse id |> LocationNoLongerSellsRonnies)
            | _ ->
                sprintf "`%s` is not a valid case for Event" caseName
                |> Decode.fail)
