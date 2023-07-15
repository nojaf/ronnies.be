[<RequireQualifiedAccess>]
module API

open Fable.Core
open type Firebase.Functions.Exports

type User = {| displayName : string ; uid : uid |}

let private users = httpsCallable<unit, User array> (functions, "users")

let getUsers () : JS.Promise<User array> =
    users () |> Promise.map (fun result -> result.data)
