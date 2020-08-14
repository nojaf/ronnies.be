[<RequireQualifiedAccess>]
module Ronnies.Server.EventStore

open CosmoStore
open FSharp.Control.Tasks
open Microsoft.FSharp.Reflection
open Ronnies.Domain
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

let getUnionCaseName (x : 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

type EventMetaData =
    { Creator : string }

    static member Encode emd =
        Encode.object [ "creator", Encode.string emd.Creator ]

let private createEvent creator event =
    { Id = (Guid.NewGuid())
      CorrelationId = None
      CausationId = None
      Name = getUnionCaseName event
      Data = Ronnies.Domain.Event.Encoder event
      Metadata = Some(EventMetaData.Encode { Creator = creator }) }

[<Literal>]
let private BatchLimit = 99

let rec private appendToAzureTableStorage (cosmoEvents : EventWrite<JsonValue> seq) =
    task {
        let moreThanBatchLimit = Seq.length cosmoEvents > BatchLimit

        let batch =
            if moreThanBatchLimit then
                Seq.take BatchLimit cosmoEvents
            else
                cosmoEvents
            |> List.ofSeq

        let! events = eventStore.AppendEvents EventStream Any batch

        if moreThanBatchLimit then
            let rest = Seq.skip BatchLimit cosmoEvents
            let! others = appendToAzureTableStorage rest
            return events @ others
        else
            return events
    }

let appendEvents userId (events : Event list) =
    let cosmoEvents = List.map (createEvent userId) events
    task { return! appendToAzureTableStorage cosmoEvents }

let getEvents (lastEvent : int64 option) =
    task {
        let! cosmoEvents =
            match lastEvent with
            | Some lastVersion -> eventStore.GetEvents EventStream (EventsReadRange.FromVersion(lastVersion + 1L))
            | None -> eventStore.GetEvents EventStream EventsReadRange.AllEvents

        let events =
            List.map (fun (ce : EventRead<JsonValue, _>) -> ce.Version.ToString(), ce.Data) cosmoEvents

        return events
    }
