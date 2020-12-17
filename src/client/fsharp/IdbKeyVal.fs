module Ronnies.Client.IdbKeyVal

open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json
open Fetch
open Ronnies.Domain
open Ronnies.Client

type private Store =
    interface
    end

[<Import("Store", "idb-keyval")>]
[<Emit("new $0($1,$2)")>]
let private createStore (_dbName : string, _storeName : string) : Store = jsNative

let private get<'t> (_key : int) (_store : Store) : JS.Promise<'t> = import "get" "idb-keyval"
let private set<'t> (_key : int) (_value : 't) (_store : Store) : JS.Promise<unit> = import "set" "idb-keyval"
let private keys : Store -> JS.Promise<int array> = import "keys" "idb-keyval"
let private clear : Store -> unit = import "clear" "idb-keyval"
let private ronniesStore = createStore ("ronnies.be", "events")

let private getLastEvent () =
    keys ronniesStore
    |> Promise.map
        (fun keys ->
            if Array.isEmpty keys then
                None
            else
                keys |> Array.max |> Some)

let addEvent version event : JS.Promise<unit> = set version event ronniesStore

let getAllEvents () : JS.Promise<Event list> =
    keys ronniesStore
    |> Promise.bind
        (fun keys ->
            keys
            |> Array.map
                (fun key ->
                    get key ronniesStore
                    |> Promise.bind
                        (fun evJson ->
                            match Decode.fromValue "$root" Event.Decoder evJson with
                            | Ok ev -> Promise.lift ev
                            | Error err -> Promise.reject err))
            |> Promise.all
            |> Promise.map (List.ofArray))

let private addEventsToIdb (response : Response) =
    response.text ()
    |> Promise.bind
        (fun json ->
            let result =
                Decode.fromString (Decode.keyValuePairs Event.Decoder) json

            match result with
            | Ok events ->
                let persistEventsPromise =
                    events
                    |> List.map (fun (k, v) -> addEvent ((int) k) (Event.Encoder v))
                    |> Promise.all
                    |> Promise.map (fun _ -> [])

                let newEventsPromise = Promise.lift (List.map snd events)

                Promise.all
                    [ newEventsPromise
                      persistEventsPromise ]
                |> Promise.map (List.concat)
            | Error err -> Promise.reject err)

let syncLatestEvents () =
    getLastEvent ()
    |> Promise.bind
        (fun lastEvent ->
            let url =
                match lastEvent with
                | Some id -> sprintf "%s/events?lastEvent=%i" Common.backendUrl id
                | None -> sprintf "%s/events" Common.backendUrl

            fetch
                url
                [ requestHeaders
                    [ HttpRequestHeaders.ContentType "application/json"
                      Common.subscriptionHeader ] ])
    |> Promise.bind addEventsToIdb

let persistEvents (events : Event list) authToken =
    let url = sprintf "%s/events" Common.backendUrl

    let json =
        events
        |> List.map Event.Encoder
        |> Encode.list
        |> Encode.toString 4

    fetch
        url
        [ RequestProperties.Method HttpMethod.POST
          RequestProperties.Body(!^json)
          requestHeaders
              [ HttpRequestHeaders.ContentType "application/json"
                Common.authHeader authToken
                Common.subscriptionHeader ] ]
    |> Promise.bind addEventsToIdb

let removeAllEvents () = clear ronniesStore
