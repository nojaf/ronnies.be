module Ronnies.Client.Pages.Home

open Fable.Core.JsInterop
open Feliz
open Ronnies.Client.Components.WorldMap

let private HomePage =
    React.functionComponent ("HomePage", (fun () -> WorldMap()))

exportDefault HomePage
