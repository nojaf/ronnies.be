module UseHooksTs

#nowarn "1182"

open Fable.Core.JsInterop

let useMediaQuery (query : string) : bool = import "useMediaQuery" "usehooks-ts"
