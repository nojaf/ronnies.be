module Ronnies.Client.Components.Loading

open Fable.React
open Fable.React.Props

let loading info =
    div [ Id "preloader" ] [
        div [ Id "loader" ] [ str info ]
    ]
