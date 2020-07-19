module Ronnies.Client.App

open System
open Ronnies.Domain
open Fable.React
open Fable.React.Props
open Feliz
open Zanaptak.TypedCssClasses

type Bootstrap = CssClasses<"../src/style.css", Naming.PascalCase>

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
        3.0m
        "EUR"
        false
        None
        (DateTimeOffset.Now)
        "nojaf"

printfn "%A" addLocation

let classNames names = String.concat " " names |> ClassName

let AddLocationPage =
    React.functionComponent (fun () ->
        div [ classNames [ Bootstrap.Container; Bootstrap.P3 ] ] [
            h1 [] [str "Voeg plekke toe"]
            form [] [

            ]
            strong [] [ str "meh" ]
            br []
            button [ classNames [ Bootstrap.Btn
                                  Bootstrap.BtnPrimary ] ] [
                str "My button XYZ"
            ]
        ])
