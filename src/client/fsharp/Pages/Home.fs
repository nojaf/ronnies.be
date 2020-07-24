module Ronnies.Client.Pages.Home

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Components.WorldMap

let private HomePage =
    React.functionComponent ("HomePage", (fun () -> WorldMap ()))

exportDefault HomePage
