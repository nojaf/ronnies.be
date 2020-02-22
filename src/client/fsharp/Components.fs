module Ronnies.Client.Components

open Fable.React
open Ronnies.Client.Model
open Ronnies.Client.Hooks

let WhenEditor =
    FunctionComponent.Of
        ((fun (props: {| children: ReactElement |}) ->
            let role = useRole()
            match role with
            | Role.Editor
            | Role.Admin -> props.children
            | _ -> null), "WhenEditor", equalsButFunctions)
