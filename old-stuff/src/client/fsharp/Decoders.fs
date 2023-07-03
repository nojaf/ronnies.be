module Ronnies.Client.Decoders

open Thoth.Json
open Ronnies.Shared

let private decodeEvent = Decode.Auto.generateDecoder<Event> ()

let decodeEvents json =
    Decode.fromString (Decode.list decodeEvent) json

let private encodeEvent = Encode.Auto.generateEncoder<Event> ()

let encodeEvents events =
    events
    |> List.map encodeEvent
    |> Encode.list
    |> Encode.toString 2
