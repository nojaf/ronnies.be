module Home

open Feliz
open React
open React.Props
open Firebase

[<ReactComponent>]
let HomePage () =
    let snapshot, _, _ = Hooks.Exports.useQuery allRonniesQuery

    main [

    ] [
        h1 [] [ str "Sie homepage" ]
        ul [] [
            match snapshot with
            | None -> ()
            | Some allLocations ->
                allLocations.docs
                |> Array.map (fun documentSnapshot ->
                    let ronnyLocation = documentSnapshot.data ()
                    li [ Key documentSnapshot.id ] [ str ronnyLocation.name ]
                )
                |> ofArray
        ]
    ]
