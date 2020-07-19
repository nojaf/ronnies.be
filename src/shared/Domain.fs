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
    | InvalidStringLength of expected: int * actual: int
    | NegativeNumber
    | InvalidGuidString

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

type NonEmptyString =
    | NonEmptyString of string

    static member Read (NonEmptyString v) = v

    static member Parse (v: string) =
        if String.IsNullOrWhiteSpace(v) then
            Failure [ EmptyString ]
        else
            Success(NonEmptyString v)

    static member Serialize: Encoder<NonEmptyString> =
        fun (NonEmptyString v) -> Encode.string v

    static member Deserialize: Decoder<NonEmptyString> =
        Decode.string
        |> Decode.andThen (fun v ->
            match NonEmptyString.Parse v with
            | Success v -> Decode.succeed v
            | Failure _ -> Decode.fail "Not an non empty string")

type Identifier =
    private
    | Identifier of Guid

    static member Read (Identifier (identifier)) = identifier

    static member Parse (v: string) =
        match Guid.TryParse(v) with
        | true, v -> Identifier v |> Success
        | false, _ -> Failure [ InvalidGuidString ]

type Latitude =
    | Latitude of float

    static member Read (Latitude v) = v

    static member Parse (v: float) =
        if (v >= -90.0) && (v <= 90.0) then
            Latitude v |> Success
        else
            Failure [ InvalidLatitude ]

type Longitude =
    private
    | Longitude of float

    static member Read (Longitude v) = v

    static member Parse (v: float) =
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

    static member Read (Currency (v, t)) = v, t

    static member Parse (value: decimal) (currencyType: string) =
        if value <= 0m then
            Failure [ NegativeNumber ]
        else
            ThreeLetterString.Parse currencyType
            |> ValidationResult.map (fun currencyType -> Currency(value, currencyType))

let private mapValidationError propertyName
                               (v: ValidationResult<'a, ValidationErrorType>)
                               : ValidationResult<'a, ValidationError> =
    match v with
    | Success s -> Success s
    | Failure errors ->
        errors
        |> List.map (fun e -> propertyName, e)
        |> Failure

type LocationAdded =
    { Id: Identifier
      Name: NonEmptyString
      Location: Location
      Price: Currency
      IsDraft: bool
      Remark: string option
      Created: DateTimeOffset
      Creator: NonEmptyString }

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

        createFn
        <!> (Identifier.Parse >> mapValidationError "id") id
        <*> (NonEmptyString.Parse >> mapValidationError "name") name
        <*> (fun lat lng ->
                Location.Parse lat lng
                |> mapValidationError "location")
                lat
                lng
        <*> (fun p c ->
                Currency.Parse p c
                |> mapValidationError "currency")
                price
                currency
        <*> (ValidationResult.lift
             >> mapValidationError "draft") isDraft
        <*> (ValidationResult.lift
             >> mapValidationError "remark") remark
        <*> (ValidationResult.lift
             >> mapValidationError "created") created
        <*> (NonEmptyString.Parse
             >> mapValidationError "creator") creator

type LocationAddedNotification = unit

// type Event = LocationAdded of LocationAdded
//    | NameUpdated of id: Identifier * name: string
//    | PriceUpdated of id: Identifier * price: Price
//    | LocationUpdated of id: Identifier * location: Location
//    | RemarkUpdated of id: Identifier * description: string
//    | IsDraftUpdated of id: Identifier * isDraft: bool
//    | NewLeaderInHighScores of user: EventSource * score: int
//    | LocationAddedNotificationSent of LocationAddedNotification
//    | LocationMarkedAsDuplicate of id: Identifier

//let newId (): Identifier = System.Guid.NewGuid()

//[<RequireQualifiedAccess>]
//module Projections =
//    let getTotal events =
//        events
//        |> List.filter (function
//            | LocationAdded _ -> true
//            | _ -> false)
//        |> List.length
//
//    let getRonniesWithLocation events =
//        events
//        |> List.choose (function
//            | LocationAdded ({ Location = location; Name = name }) -> Some(name, location)
//            | _ -> None)
//
//    let getRonnies projection events =
//        events
//        |> List.choose (function
//            | LocationAdded la -> Some(projection la)
//            | _ -> None)
