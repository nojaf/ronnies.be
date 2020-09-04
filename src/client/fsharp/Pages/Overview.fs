module Ronnies.Client.Pages.Overview

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json
open Fable.React
open Fable.React.Props
open Feliz
open ReactToastify
open Auth0
open Fetch
open Ronnies.Client
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Components.Navigation
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page
open Ronnies.Domain

type private Location =
    { Id : string
      Name : string
      Price : string
      PriceValue : decimal
      Creator : string
      Date : string
      Ticks : int64
      NoLongerSellsRonnies : bool }

let private getLocations events =
    List.fold (fun acc event ->
        match event with
        | LocationAdded la ->
            { Id = (Identifier.Read la.Id).ToString()
              Name = NonEmptyString.Read la.Name
              Price = Common.readCurrency la.Price
              PriceValue = Currency.Read la.Price |> fst
              Creator = NonEmptyString.Read la.Creator
              Date = la.Created.ToString("dd/MM/yy")
              Ticks = la.Created.Ticks
              NoLongerSellsRonnies = false }
            :: acc
        | LocationCancelled id ->
            let id = (Identifier.Read id).ToString()
            List.filter (fun l -> l.Id <> id) acc
        | LocationNoLongerSellsRonnies id ->
            let id = (Identifier.Read id).ToString()

            List.map (fun l ->
                if l.Id = id then
                    { l with NoLongerSellsRonnies = true }
                else
                    l) acc) [] events

let private useLocations () =
    let eventCtx = React.useContext (eventContext)

    let locations =
        React.useMemo ((fun () -> getLocations eventCtx.Events), [| eventCtx.Events |])

    locations

type private SortBy =
    | Name
    | Price
    | Date

let private sortFn sort =
    match sort with
    | SortBy.Name -> List.sortBy (fun l -> l.Name)
    | SortBy.Price -> List.sortBy (fun l -> l.PriceValue, l.Name)
    | SortBy.Date -> List.sortBy (fun l -> l.Ticks, l.Name)

let private nameDecoder : Decoder<string> =
    Decode.object (fun get -> get.Required.Field "name" Decode.string)

let private useGetUsers () =
    let users, setUsers = React.useState (Map.empty)
    let auth0 = useAuth0 ()

    React.useEffect
        ((fun () ->
            if auth0.isAuthenticated then
                auth0.getAccessTokenSilently ()
                |> Promise.bind (fun token ->
                    let url = sprintf "%s/users" Common.backendUrl

                    fetch
                        url
                        [ requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                           Common.subscriptionHeader
                                           Common.authHeader token ] ])
                |> Promise.bind (fun res -> res.text ())
                |> Promise.map (fun json ->
                    let result =
                        Decode.fromString (Decode.keyValuePairs nameDecoder) json

                    match result with
                    | Ok users -> Map.ofList users |> setUsers
                    | Error err -> JS.console.error err)
                |> Promise.catchEnd (fun err ->
                    JS.console.error err
                    errorToast "Kon de patrons niet ophalen")),
         [| box auth0.isAuthenticated |])

    users

let private OverviewPage =
    React.functionComponent
        ("OverviewPage",
         (fun () ->
             let locations = useLocations ()
             let sort, setSort = React.useState (SortBy.Name)
             let roles = useRoles ()
             let users = useGetUsers ()

             let locationRows =
                 locations
                 |> sortFn sort
                 |> List.map (fun loc ->
                     let creator =
                         Map.tryFind loc.Creator users
                         |> Option.map (fun creator ->
                             td [ classNames [ Bootstrap.TextRight
                                               Bootstrap.TextSmLeft ] ] [
                                 str creator
                             ])

                     tr [ Key loc.Id ] [
                         td [] [
                             Link [ To(sprintf "/detail/%s" loc.Id)
                                    classNames [ if loc.NoLongerSellsRonnies then
                                                     yield!
                                                         [ Bootstrap.Strike
                                                           Bootstrap.TextMuted ] ] ] [
                                 str loc.Name
                             ]
                         ]
                         td [] [ str loc.Price ]
                         td [ classNames [ Bootstrap.TextCenter
                                           Bootstrap.TextSmLeft ] ] [
                             str loc.Date
                         ]
                         if roles.IsEditorOrAdmin then
                             ofOption creator
                     ])

             page [] [
                 h1 [] [ str "Overzicht" ]
                 table [ classNames [ Bootstrap.Table
                                      Bootstrap.TableStriped ] ] [
                     thead [] [
                         tr [ classNames [ Bootstrap.TextPrimary
                                           Bootstrap.Pointer ] ] [
                             th [ OnClick(fun _ -> setSort (SortBy.Name)) ] [
                                 str "Naam"
                             ]
                             th [ OnClick(fun _ -> setSort (SortBy.Price)) ] [
                                 str "Prijs"
                             ]
                             th [ OnClick(fun _ -> setSort (SortBy.Date)) ] [
                                 str "Datum toegevoegd"
                             ]
                             if roles.IsEditorOrAdmin then
                                 th [] [ str "Door" ]
                         ]
                     ]
                     tbody [ ClassName Bootstrap.OverviewTbody ] [
                         ofList locationRows
                     ]
                 ]
             ]))

exportDefault OverviewPage
