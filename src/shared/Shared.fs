module Ronnies.Shared

open System

type EventSource = Guid // The source/instance the event is related to.

type UserId = string

type Identifier = Guid

type Location = float * float

type Price = float

type LocationAdded =
    { Id: Identifier
      Name: string
      Location: Location
      Price: Price
      IsDraft: bool
      Remark: string option
      Date: DateTime
      Creator: UserId }

type LocationAddedNotification = unit

type Event =
    | LocationAdded of LocationAdded
    | NameUpdated of id: Identifier * name: string
    | PriceUpdated of id: Identifier * price: Price
    | LocationUpdated of id: Identifier * location: Location
    | RemarkUpdated of id: Identifier * description: string
    | IsDraftUpdated of id: Identifier * isDraft: bool
    | NewLeaderInHighScores of user: EventSource * score: int
    | LocationAddedNotificationSent of LocationAddedNotification
    | LocationMarkedAsDuplicate of id: Identifier

let newId (): Identifier = System.Guid.NewGuid()

[<RequireQualifiedAccess>]
module Projections =
    let getTotal events =
        events
        |> List.filter (function
            | LocationAdded _ -> true
            | _ -> false)
        |> List.length

    let getRonniesWithLocation events =
        events
        |> List.choose (function
            | LocationAdded ({ Location = location; Name = name }) -> Some(name, location)
            | _ -> None)

    let getRonnies projection events =
        events
        |> List.choose (function
            | LocationAdded la -> Some(projection la)
            | _ -> None)
