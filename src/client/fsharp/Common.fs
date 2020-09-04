[<RequireQualifiedAccess>]
module Ronnies.Client.Common

open Fable.Core
open Ronnies.Domain

[<Emit("process.env.REACT_APP_BACKEND")>]
let backendUrl : string = jsNative

[<Emit("process.env.REACT_APP_SUBSCRIPTION_KEY")>]
let private subscriptionKey : string = jsNative

let subscriptionHeader =
    Fetch.Types.HttpRequestHeaders.Custom("Ocp-Apim-Subscription-Key", box subscriptionKey)

let authHeader token =
    Fetch.Types.HttpRequestHeaders.Authorization(sprintf "Bearer %s" token)

let vapidKey =
    "BKjykdL6nZKMNQcO9viWqf6TbA_XegmhbCneNMBX4AWu5D8DD6e6KjeSMxXmUycsNPeGkYHtca-i_-eePtzQn3w"

let readCurrency price =
    let (value, currency) = Currency.Read price

    match currency with
    | "EUR" -> sprintf "€%.2f" value
    | "USD" -> sprintf "$%.2f" value
    | "GBP" -> sprintf "£%.2f" value
    | _ -> sprintf "%.2f %s" value currency
