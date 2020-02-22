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
