module Ronnies.Client.View

open Elmish
open Fable.React
open Ronnies.Client.Model
open Ronnies.Client

[<NoComparison>]
type AppContext =
    { Model: Model
      Dispatch: Dispatch<Msg> }

let private defaultContextValue: AppContext = Fable.Core.JS.undefined
let appContext = ReactBindings.React.createContext (defaultContextValue)

let ElmishCapture =
    FunctionComponent.Of
        ((fun (props: {| children: ReactElement |}) ->

            let state: IStateHook<AppContext> =
                Hooks.useState
                    ({ Model = State.initialState
                       Dispatch = ignore })

            let view model dispatch =
                state.update
                    ({ Model = model
                       Dispatch = dispatch })

            Hooks.useEffect
                ((fun () -> Program.mkProgram State.init State.update view |> Program.run), Array.empty)

            contextProvider appContext state.current [ props.children ]), "ElmishCapture", memoEqualsButFunctions)
