module Ronnies.Client.Pages.Home

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz

let private HomePage =
    React.functionComponent ("HomePage", (fun () -> span [] []))

exportDefault HomePage
