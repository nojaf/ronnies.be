module Ronnies.Client.Pages.Detail

open System
open Fable.Core
open Fable.Core.JsInterop
open Fetch
open Fable.React
open Fable.React.Props
open Feliz
open Thoth.Json
open Auth0
open Ronnies.Client
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page
open Ronnies.Domain

type private LocationDetail =
    { Id : Guid
      Name : string
      Creator : string
      Created : string
      Price : string
      IsDraft : bool
      Remark : string option }

    static member Empty =
        { Id = Guid.Empty
          Name = "???"
          Creator = ""
          Created = ""
          Price = ""
          IsDraft = false
          Remark = None }

let readCurrency price =
    let (value, currency) = Currency.Read price
    match currency with
    | "EUR" -> sprintf "€%.2f" value
    | "USD" -> sprintf "$%.2f" value
    | "GBP" -> sprintf "£%.2f" value
    | _ -> sprintf "%.2f %s" value currency

let private getLocation events id =
    List.fold (fun acc event ->
        match event with
        | LocationAdded loc when (Identifier.Read loc.Id |> (=) id) ->
            { acc with
                  Id = id
                  Name = NonEmptyString.Read loc.Name
                  Creator = NonEmptyString.Read loc.Creator
                  Created = loc.Created.ToString("dd/MM/yy")
                  Price = readCurrency loc.Price
                  IsDraft = loc.IsDraft
                  Remark = loc.Remark } : LocationDetail

        | LocationAdded _ -> acc) LocationDetail.Empty events

let nameDecoder : Decoder<string> =
    Decode.object (fun get -> get.Required.Field "name" Decode.string)

let private useLocationDetail id =
    let id = Guid.Parse(id)
    let eventCtx = React.useContext (eventContext)
    let (creatorName, setCreatorName) = React.useState<string option> (None)

    let location =
        React.useMemo ((fun () -> getLocation eventCtx.Events id), [| eventCtx.Events; id |])

    let roles = useRoles ()
    let auth0 = useAuth0 ()

    React.useEffect
        ((fun () ->
            if roles.IsEditorOrAdmin && not (String.IsNullOrWhiteSpace(location.Creator)) then
                auth0.getAccessTokenSilently ()
                |> Promise.bind (fun authToken ->
                    let url =
                        sprintf "%s/users/%s" Config.backendUrl (location.Creator)

                    fetch
                        url
                        [ requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                           Config.authHeader authToken
                                           Config.subscriptionHeader ] ])
                |> Promise.bind (fun res -> res.text ())
                |> Promise.iter (fun json ->
                    let usersResult =
                        Decode.fromString nameDecoder json

                    match usersResult with
                    | Ok name -> setCreatorName (Some name)
                    | Error err -> JS.console.log err)),
         [| box roles.Roles
            box location.Creator |])

    location, creatorName

let private fact label value =
    [ dt [ ClassName Bootstrap.Col6 ] [
        str label
      ]
      dd [ ClassName Bootstrap.Col6 ] [
          str value
      ] ]

let private DetailPage =
    React.functionComponent
        ("DetailPage",
         (fun (props : {| id : string |}) ->
             let (locationDetail, creatorName) = useLocationDetail props.id

             let isDraftValue =
                 if locationDetail.IsDraft then
                     "Joat"
                 else
                     "Nint"

             let creator =
                 match creatorName with
                 | Some name -> fact "Patron" name
                 | None -> []

             let remark =
                 match creatorName, locationDetail.Remark with
                 | Some name, Some remark ->
                     blockquote [ classNames [ Bootstrap.Blockquote
                                               Bootstrap.TextCenter
                                               Bootstrap.BgLight
                                               Bootstrap.P2
                                               Bootstrap.Mt4 ] ] [
                         p [ classNames [ Bootstrap.Mb0 ] ] [
                             str remark
                         ]
                         footer [ ClassName Bootstrap.BlockquoteFooter ] [
                             cite [] [ str name ]
                         ]
                     ]
                 | _ -> ofOption None

             page [] [
                 h1 [ classNames [ Bootstrap.Pb4 ] ] [
                     str locationDetail.Name
                 ]
                 div [ classNames [ Bootstrap.Row ] ] [
                     yield! (fact "Prijs" locationDetail.Price)
                     yield! (fact "Vant vat?" isDraftValue)
                     yield! (fact "Toegevoegd op" locationDetail.Created)
                     yield! creator
                 ]
                 remark
             ]))

exportDefault DetailPage
