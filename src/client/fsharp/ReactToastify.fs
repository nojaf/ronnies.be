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
    abstract error : string -> obj -> unit

let private toast : Toast = import "toast" "react-toastify"

let private defaultToastOptions =
    [ ToastOptions.ToastPosition ToastPosition.BottomRight
      ToastOptions.HideProgressBar true ]
    |> keyValueList CaseRules.LowerFirst

let successToast title = toast.success title defaultToastOptions

let infoToast title = toast.info title defaultToastOptions

let errorToast title = toast.error title defaultToastOptions
