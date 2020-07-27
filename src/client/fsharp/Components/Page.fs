module Ronnies.Client.Components.Page

open Fable.React
open Fable.React.Props
open Ronnies.Client.Styles

let page attributes children =
    div [ ClassName "page"; yield! attributes ] [
        div
            [ classNames [ Bootstrap.Container
                           Bootstrap.P3 ] ]
            children
    ]
