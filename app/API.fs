[<RequireQualifiedAccess>]
module API

open Fable.Core
open Fable.Core.JsInterop

let private functionsBase = import "functionsBase" "../firebase.config.js"

type User = {| displayName : string ; uid : uid |}

let getUsers (uid : string) : JS.Promise<User array> =
    Fetch.fetchUnsafe $"%s{functionsBase}/users/%s{uid}" []
    |> Promise.bind (fun res -> res.json<User array> ())
