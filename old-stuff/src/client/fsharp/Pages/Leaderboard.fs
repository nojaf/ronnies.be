module Ronnies.Client.Pages.Leaderboard

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.React
open Fable.React.Props
open Feliz
open ReactToastify
open Auth0
open Fetch
open Thoth.Json
open Ronnies.Domain
open Ronnies.Client
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Styles
open Ronnies.Client.Components.Page

type private User =
    { Name : string
      Picture : string
      Score : int }

let private userDecoder =
    Decode.object
        (fun get ->
            { Name = get.Required.Field "name" Decode.string
              Picture = get.Required.Field "picture" Decode.string
              Score = 0 })

let getScores events =
    List.fold
        (fun acc ev ->
            match ev with
            | LocationAdded la ->
                let creator = NonEmptyString.Read la.Creator

                if Map.containsKey creator acc then
                    Map.find creator acc
                    |> fun v -> Map.add creator (la.Id :: v) acc
                else
                    Map.add creator [ la.Id ] acc
            | LocationCancelled id
            | LocationNoLongerSellsRonnies id -> Map.map (fun _ locations -> List.filter ((<>) id) locations) acc)
        Map.empty
        events
    |> Map.map (fun _ v -> List.length v)

let private useUserScore () =
    let auth0 = useAuth0 ()
    let userScores, setUserScores = React.useState ([])
    let eventCtx = React.useContext (eventContext)

    let scores =
        React.useMemo ((fun () -> getScores eventCtx.Events), [| eventCtx.Events |])

    React.useEffect (
        (fun () ->
            if auth0.isAuthenticated then
                auth0.getAccessTokenSilently ()
                |> Promise.bind
                    (fun token ->
                        let url = sprintf "%s/users" Common.backendUrl

                        fetch
                            url
                            [ requestHeaders [ HttpRequestHeaders.ContentType "application/json"
                                               Common.subscriptionHeader
                                               Common.authHeader token ] ])
                |> Promise.bind (fun res -> res.text ())
                |> Promise.map
                    (fun json ->
                        let result =
                            Decode.fromString (Decode.keyValuePairs userDecoder) json

                        match result with
                        | Ok users ->
                            users
                            |> List.choose
                                (fun (k, u) ->
                                    let userScore =
                                        Map.tryFind k scores |> Option.defaultValue 0

                                    if userScore > 0 then
                                        Some { u with Score = userScore }
                                    else
                                        None)
                            |> List.sortByDescending (fun u -> u.Score)
                            |> setUserScores
                        | Error err -> JS.console.error err)
                |> Promise.catchEnd
                    (fun err ->
                        JS.console.error err
                        errorToast "Kon de patrons niet ophalen")),
        [| box auth0.isAuthenticated |]
    )

    userScores

[<ReactComponent>]
let private LeaderboardPage () =
    let scores = useUserScore ()

    let userRows =
        scores
        |> List.map
            (fun us ->
                tr [ Key us.Name ] [
                    td [] [
                        img [ Src us.Picture
                              HTMLAttr.Height "30px"
                              HTMLAttr.Width "30x" ]
                    ]
                    td [] [ str us.Name ]
                    td [] [ strong [] [ ofInt us.Score ] ]
                ])

    page [] [
        h1 [] [ str "Klassement" ]
        table [ classNames [ Bootstrap.Table
                             Bootstrap.TableStriped
                             Bootstrap.TableLight ] ] [
            thead [] [
                tr [] [
                    th [] []
                    th [] [ str "Naam" ]
                    th [] [ str "Score" ]
                ]
            ]
            tbody [] [ ofList userRows ]
        ]
    ]

exportDefault LeaderboardPage
