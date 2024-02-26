module StyledComponents

open Fable.Core.JsInterop

let styled : obj = import "styled" "styled-components"

let inline mkStyleComponent tag css =
    emitJsExpr (styled, tag, css) """$0[$1]`${$2}`"""
