module ReactToastify

open Fable.Core
open Fable.Core.JsInterop

[<StringEnum>]
type ToastPosition =
    | [<CompiledName("top-right")>] TopRight
    | [<CompiledName("top-center")>] TopCenter
    | [<CompiledName("top-left")>] TopLeft
    | [<CompiledName("bottom-right")>] BottomRight
    | [<CompiledName("bottom-center")>] BottomCenter
    | [<CompiledName("bottom-left")>] BottomLeft

type ToastOptions =
    | [<CompiledName("position")>] ToastPosition of ToastPosition
    | HideProgressBar of bool

type private Toast =
    abstract info : string -> obj -> unit
    abstract success : string -> obj -> unit

let private toast : Toast = import "toast" "react-toastify"

let successToast title (options : ToastOptions seq) =
    let opt =
        keyValueList Fable.Core.CaseRules.LowerFirst options

    toast.success title opt

let infoToast title (options : ToastOptions seq) =
    let opt =
        keyValueList Fable.Core.CaseRules.LowerFirst options

    toast.info title opt
