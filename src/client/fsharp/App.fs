module Ronnies.Client.App

open System
open Ronnies.Domain

let myLocation = Location.Parse 4.7 80.9

printfn "%A" myLocation

let myCurrency = ThreeLetterString.Parse "USD"

printfn "%A" myCurrency

let addLocation =
    AddLocation.Parse (Identifier.Create()) "Cafe X" 4.7 80.6 "3.0" "EUR" false None (DateTimeOffset.Now) "nojaf"

printfn "%A" addLocation
