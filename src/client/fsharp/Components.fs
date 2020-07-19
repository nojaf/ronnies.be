module Ronnies.Client.Components

open Fable.React
open Fable.React.Props
open Feliz
//open Ronnies.Client.Model
//open Ronnies.Client.Hooks

//let WhenEditor =
//    React.functionComponent(fun ({| children: ReactElement |}) ->
//
//            let role = useRole ()
//            match role with
//            | Role.Editor
//            | Role.Admin -> props.children
//            | _ -> null)

let loading info =
    div [ Id "preloader" ] [
        div [ Id "loader" ] [ str info ]
    ]
