module ReactLazyLoad

open Fable.Core
open React.Plugin

#nowarn "1182"

[<RequireQualifiedAccess>]
type LazyLoadProp =
    [<Emit "offset">]
    static member Offset (value : int) : JSX.Prop = "offset", box value

    [<Emit "height">]
    static member Height (value : int) : JSX.Prop = "height", box value

[<JSX("default as LazyLoad", "react-lazy-load")>]
let lazyLoad (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null
