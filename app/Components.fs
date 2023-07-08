module Components

open Feliz
open React
open React.Props

[<ReactComponent>]
let inline Loader () =
    div [ ClassName "loading" ] [ div [] [] ]

type ToggleProps =
    {|
        TrueLabel : string
        FalseLabel : string
        OnChange : (bool -> unit)
        Value : bool
        Disabled : bool
    |}

[<ReactComponent>]
let inline Toggle (props : ToggleProps) =

    div [ ClassName "toggle" ] [
        button [
            if not props.Value then
                OnClick (fun ev ->
                    ev.preventDefault ()
                    props.OnChange true
                )
            else
                ClassName "active"
            Disabled props.Disabled
        ] [ str props.TrueLabel ]
        button [
            if props.Value then
                OnClick (fun ev ->
                    ev.preventDefault ()
                    props.OnChange false
                )
            else
                ClassName "active"
            Disabled props.Disabled
        ] [ str props.FalseLabel ]
    ]
