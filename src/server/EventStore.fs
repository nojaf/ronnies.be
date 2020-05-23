module Ronnies.Server.EventStore

open CosmoStore
open FSharp.Control.Tasks
open Microsoft.FSharp.Reflection
open Ronnies.Shared
open System
open Thoth.Json.Net

let private storageAccountName =
    Environment.GetEnvironmentVariable("StorageAccountName")

let private storageAuthKey =
    Environment.GetEnvironmentVariable("StorageAccountKey")

let private config =
    TableStorage.Configuration.CreateDefault storageAccountName storageAuthKey

let private eventStore =
    TableStorage.EventStore.getEventStore config

[<Literal>]
let private EventStream = "ronnies.be"

let encodeEvent = Encode.Auto.generateEncoder<Event> ()
let decodeEvent = Decode.Auto.generateDecoder<Event> ()

let getUnionCaseName (x: 'a) =
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

[<Literal>]
let private BatchLimit = 99

let rec private appendToAzureTableStorage (cosmoEvents: EventWrite<JsonValue> seq) =
    task {
        let moreThanBatchLimit = Seq.length cosmoEvents > BatchLimit

        let batch =
            if moreThanBatchLimit then Seq.take BatchLimit cosmoEvents else cosmoEvents
            |> List.ofSeq

        let! _ = eventStore.AppendEvents EventStream Any batch

        if moreThanBatchLimit then
            let rest = Seq.skip BatchLimit cosmoEvents
            return! appendToAzureTableStorage rest
        else
            return ()
    }

let appendEvents userId (events: Event list) =
    let cosmoEvents = List.map (createEvent userId) events
    task { do! appendToAzureTableStorage cosmoEvents }

let getEvents () =
    task {
        let! cosmoEvents = eventStore.GetEvents EventStream AllEvents

        let events =
            List.map (fun (ce: EventRead<JsonValue, _>) -> ce.Data) cosmoEvents

        return events
    }
