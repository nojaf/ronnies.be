module Overview

open System
open Fable.Core.JsInterop
open Feliz
open React
open React.Props
open Firebase
open ReactRouterDom
open Components

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
    let querySnapshot, isLoading, _ = Hooks.Exports.useQuery allRonniesQuery

    let sortOrder, setSortOrder =
        React.useState<SortOrder * bool> (SortOrder.ByDate, false)

    let overviewTable =
        if isLoading then
            None
        else

        querySnapshot
        |> Option.map (fun querySnapshot ->
            let rows =
                querySnapshot.docs
                |> Array.map (fun snapshot -> snapshot.id, snapshot.data ())
                |> fun locations ->
                    match fst sortOrder with
                    | SortOrder.ByPrice ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.price)
                            locations
                    | SortOrder.ByDate ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.date.toDate ())
                            locations
                    | _ ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.name)
                            locations
                |> Array.map (fun (id, location) ->
                    let priceText =
                        if location.currency = "EUR" then $"€%.2f{location.price}"
                        elif location.currency = "USD" then $"$%.2f{location.price}"
                        elif location.currency = "GBP" then $"£%.2f{location.price}"
                        else $"{location.price} {location.currency}"

                    tr [ Key id ] [
                        td [] [ Link [ To $"/detail/{id}" ] [ str location.name ] ]
                        td [] [ str $"%s{priceText}" ]
                        td [] [ str (formatDate (location.date.toDate ())) ]
                    ]
                )

            let onHeaderClick (nextSortOrder : SortOrder) =
                if nextSortOrder = fst sortOrder then
                    fun _ -> setSortOrder (nextSortOrder, not (snd sortOrder))
                else
                    fun _ -> setSortOrder (nextSortOrder, true)

            table [] [
                thead [] [
                    tr [] [
                        th [ OnClick (onHeaderClick SortOrder.ByName) ] [ str "Naam" ]
                        th [ OnClick (onHeaderClick SortOrder.ByPrice) ] [ str "Prijs" ]
                        th [ OnClick (onHeaderClick SortOrder.ByDate) ] [ str "Datum toegevoegd" ]
                    ]
                ]
                tbody [] [ ofArray rows ]
            ]
        )

    main [ Id "overview" ] [
        h1 [] [ str "Overzicht" ]
        match overviewTable with
        | None -> Loader ()
        | Some overviewTable -> overviewTable
    ]
