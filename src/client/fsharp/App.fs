module Ronnies.Client.App

open System
open Ronnies.Domain

let myLocation = Location.Parse 4.7 80.9

printfn "%A" myLocation

let myCurrency = ThreeLetterString.Parse "USD"

printfn "%A" myCurrency

let addLocation =
    LocationAdded.Parse
        "257ECC46-DC1F-4EA0-B62D-EF0ADAF1D02D"
        "Cafe X"
        4.7
        80.6
        "3.0"
        "EUR"
        false
        None
        (DateTimeOffset.Now)
        "nojaf"

printfn "%A" addLocation
