[<AutoOpen>]
module Types

open System

/// Firebase uid
type uid = string

type RonnyLocation =
    {|
        name : string
        price : float
        currency : string
        latitude : float
        longitude : float
        isDraft : bool
        userId : uid
        otherUserIds : uid array
        photoName : string option
        date : DateTime
    |}

let Constants = {| Locations = "locations" |}
