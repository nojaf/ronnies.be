module Overview

open System
open Fable.Core.JsInterop
open Feliz
open React
open React.Props
open Firebase
open ReactRouterDom

let formatDate (d : DateTime) : string =
    emitJsExpr
        d
        "`${$0.getDate().toString().padStart(2, '0')}/${($0.getMonth() + 1).toString().padStart(2, '0')}/${$0.getFullYear()}`;"

type SortOrder =
    | ByName = 0
    | ByPrice = 1
    | ByDate = 2

[<ReactComponent>]
let OverviewPage () =
    let querySnapshot, _, _ = Hooks.Exports.useQuery allRonniesQuery
    let sortOrder, setSortOrder = React.useState<SortOrder> (SortOrder.ByPrice)

    let overviewTable =
        querySnapshot
        |> Option.map (fun querySnapshot ->
            let rows =
                querySnapshot.docs
                |> Array.map (fun snapshot -> snapshot.id, snapshot.data ())
                |> fun locations ->
                    match sortOrder with
                    | SortOrder.ByPrice -> Array.sortBy (fun (_, location : RonnyLocation) -> location.price) locations
                    | SortOrder.ByDate ->
                        Array.sortBy (fun (_, location : RonnyLocation) -> location.date.toDate ()) locations
                    | _ -> Array.sortBy (fun (_, location : RonnyLocation) -> location.name) locations
                |> Array.map (fun (id, location) ->
                    let priceText =
                        if location.currency = "EUR" then $"€{location.price}"
                        elif location.currency = "USD" then $"${location.price}"
                        else $"{location.price} {location.currency}"

                    tr [ Key id ] [
                        td [] [ Link [ To $"/detail/{id}" ] [ str location.name ] ]
                        td [] [ str $"%s{priceText}" ]
                        td [] [ str (formatDate (location.date.toDate ())) ]
                    ]
                )

            table [] [
                thead [] [
                    tr [] [
                        th [ OnClick (fun _ -> setSortOrder SortOrder.ByName) ] [ str "Naam" ]
                        th [ OnClick (fun _ -> setSortOrder SortOrder.ByPrice) ] [ str "Prijs" ]
                        th [ OnClick (fun _ -> setSortOrder SortOrder.ByDate) ] [ str "Datum toegevoegd" ]
                    ]
                ]
                tbody [] [ ofArray rows ]
            ]
        )

    main [ Id "overview" ] [ h1 [] [ str "Overzicht" ] ; ofOption overviewTable ]
