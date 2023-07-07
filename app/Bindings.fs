module Bindings

open Fable.Core.JsInterop
open React
open React.Props

#nowarn "1182"

[<RequireQualifiedAccess>]
type IconProps =
    | Icon of string
    | Width of int
    | Height of int

    interface IProp

let inline Icon (props : IProp seq) =
    ofImportWithoutChildren "Icon" "@iconify/react" props

let useMediaQuery (query : string) : bool = import "useMediaQuery" "usehooks-ts"

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
