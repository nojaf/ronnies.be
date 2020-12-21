module Ronnies.Client.Pages.Home

open Fable.Core.JsInterop
open Feliz
open Ronnies.Client.Components.WorldMap

[<ReactComponent>]
let private HomePage () = WorldMap()

exportDefault HomePage
