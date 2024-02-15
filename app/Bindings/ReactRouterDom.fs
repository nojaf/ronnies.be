module ReactRouterDom

open Fable.Core.JsInterop
open React
open React.Plugin

#nowarn "1182"

[<JSX("BrowserRouter", "react-router-dom")>]
let browserRouter (props : #IProp seq) (children : ReactElement seq) : ReactElement = null

[<JSX("Routes", "react-router-dom")>]
let routes (props : #IProp seq) (routes : ReactElement seq) : ReactElement = null

[<JSX("Route", "react-router-dom")>]
let route (props : IProp seq) : ReactElement = null

type ReactRouterProp =
    | To of string
    | Index of bool
    | Path of string
    | Element of ReactElement

    interface IProp

[<JSX("Navigate", "react-router-dom")>]
let navigate (props : IProp seq) : ReactElement = null

[<JSX("Link", "react-router-dom")>]
let link (props : IProp seq) (children : ReactElement seq) : ReactElement = null

[<JSX("NavLink", "react-router-dom")>]
let navLink (props : IProp seq) (children : ReactElement seq) : ReactElement = null

let useNavigate () : string -> unit = import "useNavigate" "react-router-dom"

let useParams<'T> () : 'T = import "useParams" "react-router-dom"
