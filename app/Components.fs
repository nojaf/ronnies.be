module Components

open React.DSL
open React.DSL.Props

let Loader () =
    div [ ClassName "loading" ; Key "loader" ] [ div [] [] ]

type ToggleProps =
    {|
        trueLabel : string
        falseLabel : string
        onChange : (bool -> unit)
        value : bool
        disabled : bool
    |}

let Toggle (props : ToggleProps) =

    div [ ClassName "toggle" ; Key "toggle-container" ] [
        button [
            Key "true-button"
            if not props.value then
                OnClick (fun ev ->
                    ev.preventDefault ()
                    props.onChange true
                )
            else
                ClassName "active"
            Disabled props.disabled
        ] [ str props.trueLabel ]
        button [
            Key "false-button"
            if props.value then
                OnClick (fun ev ->
                    ev.preventDefault ()
                    props.onChange false
                )
            else
                ClassName "active"
            Disabled props.disabled
        ] [ str props.falseLabel ]
    ]
