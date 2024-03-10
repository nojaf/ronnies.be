module Iconify

open Fable.Core
open React.Plugin

#nowarn "1182"

[<RequireQualifiedAccess>]
type IconProp =
    [<Emit "icon">]
    static member Icon (value : string) : JSX.Prop = "icon", box value

    [<Emit "width">]
    static member Width (value : int) : JSX.Prop = "width", box value

    [<Emit "height">]
    static member Height (value : int) : JSX.Prop = "height", box value

[<JSX("Icon", "@iconify/react")>]
let icon (props : JSX.Prop seq) : JSX.Element = null
