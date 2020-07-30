module Ronnies.Client.Pages.Leaderboard

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Browser.Navigator
open Fable.Import
open Fable.React
open Fable.React.Props
open Feliz
open ReactToastify
open Auth0
open Fetch
open Ronnies.Client
open Ronnies.Client.Components.Loading
open Ronnies.Client.Components.Switch
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page

let LeaderboardPage =
    React.functionComponent ("LeaderboardPage", (fun () -> page [] [ str "LeaderboardPage" ]))

exportDefault LeaderboardPage
