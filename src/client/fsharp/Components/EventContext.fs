module Ronnies.Client.Components.EventContext

open Fable.Core
open Feliz
open Fetch
open Thoth.Json
open Ronnies.Domain
open Ronnies.Client

[<NoComparison>]
[<NoEquality>]
type EventContext =
    { Events : Event list
      AddEvents : Event list -> JS.Promise<unit> }

let private emptyEventContext =
    { Events = []
      AddEvents = fun _ -> Promise.lift () }

let eventContext =
    React.createContext<EventContext> (defaultValue = emptyEventContext)

/// Syncs the latest events from the backend to the IDB store
/// Returns all events
let private fetchLatestEvents setEvents =
    IdbKeyVal.syncLatestEvents ()
    |> Promise.bind (fun _ -> IdbKeyVal.getAllEvents ())
    |> Promise.iter setEvents

let Events =
    React.functionComponent
        ("Events",
         (fun (props : {| children : ReactElement |}) ->
             let (events, setEvents) = React.useState ([])

             let addEvents (appendEvents : Event list) = Promise.lift ()

             React.useEffectOnce (fun () -> fetchLatestEvents setEvents)

             let contextValue =
                 { Events = events
                   AddEvents = addEvents }

             React.contextProvider (eventContext, contextValue, props.children)))
