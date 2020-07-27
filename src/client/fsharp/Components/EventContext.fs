module Ronnies.Client.Components.EventContext

open Fable.Core
open Feliz
open Auth0
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
    IdbKeyVal.getAllEvents ()
    |> Promise.bind (fun existingEvents ->
        setEvents existingEvents
        let newEvents = IdbKeyVal.syncLatestEvents ()
        Promise.all [ Promise.lift existingEvents
                      newEvents ])
    |> Promise.iter (List.concat >> setEvents)

let Events =
    React.functionComponent
        ("Events",
         (fun (props : {| children : ReactElement |}) ->
             let (events, setEvents) = React.useState ([])
             let auth0 = useAuth0 ()

             let addEvents (appendEvents : Event list) =
                 auth0.getAccessTokenSilently ()
                 |> Promise.bind (IdbKeyVal.persistEvents appendEvents)
                 |> Promise.map (fun persistedEvents -> setEvents (events @ persistedEvents))

             React.useEffectOnce (fun () -> fetchLatestEvents setEvents)

             let contextValue =
                 { Events = events
                   AddEvents = addEvents }

             React.contextProvider (eventContext, contextValue, props.children)))
