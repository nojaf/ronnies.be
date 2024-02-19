module Leaderboard

open React
open type React.DSL.DOMProps
open Iconify
open Firebase
open type Firebase.Auth.Exports
open ComponentsDSL

type HighScore =
    {|
        uid : uid
        displayName : string
        score : int
    |}

let LeaderboardPage () =
    let querySnapshot, snapShotIsLoading, _ = Hooks.Exports.useQuery allRonniesQuery
    let user, isUserLoading, _ = Hooks.Exports.useAuthState auth
    let scores, setScores = React.useState<HighScore array> Array.empty

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

            let highestScoreIcon =
                if not hasHighestScore then
                    null
                else
                    icon [
                        Key "crown"
                        IconProp.Icon "mdi:crown"
                        IconProp.Height 24
                        IconProp.Width 24
                    ]

            tr [ Key highScore.uid ] [
                td [ ClassName (if hasHighestScore then "highscore" else "") ] [
                    highestScoreIcon
                    str highScore.displayName
                ]
                td [] [ ofInt highScore.score ]
            ]
        )

    let content =
        if snapShotIsLoading || isUserLoading then
            loader [ Key "loader" ]
        else
            table [ Key "table" ] [
                thead [] [ tr [] [ th [] [ str "Naam" ] ; th [] [ str "Score" ] ] ]
                tbody [] rows
            ]

    main [] [ h1 [ Key "title" ] [ str "Klassement" ] ; content ]
