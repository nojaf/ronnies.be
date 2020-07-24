module Ronnies.Client.Components.Loading

open Fable.React
open Fable.React.Props
open Ronnies.Client.Styles

let loading =
    div [ classNames [ Bootstrap.DFlex
                       Bootstrap.JustifyContentCenter ] ] [
        div [ classNames [ Bootstrap.SpinnerBorder
                           Bootstrap.TextPrimary ] ] []
    ]
