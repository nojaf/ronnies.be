module Overview

open Fable.Core
open React
open type React.DSL.DOMProps
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open StyledComponents
open ReactLazyLoad
open Ronnies.Shared
open ComponentsDSL

type SortOrder =
    | ByName = 0
    | ByPrice = 1
    | ByDate = 2
    | ByAuthor = 4

let StyledMain : JSX.ElementType =
    mkStyleComponent
        "main"
        """
& {
    padding-inline: 0;
}

h1 {
    padding-inline: var(--spacing-200);
}

header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--spacing-50);
    flex-wrap: wrap;
    margin-bottom: var(--spacing-400);
    padding-inline: var(--spacing-200);
}

header p {
    flex: 1;
}

> div:first-of-type {
    padding-top: 0;
}

@media screen and (min-width: 600px) {
    & {
        max-width: 600px;
        margin-inline: auto;
    }
}
"""

[<ExportDefault>]
let OverviewPage () =
    let querySnapshot, isLoading, _ = useQuery allRonniesQuery

    let sortOrder, setSortOrder =
        React.useState<SortOrder * bool> (SortOrder.ByDate, false)

    let tokenResult, isTokenLoading, _ = useAuthIdTokenResult<CustomClaims> auth
    let users, setUsers = React.useState<Map<uid, string>> Map.empty

    React.useEffect (
        fun () ->
            match tokenResult with
            | None -> ()
            | Some token ->
                if not token.claims.``member`` then
                    ()
                else

                API.getUsers {| includeCurrentUser = true |}
                |> Promise.iter (fun users ->
                    users |> Array.map (fun u -> u.uid, u.displayName) |> Map.ofArray |> setUsers
                )
        , [| isTokenLoading |]
    )

    let overview =
        if isLoading then
            None
        else

        querySnapshot
        |> Option.map (fun querySnapshot ->
            let locations =
                querySnapshot.docs
                |> Array.map (fun snapshot -> snapshot.id, snapshot.data ())
                |> fun locations ->
                    match fst sortOrder with
                    | SortOrder.ByPrice ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.price)
                            locations
                    | SortOrder.ByDate ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.date.toDate ())
                            locations
                    | _ ->
                        (if snd sortOrder then
                             Array.sortBy
                         else
                             Array.sortByDescending)
                            (fun (_, location : RonnyLocation) -> location.name.ToLower ())
                            locations
                |> Array.map (fun (id, location) ->
                    lazyLoad [ LazyLoadProp.Offset 600 ; Key id ] [
                        overviewItem [
                            OverviewItemProp.Id id
                            OverviewItemProp.Location location
                            OverviewItemProp.Users users
                        ]
                    ]
                )

            let onHeaderClick (nextSortOrder : SortOrder) =
                if nextSortOrder = fst sortOrder then
                    fun _ -> setSortOrder (nextSortOrder, not (snd sortOrder))
                else
                    fun _ -> setSortOrder (nextSortOrder, true)

            fragment [ Key "overview" ] [
                h1 [ Key "overview-h1" ] [ str "Overzicht" ]
                header [ Key "header" ] [
                    p [] [ str "Sorteer op " ]
                    button [ Key "name-header" ; OnClick (onHeaderClick SortOrder.ByName) ] [ str "Naam" ]
                    button [ Key "price-header" ; OnClick (onHeaderClick SortOrder.ByPrice) ] [ str "Prijs" ]
                    button [ Key "date-header" ; OnClick (onHeaderClick SortOrder.ByDate) ] [ str "Datum toegevoegd" ]
                ]
                yield! locations
            ]
        )

    let content =
        match overview with
        | None -> loader []
        | Some overview -> overview

    styledComponent StyledMain [ content ]
