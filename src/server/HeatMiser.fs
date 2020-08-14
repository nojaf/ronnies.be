namespace Ronnies.Server

open System
open Microsoft.Azure.WebJobs
open FSharp.Control.Tasks
open Microsoft.Extensions.Logging
open FSharp.Data

module HeatMiser =

    let private backend =
        Environment.GetEnvironmentVariable("BACKEND")

    let private subscriptionKey =
        Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY")

    [<FunctionName("HeatMiser")>]
    let run ([<TimerTrigger("0 */10 * * * *")>] _myTimer : TimerInfo, log : ILogger) =
        let msg =
            sprintf "F# Time trigger function executed at: %A" DateTime.Now

        log.LogInformation msg

        task {
            let url = sprintf "%s/ping" backend
            let! response = Http.AsyncRequest(url, headers = [ "Ocp-Apim-Subscription-Key", subscriptionKey ])
            log.LogInformation(sprintf "response was %i" response.StatusCode)
            ()
        }
