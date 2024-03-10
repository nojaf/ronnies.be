module Leaderboard

open Fable.Core
open React
open type React.DSL.DOMProps
open Iconify
open Firebase
open type Firebase.Auth.Exports
open StyledComponents
open ComponentsDSL

type HighScore =
    {|
        uid : uid
        displayName : string
        score : int
    |}

let StyledMain : JSX.ElementType =
    mkStyleComponent
        "main"
        """
table {
    margin-top: var(--spacing-400);
    width: 100%;
    border-collapse: collapse;
}

table th {
    text-align: left;
}

table th:last-of-type, table tr td:last-of-type {
    text-align: center;
}

table th, table td {
    display: table-cell;
    border-top: 1px solid var(--grey);
    box-sizing: border-box;
    border-collapse: collapse;
}

table th {
    color: var(--ronny-600);
    cursor: pointer;
}

table th, table td {
    padding: var(--spacing-400);
}

table tbody tr:nth-child(2n) {
    background-color:var(--ronny-50);
}

.highscore {
    display: flex;
    align-items: center;
}

.highscore svg {
    margin-right: var(--spacing-100);
    color: #F6CF57;
}
"""

[<ExportDefault>]
let LeaderboardPage () =
    let querySnapshot, snapShotIsLoading, _ = Hooks.Exports.useQuery allRonniesQuery
    let scores, setScores = React.useState<HighScore array> Array.empty

    React.useEffect (
        fun () ->
            match querySnapshot with
            | Some querySnapshot ->
                API.getUsers {| includeCurrentUser = true |}
                |> Promise.map (fun users ->
                    let userMap = users |> Array.map (fun u -> u.uid, u.displayName) |> Map.ofArray

                    querySnapshot.docs
                    |> Array.collect (fun snapshot ->
                        let ronnyLocation = snapshot.data ()
                        [| yield ronnyLocation.userId ; yield! ronnyLocation.otherUserIds |]
                    )
                    |> Array.groupBy id
                    |> Array.choose (fun (uid, locations) ->
                        Map.tryFind uid userMap
                        |> Option.map (fun userName ->
                            {|
                                uid = uid
                                displayName = userName
                                score = locations.Length
                            |}
                        )
                    )
                    |> Array.sortByDescending (fun score -> score.score)

                )
                |> Promise.iter setScores
            | _ -> ()
        , [| snapShotIsLoading |]
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
        if snapShotIsLoading then
            loader [ Key "loader" ]
        else
            table [ Key "table" ] [
                thead [] [ tr [] [ th [] [ str "Naam" ] ; th [] [ str "Score" ] ] ]
                tbody [] rows
            ]

    styledComponent StyledMain [ h1 [ Key "title" ] [ str "Klassement" ] ; content ]
