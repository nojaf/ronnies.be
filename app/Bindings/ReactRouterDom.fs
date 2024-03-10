module ReactRouterDom

open Fable.Core
open Fable.Core.JsInterop
open React.Plugin

#nowarn "1182"

[<JSX("BrowserRouter", "react-router-dom")>]
let browserRouter (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null

[<JSX("Routes", "react-router-dom")>]
let routes (props : JSX.Prop seq) (routes : JSX.Element seq) : JSX.Element = null

[<JSX("Route", "react-router-dom")>]
let route (props : JSX.Prop seq) : JSX.Element = null

[<RequireQualifiedAccess>]
type ReactRouterProp =
    [<Emit("to")>]
    static member To (value : string) : JSX.Prop = "to", box value

    [<Emit "index">]
    static member Index (value : bool) = "index", box value

    [<Emit "path">]
    static member Path (value : string) = "path", box value

    [<Emit "element">]
    static member Element (value : JSX.Element) = "element", box value

[<JSX("Navigate", "react-router-dom")>]
let navigate (props : JSX.Prop seq) : JSX.Element = null

[<JSX("Link", "react-router-dom")>]
let link (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null

[<JSX("NavLink", "react-router-dom")>]
let navLink (props : JSX.Prop seq) (children : JSX.Element seq) : JSX.Element = null

let useNavigate () : string -> unit = import "useNavigate" "react-router-dom"

let useParams<'T> () : 'T = import "useParams" "react-router-dom"
