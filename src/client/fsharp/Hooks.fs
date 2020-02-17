module Ronnies.Client.Hooks

open Fable.Core
open Fable.React
open Ronnies.Client.Model
open Ronnies.Client.View

let private f g = System.Func<_, _>(g)

let private useModel() =
    let { Model = model } = Hooks.useContext (appContext)
    model

let private useDispatch() =
    let { Dispatch = dispatch } = Hooks.useContext (appContext)
    dispatch

let useSetToken() =
    let dispatch = useDispatch()
    f (fun token -> SetToken token |> dispatch)

let useDump() =
    let model = useModel()
    JS.JSON.stringify (model, space = 4)
