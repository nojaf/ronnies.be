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
open ReactToastify
open Ronnies.Client
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page
open Ronnies.Client.Components.Loading
open Ronnies.Client.Components.Navigation
open Ronnies.Domain

type private LocationDetail =
    { Id : Guid
      Name : string
      Creator : string
      Created : string
      Price : string
      IsDraft : bool
      Remark : string option
      IsCancelled : bool
      NoLongerSellsRonnies : bool }

    static member Empty =
        { Id = Guid.Empty
          Name = "???"
          Creator = ""
          Created = ""
          Price = ""
          IsDraft = false
          Remark = None
          IsCancelled = false
          NoLongerSellsRonnies = false }

let equalsId id value = Identifier.Read id |> (=) value

let private getLocation events id =
    List.fold (fun acc event ->
        match event with
        | LocationAdded loc when (equalsId loc.Id id) ->
            { acc with
                  Id = id
                  Name = NonEmptyString.Read loc.Name
                  Creator = NonEmptyString.Read loc.Creator
                  Created = loc.Created.ToString("dd/MM/yy")
                  Price = Common.readCurrency loc.Price
                  IsDraft = loc.IsDraft
                  Remark = loc.Remark } : LocationDetail

        | LocationCancelled cid when (equalsId cid id) -> { acc with IsCancelled = true }
        | LocationNoLongerSellsRonnies nid when (equalsId nid id) -> { acc with NoLongerSellsRonnies = true }
        | LocationAdded _
        | LocationCancelled _
        | LocationNoLongerSellsRonnies _ -> acc) LocationDetail.Empty events

let nameDecoder : Decoder<string> =
    Decode.object (fun get -> get.Required.Field "name" Decode.string)

