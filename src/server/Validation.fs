module Ronnies.Server.Validation

open AccidentalFish.FSharp.Validation
open Ronnies.Shared

let private isValidLongitude propertyName (v: float) =
    if v >= -180.0 && v <= 180. then
        Ok
    else
        Errors
            ([ { errorCode = "invalidLongitude"
                 message = "Breedtegraad moet tussen -180 en 180 zijn."
                 property = propertyName } ])

let private isValidLatitude propertyName (v: float) =
    if (v >= -85.0) && (v <= 85.0) then
        Ok
    else
        Errors
            ([ { errorCode = "invalidLongitude"
                 message = "Lengtegraad moet tussen -85 en 85 zijn."
                 property = propertyName } ])

let createPrice (v: string) =
    let vd = v.Replace(",", ".")
    match System.Double.TryParse(vd) with
    | true, v -> Some v
    | _ -> None

let private isNotEmptyGuid propertyName value =
    if value = System.Guid.Empty then
        Errors
            ([ { errorCode = "emptyGuid"
                 message = "Guid cannot be empty"
                 property = propertyName } ])
    else
        Ok

let private isNotMinDate propertyName value =
    if value = System.DateTime.MinValue then
        Errors
            ([ { errorCode = "minDate"
                 message = "DateTime cannot be the minimum date"
                 property = propertyName } ])
    else
        Ok

let private validateLocation =
    createValidatorFor<LocationAdded> () {
        validate (fun l -> l.Id) [ isNotEmptyGuid ]
        validate (fun l -> l.Name)
            [ isNotEmpty
              hasMinLengthOf 3 ]
        validate (fun l -> fst l.Location) [ isValidLongitude ]
        validate (fun l -> snd l.Location) [ isValidLatitude ]
        validate (fun l -> l.Price) [ isGreaterThan 0. ]
        validate (fun l -> l.Date) [ isNotMinDate ]
        validate (fun l -> l.Creator) [ isNotEmpty ]
    }

let private validateEvent event =
    match event with
    | Event.LocationAdded location -> validateLocation location
    | _ -> ValidationState.Errors []

let private collectErrors errors =
    errors |> List.map (fun { message = message; property = property } -> sprintf "%s: %s" property message)

let getValidationErrors event =
    match validateEvent event with
    | ValidationState.Errors errors -> Some(collectErrors errors)
    | _ -> None
