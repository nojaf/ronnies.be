﻿module Components.Toggle

open Fable.Core
open Browser.Types
open React.DSL
open type React.DSL.DOMProps
open StyledComponents

type ToggleProps =
    {|
        trueLabel : string
        falseLabel : string
        onChange : bool -> unit
        value : bool
        disabled : bool
    |}

let private StyledDiv : JSX.ElementType =
    mkStyleComponent
        "div"
        """
button.active {
    outline: 2px solid var(--primary);
    z-index: 2;
    color: var(--primary);
}

button.active:disabled {
    background-color: #fafbfc;
    border-color: #1b1f2326;
    color: #959da5;
    cursor: default;
    outline: none;
}

button:first-child {
    border-right: none;
}

button:last-child {
    border-left: none;
}
"""

let inline private styledDiv (children : JSX.Element seq) : JSX.Element =
    JSX.create StyledDiv [ "children", children ]

let Toggle (props : ToggleProps) =
    let onClick (value : bool) (ev : MouseEvent) =
        ev.preventDefault ()
        props.onChange value

    styledDiv [
        button [
            Key "true-button"
            OnClick (if props.value then ignore else onClick true)
            ClassName (if props.value then "active" else null)
            Disabled props.disabled
        ] [ str props.trueLabel ]
        button [
            Key "false-button"
            OnClick (if not props.value then ignore else onClick false)
            ClassName (if props.value then null else "active")
            Disabled props.disabled
        ] [ str props.falseLabel ]
    ]
