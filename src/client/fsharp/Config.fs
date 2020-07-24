[<RequireQualifiedAccess>]
module Ronnies.Client.Config

open Fable.Core

[<Emit("import.meta.env.SNOWPACK_PUBLIC_BACKEND")>]
let backendUrl : string = jsNative
