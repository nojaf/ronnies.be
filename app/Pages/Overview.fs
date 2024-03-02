module Overview

open System
open Browser.Types
open Fable.Core
open React
open type React.DSL.DOMProps
open Firebase
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports
open ReactRouterDom
open Iconify
open StyledComponents
open ComponentsDSL

[<Import("formatDistanceToNow", "date-fns")>]
let formatDistanceToNow (date : DateTime, options : {| addSuffix : bool |}) : string = jsNative

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
    justify-content: space-around;
    margin-bottom: var(--spacing-400);
    padding-inline: var(--spacing-200);
}

> div {
    padding-block: var(--spacing-300);
}

> div:first-of-type {
    padding-top: 0;
}

> div strong {
    display: inline-block;
    color: var(--dark);
    padding-inline: var(--spacing-200);
    font-size: var(--font-100);
}

> div .image-container {
    position: relative;
    
    > div {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
    }
    
    &:has(.loaded) {
        > div {
            display: none;
        }
    }
    
    img {
        opacity: 0;
        visibility: hidden;
        transition: opacity 200ms;

        &.loaded {
            opacity: 1;
            visibility: visible;
        }
    }
}

> div img {
    width: 100%;
    max-width: 600px;
    margin-block: var(--spacing-100);
    display: block;
}

@media screen and (min-width: 600px) {
    & {
        max-width: 600px;
        margin-inline: auto;
    }
    
    > div {
        padding-block: var(--spacing-500);
    }
}

> div .details {
    display: flex;
    align-items: center;
    padding-inline: var(--spacing-200);
    max-width: 600px;
    margin-top: var(--spacing-200);
}

> div a {
    flex: 1;
    display: flex;
    color: var(--ronny-500);
    align-items: center;
    
    > svg {
        margin-left: var(--spacing-100);
    }
}

> div .end {
    display: flex;
    align-items: center;
    
    strong {
        padding-block: 0;
        font-weight: 300;
        font-size: var(--font-200);
    }
}

time {
    color: var(--ronny-900);
    padding-inline: var(--spacing-200);
    margin-top: var(--spacing-200);
    display: block;
}
"""

[<ExportDefault>]
let OverviewPage () =
    let querySnapshot, isLoading, _ = useQuery allRonniesQuery

    let sortOrder, setSortOrder =
        React.useState<SortOrder * bool> (SortOrder.ByDate, false)

    let tokenResult, isTokenLoading, _ = useAuthIdTokenResult<CustomClaims> auth
    let users, setUsers = React.useState<Map<uid, string>> Map.empty
    // Map of photoName, downloadUrl
    let images, setImages = React.useState<Map<string, string>> Map.empty

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

    React.useEffect (
        fun () ->
            match querySnapshot with
            | None -> ()
            | Some querySnapshot ->
                querySnapshot.docs
                |> Array.choose (fun d ->
                    let data = d.data ()

                    if Array.isEmpty data.photoNames then
                        None
                    else

                    let photoName = data.photoNames.[0]
                    let storageRef = Storage.Exports.ref (storage, $"locations/{photoName}")

                    Some (
                        Storage.Exports.getDownloadURL storageRef
                        |> Promise.map (fun url -> Some (photoName, url))
                        |> Promise.catch (fun _ -> None)
                    )
                )
                |> Promise.all
                |> Promise.iter (Array.choose id >> Map.ofArray >> setImages)
        , [| isLoading |]
    )

    let overview =
        if isLoading then
            None
        else

        let showByColumn =
            not isTokenLoading
            && (
                match tokenResult with
                | None -> false
                | Some token -> token.claims.``member``
            )

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
                    let priceText =
                        if location.currency = "EUR" then $"€%.2f{location.price}"
                        elif location.currency = "USD" then $"$%.2f{location.price}"
                        elif location.currency = "GBP" then $"£%.2f{location.price}"
                        else $"{location.price} {location.currency}"

                    let by =
                        if not showByColumn then
                            null
                        else
                            strong [ Key $"%s{id}-by" ] [
                                match Map.tryFind location.userId users with
                                | None -> str "???"
                                | Some userName -> str userName
                            ]

                    let photo =
                        if Array.isEmpty location.photoNames then
                            null
                        else

                        match Map.tryFind location.photoNames.[0] images with
                        | None -> null
                        | Some imageUrl ->
                            div [ Key $"%s{id}-image" ; ClassName "image-container" ] [
                                img [
                                    Loading "lazy"
                                    Src imageUrl
                                    OnLoad (fun ev ->
                                        ev.currentTarget :?> HTMLElement |> fun e -> e.classList.add "loaded"
                                    )
                                ]
                                loader []
                            ]

                    div [ Key id ] [
                        by
                        photo
                        div [ Key $"%s{id}-details" ; ClassName "details" ] [
                            link [ Key $"%s{id}-link" ; ReactRouterProp.To $"/detail/{id}" ] [
                                str location.name
                                icon [ IconProp.Icon "ion:open-outline" ]
                            ]
                            div [ ClassName "end" ] [
                                strong [ Key $"%s{id}-price" ] [ str $"%s{priceText}" ]
                                icon [
                                    IconProp.Height 24
                                    IconProp.Width 24
                                    IconProp.Icon (
                                        if location.isDraft then
                                            "game-icons:kitchen-tap"
                                        else
                                            "ph:beer-bottle-fill"
                                    )
                                ]
                            ]
                        ]
                        time [ Key $"%s{id}-date" ] [
                            str (formatDistanceToNow ((location.date.toDate ()), {| addSuffix = true |}))
                        ]
                    ]
                )

            let onHeaderClick (nextSortOrder : SortOrder) =
                if nextSortOrder = fst sortOrder then
                    fun _ -> setSortOrder (nextSortOrder, not (snd sortOrder))
                else
                    fun _ -> setSortOrder (nextSortOrder, true)

            fragment [ Key "overview" ] [
                h1 [] [ str "Overzicht" ]
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

    styleComponent StyledMain [ content ]
