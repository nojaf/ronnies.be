module Ronnies.Client.State

open Elmish
open Ronnies.Client.Model
open Fable.Core.JsInterop

let initialState: Model =
    { Events = []
      AuthorizationToken = None
      Role = Role.Visitor }

let init _ = initialState, Cmd.none

let private getPermissions (token: string): string array = import "getPermissions" "./js/jwt"

let private getRole token =
    let permissions = getPermissions token
    let hasPermission name = Array.exists (fun p -> p = name) permissions

    if hasPermission "delete:location" then Role.Admin
    elif hasPermission "write:location" then Role.Editor
    else Role.Visitor

let update msg model =
    printfn "update"
    match msg with
    | SetToken token ->
        { model with
              AuthorizationToken = Some token
              Role = getRole token }, Cmd.none
