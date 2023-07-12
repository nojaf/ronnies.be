module ReactRouterDom

open Fable.Core.JsInterop
open React
open React.Props

#nowarn "1182"

let inline BrowserRouter (children : ReactElement seq) =
    ofImportWithoutProps "BrowserRouter" "react-router-dom" children

let inline Routes (routes : ReactElement seq) =
    ofImportWithoutProps "Routes" "react-router-dom" routes

let inline Route (props : IProp seq) =
    ofImportWithoutChildren "Route" "react-router-dom" props

type ReactRouterProp =
    | To of string
    | Index of bool
    | Path of string
    | Element of ReactElement

    interface IProp

let inline Navigate (props : IProp seq) =
    ofImportWithoutChildren "Navigate" "react-router-dom" props

let inline Link (props : IProp seq) (children : ReactElement seq) =
    ofImport "Link" "react-router-dom" props children

let inline NavLink (props : IProp seq) (children : ReactElement seq) =
    let className (prop : {| isActive : bool |}) = if prop.isActive then "active" else ""

    ofImport "NavLink" "react-router-dom" [| yield! props ; DOMAttr.Custom ("className", !!className) |] children

let useNavigate () : string -> unit = import "useNavigate" "react-router-dom"

let useParams<'T> () : 'T = import "useParams" "react-router-dom"
