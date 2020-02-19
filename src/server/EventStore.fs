module Ronnies.Server.EventStore

open CosmoStore
open FSharp.Control.Tasks
open Microsoft.FSharp.Reflection
open Ronnies.Shared
open System
open Thoth.Json.Net

let private storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName")
let private storageAuthKey = Environment.GetEnvironmentVariable("StorageAccountKey")
let private config = TableStorage.Configuration.CreateDefault storageAccountName storageAuthKey
let private eventStore = TableStorage.EventStore.getEventStore config

[<Literal>]
let private EventStream = "ronnies.be"

let encodeEvent = Encode.Auto.generateEncoder<Event>()
let decodeEvent = Decode.Auto.generateDecoder<Event>()

let private getUnionCaseName (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

type EventMetaData =
    { Creator: string }
    static member Encode emd = Encode.object [ "creator", Encode.string emd.Creator ]

let private createEvent creator event =
    { Id = (Guid.NewGuid())
      CorrelationId = None
      CausationId = None
      Name = getUnionCaseName event
      Data = encodeEvent event
      Metadata = Some(EventMetaData.Encode { Creator = creator }) }

let appendEvents userId (events: Event list) =
    let cosmoEvents = List.map (createEvent userId) events
    task {
        let! _ = eventStore.AppendEvents EventStream Any cosmoEvents
        return () }

let getEvents() =
    task {
        let! cosmoEvents = eventStore.GetEvents EventStream AllEvents
        let events = List.map (fun (ce: EventRead<JsonValue, _>) -> ce.Data) cosmoEvents
        return events
    }
