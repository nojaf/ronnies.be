module ComponentsDSL

open Fable.Core.JsInterop
open React
open React.Plugin
open React.DSL
open Components

[<JSX(nameof Loader, __SOURCE_DIRECTORY__ + "/Components.fs")>]
let loader (props : #IProp seq) : ReactElement =
    jsxTransformFallback Loader props Seq.empty

[<RequireQualifiedAccess>]
type ToggleProp =
    | [<CompiledName "trueLabel">] TrueLabel of string
    | FalseLabel of string
    | OnChange of (bool -> unit)
    | Value of bool
    | Disabled of bool

    interface IProp

[<JSX(nameof Toggle, __SOURCE_DIRECTORY__ + "/Components.fs")>]
let toggle (props : #IProp seq) : ReactElement =
    jsxTransformFallback Toggle props Seq.empty
