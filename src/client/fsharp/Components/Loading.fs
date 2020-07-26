module Ronnies.Client.Components.Loading

open Fable.React
open Fable.React.Props
open Ronnies.Client.Styles

let loading message =
    div [ classNames [ Bootstrap.TextCenter ]] [
        div [ classNames [ Bootstrap.DFlex
                           Bootstrap.JustifyContentCenter ] ] [
            div [ classNames [ Bootstrap.SpinnerBorder
                               Bootstrap.TextPrimary ] ] []
        ]
        p [ classNames [ Bootstrap.Mt3; Bootstrap.TextPrimary; Bootstrap.FontItalic ] ] [
            str message
        ]
    ]

