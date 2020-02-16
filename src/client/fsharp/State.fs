module Ronnies.Client.State

open Elmish
open Ronnies.Client.Model

let initialState: Model =
    { Events = [] }

let init _ = initialState, Cmd.none

let update msg model = model, Cmd.none
