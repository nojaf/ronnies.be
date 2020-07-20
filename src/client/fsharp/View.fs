module Ronnies.Client.View

open Elmish
open Feliz
open Fable.React
open Ronnies.Client.Model
open Ronnies.Client
open Feliz.UseElmish

[<NoComparison>]
type AppContext =
    { Model : Model
      Dispatch : Dispatch<Msg> }

let private defaultContextValue : AppContext = Fable.Core.JS.undefined

let appContext =
    React.createContext (defaultValue = defaultContextValue)

let ElmishCapture =
    React.functionComponent
        ("ElmishComponent",
         (fun (props : {| children : ReactElement |}) ->
             let model, dispatch =
                 React.useElmish (State.init, State.update, [||])

             React.contextProvider (appContext, ({ Model = model; Dispatch = dispatch }), props.children)))
