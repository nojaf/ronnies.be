[<AutoOpen>]
module Types

open System
open Fable.Core.JsInterop
open Firebase
open Firebase.FireStore

type LatLng = float * float

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
        photoNames : string array
        remark : string
        date : Timestamp
    |}

let Constants = {| Locations = "locations" |}

let auth : Auth.Auth = import "auth" "./firebase.config.js"
let storage : Storage.FirebaseStorage = import "storage" "./firebase.config.js"
let firestore : FireStore.FireStore = import "firestore" "./firebase.config.js"
let functions : Functions.Functions = import "functions" "./firebase.config.js"

let messaging : unit -> Fable.Core.JS.Promise<Messaging.Messaging> =
    import "messaging" "./firebase.config.js"

let allRonniesQuery =
    FireStore.Exports.query<RonnyLocation> (FireStore.Exports.collection (firestore, Constants.Locations))
