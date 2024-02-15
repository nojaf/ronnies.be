module Iconify

open Fable.Core.JsInterop
open React
open React.Plugin
open React.DSL

#nowarn "1182"

[<RequireQualifiedAccess>]
type IconProp =
    | Icon of string
    | Width of int
    | Height of int

    interface IProp

[<JSX("Icon", "@iconify/react")>]

let icon (props : IProp seq) : ReactElement =
    jsxTransformFallback (import "Icon" "@iconify/react") props Seq.empty
