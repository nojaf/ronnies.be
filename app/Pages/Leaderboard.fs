module Leaderboard

open Fable.Core.JsInterop
open Fable.Core
open Browser
open Browser.Types
open Feliz
open React
open React.Props
open Iconify
open Firebase
open type Firebase.Auth.Exports
open ReactRouterDom
open Components

type HighScore =
    {|
        uid : uid
        displayName : string
        score : int
    |}

[<ReactComponent>]
let LeaderboardPage () =
    let querySnapshot, snapShotIsLoading, _ = Hooks.Exports.useQuery allRonniesQuery
    let user, isUserLoading, _ = Hooks.Exports.useAuthState auth
    let scores, setScores = React.useState<HighScore array> (Array.empty)

    React.useEffect (
        fun () ->
            match querySnapshot, user with
            | Some querySnapshot, Some user ->
                API.getUsers ()
                |> Promise.map (fun users ->
                    let userMap =
                        [|
                            yield (user.uid, user.displayName)
                            for otherUser in users do
                                yield (otherUser.uid, otherUser.displayName)
                        |]
                        |> Map.ofArray

                    querySnapshot.docs
                    |> Array.collect (fun snapshot ->
                        let ronnyLocation = snapshot.data ()
                        [| yield ronnyLocation.userId ; yield! ronnyLocation.otherUserIds |]
                    )
                    |> Array.groupBy id
                    |> Array.map (fun (uid, locations) ->
                        {|
                            uid = uid
                            displayName = Map.find uid userMap
                            score = locations.Length
                        |}
                    )
                    |> Array.sortByDescending (fun score -> score.score)

                )
                |> Promise.iter setScores
            | _ -> ()
        , [| box querySnapshot ; box user |]
    )

    let rows =
        let highestScore = if scores.Length = 0 then 0 else scores.[0].score

        scores
        |> Array.map (fun highScore ->
            let hasHighestScore = highScore.score = highestScore

            tr [ Key highScore.uid ] [
                td [ ClassName (if hasHighestScore then "highscore" else "") ] [
                    if hasHighestScore then
                        Icon [ IconProp.Icon "mdi:crown" ; IconProp.Height 24 ; IconProp.Width 24 ]
                    str highScore.displayName
                ]
                td [] [ ofInt highScore.score ]
            ]
        )

    main [] [
        h1 [] [ str "Klassement" ]
        if snapShotIsLoading || isUserLoading then
            Loader ()
        else
            table [] [
                thead [] [ tr [] [ th [] [ str "Naam" ] ; th [] [ str "Score" ] ] ]
                tbody [] [ ofArray rows ]
            ]
    ]
