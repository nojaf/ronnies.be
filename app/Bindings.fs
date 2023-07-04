module Bindings

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

#nowarn "1182"

type React =
    [<Import("useState", "react")>]
    static member useState<'T> (initial : 'T) : 'T * ('T -> unit) = jsNative

    [<Import("useEffect", "react")>]
    static member useEffect (callback : unit -> unit) (deps : obj array) : unit = jsNative

[<RequireQualifiedAccess>]
type IconProps =
    | Icon of string
    | Width of int
    | Height of int

    interface IProp

let mkPropObject (props : IProp seq) =
    let isProp (o : IProp) =
        emitJsExpr o "$0.name && $0.fields && $0.fields.length === 1"

    let name (o : IProp) =
        emitJsExpr o "$0.name.charAt(0).toLowerCase() + $0.name.slice(1)"

    let field (o : IProp) = emitJsExpr o "$0.fields[0]"

    props
    |> Seq.choose (fun prop ->
        if JS.Constructors.Array.isArray prop then
            let array : obj array = emitJsExpr prop "$0"

            if array.Length = 2 then
                Some (!!array.[0], array.[1])
            else
                None
        elif isProp prop then
            Some (name prop, field prop)
        else
            JS.console.warn ("Prop is not parsed", prop)
            None
    )
    |> createObj

let inline Icon (props : IProp seq) =
    ofImport "Icon" "@iconify/react" (mkPropObject props) Array.empty

let useMediaQuery (query : string) : bool = import "useMediaQuery" "usehooks-ts"

let BrowserRouter (children : ReactElement seq) =
    ofImport "BrowserRouter" "react-router-dom" null children

let Routes (routes : ReactElement seq) =
    ofImport "Routes" "react-router-dom" null routes

let Route<'route> (props : 'route) =
    ofImport "Route" "react-router-dom" props Array.empty

type ReactRouterProp =
    | To of string

    interface IProp

let inline Link (props : IProp seq) (children : ReactElement seq) =
    ofImport "Link" "react-router-dom" (mkPropObject props) children

let inline NavLink (props : IProp seq) (children : ReactElement seq) =
    let className (prop : {| isActive : bool |}) = if prop.isActive then "active" else ""

    ofImport
        "NavLink"
        "react-router-dom"
        (mkPropObject [| yield! props ; DOMAttr.Custom ("className", !!className) |])
        children