let private useLocationDetail (auth0 : Auth0Hook) (roles : RolesHook) id =
    let id = Guid.Parse(id)
    let eventCtx = React.useContext (eventContext)
    let (creatorName, setCreatorName) = React.useState<string option> (None)

    let location =
        React.useMemo ((fun () -> getLocation eventCtx.Events id), [| eventCtx.Events; id |])

    React.useEffect
        ((fun () ->
            if roles.IsEditorOrAdmin
               && not (String.IsNullOrWhiteSpace(location.Creator)) then
                auth0.getAccessTokenSilently ()
                |> Promise.bind (fun authToken ->
                    let url =
                        sprintf "%s/users/%s" Common.backendUrl (location.Creator)

                    fetch
                        url
                        [ requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                           Common.authHeader authToken
                                           Common.subscriptionHeader ] ])
                |> Promise.bind (fun res -> res.text ())
                |> Promise.iter (fun json ->
                    let usersResult = Decode.fromString nameDecoder json

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

type ActionModal =
    { Title : string
      Description : string
      Event : Event
      SuccessToastMessage : string
      FailureToastMessage : string }

let private DetailPage =
    React.functionComponent
        ("DetailPage",
         (fun (props : {| id : string |}) ->
             let roles = useRoles ()
             let auth0 = useAuth0 ()
             let navigate = useNavigate ()
             let (locationDetail, creatorName) = useLocationDetail auth0 roles props.id
             let eventCtx = React.useContext (eventContext)
             let isLoading, setIsLoading = React.useState (false)
             let (actionModal, setActionModal) = React.useState (None)

             let isDraftValue =
                 if locationDetail.IsDraft then
                     "Joat"
                 else
                     "Nint"

             let creator =
                 match creatorName with
                 | Some name -> fact "Patron" name
                 | None -> []

             let addEvent modalInfo =
                 setActionModal None
                 setIsLoading true

                 eventCtx.AddEvents [ modalInfo.Event ]
                 |> Promise.map (fun _ ->
                     successToast modalInfo.SuccessToastMessage
                     setIsLoading false
                     navigate "/")
                 |> Promise.catchEnd (fun err ->
                     JS.console.error err
                     setIsLoading false)

             let remark =
                 match creatorName, locationDetail.Remark with
                 | Some name, Some remark ->
                     blockquote [ classNames [ Bootstrap.Blockquote
                                               Bootstrap.TextCenter
                                               Bootstrap.BgLight
                                               Bootstrap.P2
                                               Bootstrap.Mt4 ] ] [
                         p [ classNames [ Bootstrap.Mb0 ] ] [
                             str (remark.Replace("\\n", "\n"))
                         ]
                         footer [ ClassName Bootstrap.BlockquoteFooter ] [
                             cite [] [ str name ]
                         ]
                     ]
                 | _ -> ofOption None

             let modalWindow =
                 actionModal
                 |> Option.map (fun modalInfo ->
                     div [ Class "modal fade show d-block"
                           TabIndex -1
                           Role "dialog" ] [
                         div [ Class "modal-dialog" ] [
                             div [ Class "modal-content" ] [
                                 div [ Class "modal-header" ] [
                                     h5 [ Class "modal-title" ] [
                                         str modalInfo.Title
                                     ]
                                     button [ Type "button"
                                              Class "close"
                                              OnClick(fun _ -> setActionModal None) ] [
                                         span [ DangerouslySetInnerHTML { __html = "&times;" } ] []
                                     ]
                                 ]
                                 div [ Class "modal-body" ] [
                                     p [] [ str modalInfo.Description ]
                                 ]
                                 div [ Class "modal-footer" ] [
                                     button [ Type "button"
                                              Class "btn btn-secondary"
                                              OnClick(fun _ -> setActionModal None) ] [
                                         str "Toe doen"
                                     ]
                                     button [ Type "button"
                                              Class "btn btn-primary"
                                              OnClick(fun _ -> addEvent modalInfo) ] [
                                         str "Bevestig"
                                     ]
                                 ]
                             ]
                         ]
                     ])

             let showCancelModal _ =
                 { Title = "Cancel location"
                   Description = "Wil je deze plekke uitschakelen? Dit is echt vo aj je met een misse zit.\nToedoen aj kei zit."
                   Event = LocationCancelled(Identifier.Parse locationDetail.Id)
                   SuccessToastMessage = sprintf "%s werd geannuleerd!" locationDetail.Name
                   FailureToastMessage = sprintf "Kon %s niet annuleren!" locationDetail.Name }
                 |> Some
                 |> setActionModal

             let showNoLongerSellsModal _ =
                 { Title = sprintf "%s verkoopt gin ronnies meer." locationDetail.Name
                   Description = "Ben je zeker dat ze hier gin ronnies meer verkopen?\nToedoen aj kei zit."
                   Event = LocationNoLongerSellsRonnies(Identifier.Parse locationDetail.Id)
                   SuccessToastMessage = sprintf "%s werd gemarkt als ronnies plekke no more!" locationDetail.Name
                   FailureToastMessage = sprintf "Kon %s niet updaten!" locationDetail.Name }
                 |> Some
                 |> setActionModal

             page [] [
                 ofOption modalWindow
                 h1 [ classNames [ Bootstrap.Pb4 ] ] [
                     if locationDetail.NoLongerSellsRonnies then
                         yield!
                             [ span [ DangerouslySetInnerHTML { __html = "&#10014;" } ] []
                               strong [] [ str "RIP " ]
                               str locationDetail.Name ]
                     else
                         str locationDetail.Name
                 ]
                 div [ classNames [ Bootstrap.Row ] ] [
                     yield! (fact "Prijs" locationDetail.Price)
                     yield! (fact "Vant vat?" isDraftValue)
                     yield! (fact "Toegevoegd op" locationDetail.Created)
                     yield! creator
                 ]
                 remark
                 if not locationDetail.NoLongerSellsRonnies
                    && roles.IsEditorOrAdmin
                    && not isLoading then
                     div [] [
                         hr []
                         if roles.IsAdmin then
                             button [ classNames [ Bootstrap.Btn
                                                   Bootstrap.BtnDanger ]
                                      OnClick showCancelModal ] [
                                 str "Cancel location"
                             ]
                         if roles.IsAdmin then
                             br []
                         button [ classNames [ Bootstrap.Btn
                                               Bootstrap.BtnWarning
                                               Bootstrap.Mt2 ]
                                  OnClick showNoLongerSellsModal ] [
                             str "Verkoopt gin ronnies nie meer"
                         ]
                     ]
                 elif isLoading then
                     loading "Syncen met de server..."

             ]))

exportDefault DetailPage
