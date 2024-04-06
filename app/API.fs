[<RequireQualifiedAccess>]
module API

open Fable.Core
open type Firebase.Functions.Exports
open Ronnies.Shared

type User = {| displayName : string ; uid : uid |}

let private users = httpsCallable<UsersData, User array> (functions, "users")

let getUsers (data : UsersData) : JS.Promise<User array> =
    users data |> Promise.map (fun result -> result.data)

type AddUser =
    {|
        displayName : string
        email : string
    |}

let private user = httpsCallable<AddUser, obj> (functions, "user")

let addUser name email : JS.Promise<obj> =
    user {| displayName = name ; email = email |}
    |> Promise.map (fun result -> result.data)

let testNotification = httpsCallable<unit, unit> (functions, "testNotification")
