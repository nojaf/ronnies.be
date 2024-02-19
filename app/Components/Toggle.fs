module Components.Toggle

open Browser.Types
open React.DSL
open type React.DSL.DOMProps

type ToggleProps =
    {|
        trueLabel : string
        falseLabel : string
        onChange : bool -> unit
        value : bool
        disabled : bool
    |}

let Toggle (props : ToggleProps) =
    let onClick (value : bool) (ev : MouseEvent) =
        ev.preventDefault ()
        props.onChange value

    div [ ClassName "toggle" ] [
        button [
            Key "true-button"
            OnClick (if props.value then ignore else onClick true)
            ClassName (if props.value then null else "active")
            Disabled props.disabled
        ] [ str props.trueLabel ]
        button [
            Key "false-button"
            OnClick (if not props.value then ignore else onClick false)
            ClassName (if not props.value then null else "active")
            Disabled props.disabled
        ] [ str props.falseLabel ]
    ]
