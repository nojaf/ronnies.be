module Ronnies.Client.State

open Elmish
open Ronnies.Client.Decoders
open Ronnies.Client.Model
open Fable.Core.JsInterop
open Fable.Core
open Fetch


let initialState: Model =
    { Events = []
      AuthorizationToken = None
      Role = Role.Visitor
      UserId = None
      AppException = None
      IsLoading = true
      Toasts = Map.empty }

[<Emit("process.env.REACT_APP_BACKEND")>]
let private baseUrl: string = jsNative

let private getEvents () =
    let url = sprintf "%s/api/GetEvents" baseUrl
    fetch url []
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map (fun json ->
        match decodeEvents json with
        | Ok events -> events
        | Error e -> failwithf "%s" e)

let init _ =
    initialState, Cmd.OfPromise.either getEvents () Msg.EventsReceived Msg.AppException

let private getPermissions (_token: string): string array = import "getPermissions" "./js/jwt"

let private getRole token =
    let permissions = getPermissions token

    let hasPermission name =
        Array.exists (fun p -> p = name) permissions

    if hasPermission "delete:location" then Role.Admin
    elif hasPermission "write:location" then Role.Editor
    else Role.Visitor

let private getUserId (_token: string): string option = import "getUserId" "./js/jwt"

let private authorizationHeader token =
    requestHeaders [ HttpRequestHeaders.Authorization(sprintf "bearer %s" token) ]

let private addLocationEvents (token, event) =
    let url = sprintf "%s/api/AddEvents" baseUrl
    let json = encodeEvents (List.singleton event)
    JS.console.log json
    fetch url
        [ RequestProperties.Body(!^json)
          RequestProperties.Method HttpMethod.POST
          authorizationHeader token ]
    |> Promise.map (ignore)

let private nextKey map =
    if Map.isEmpty map then
        0
    else
        Map.toArray map
        |> Array.map fst
        |> Array.max
        |> (+) 1

let private hideToastIn toastId miliSecondes dispatch =
    JS.setTimeout (fun () -> dispatch (Msg.ClearToast toastId)) miliSecondes
    |> ignore

let update msg model =
#if DEBUG
    printfn "update, %A" msg
#endif
    match msg with
    | SetToken token ->
        { model with
              AuthorizationToken = Some token
              Role = getRole token
              UserId = getUserId token },
        Cmd.none
    | EventsReceived events ->
        { model with
              Events = events
              IsLoading = false },
        Cmd.none
    | AppException exn ->
        { model with
              AppException = Some exn
              IsLoading = false },
        Cmd.none
    | AddLocation event ->
        let cmd =
            match model.AuthorizationToken with
            | Some authToken -> Cmd.OfPromise.either addLocationEvents (authToken, event) LocationAdded AppException
            | None -> Cmd.none

        let events = event :: model.Events

        { model with
              IsLoading = true
              Events = events },
        cmd
    | LocationAdded _ ->
        let cmd =
            { Icon = "success"
              Title = "Saved"
              Body = "Jupse, alle data in the cloud enzo" }
            |> Msg.ShowToast
            |> Cmd.ofMsg

        { model with IsLoading = false }, cmd

    | ShowToast (toast) ->
        let toastId = nextKey model.Toasts
        let toasts = Map.add toastId toast model.Toasts
        { model with Toasts = toasts }, Cmd.ofSub (hideToastIn toastId 2500)

    | ClearToast toastId ->
        let toasts = Map.remove toastId model.Toasts
        { model with Toasts = toasts }, Cmd.none
