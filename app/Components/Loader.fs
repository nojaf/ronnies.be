module Components.Loader

open React.DSL
open type React.DSL.DOMProps

let Loader () =
    div [ ClassName "loading" ] [ div [] [] ]
