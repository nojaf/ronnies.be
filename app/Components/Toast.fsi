module Components.Toast

open Fable.Core

type ToastProps =
    {|
        url : string
        title : string
        body : string
        onClick: unit -> unit
    |}

[<ExportDefault>]
val Toast: props: ToastProps -> JSX.Element
