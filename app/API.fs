[<RequireQualifiedAccess>]
module API

open Fable.Core
open type Firebase.Functions.Exports

type User = {| displayName : string ; uid : uid |}

let private users = httpsCallable<unit, User array> (functions, "users")

let getUsers () : JS.Promise<User array> =
    users () |> Promise.map (fun result -> result.data)

type AddUser =
    {|
        displayName : string
        email : string
    |}

let private user = httpsCallable<AddUser, obj> (functions, "user")

let addUser name email : JS.Promise<obj> =
    user {| displayName = name ; email = email |}
    |> Promise.map (fun result -> result.data)
