module Components.OverviewItem

open System
open Fable.Core
open Browser.Types
open Firebase
open type React.React
open React.DSL
open type React.DSL.DOMProps
open StyledComponents
open ReactRouterDom
open Iconify

[<Import("formatDistanceToNow", "date-fns")>]
let formatDistanceToNow (_date : DateTime, _options : {| addSuffix : bool |}) : string = jsNative

type OverviewItemProps =
    {|
        id : string
        location : RonnyLocation
        users : Map<uid, string>
    |}

let StyledElement : JSX.ElementType =
    mkStyleComponent
        "div"
        """
& {
    padding-block: var(--spacing-300);
}

@media screen and (min-width: 600px) {
    & {
        padding-block: var(--spacing-500);
    }
}

strong {
    display: inline-block;
    color: var(--dark);
    padding-inline: var(--spacing-200);
    font-size: var(--font-100);
}

.image-container {
    position: relative;
    
    img {
        width: 100%;
        max-width: 600px;
        margin-block: var(--spacing-200);
        display: block;
    }
    
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

.details {
    display: flex;
    align-items: center;
    padding-inline: var(--spacing-200);
    max-width: 600px;
    margin-top: var(--spacing-200);
}

a {
    flex: 1;
    display: flex;
    color: var(--ronny-500);
    align-items: center;
    
    > svg {
        margin-left: var(--spacing-100);
    }
}

.end {
    display: flex;
    align-items: center;
    
    strong {
        padding-block: 0;
        font-weight: 300;
        font-size: var(--font-200);
    }
}

p {
    padding-inline: var(--spacing-200);
}

.others {
    font-size: var(--font-50);
    font-style: italic;
}

.others strong {
    padding-inline: var(--spacing-50);
    font-size: var(--font-50);
    font-style: normal;
}

time {
    color: var(--ronny-900);
    padding-inline: var(--spacing-200);
    margin-top: var(--spacing-200);
    display: block;
}
"""

[<ExportDefault>]
let OverviewItem (props : OverviewItemProps) : JSX.Element =
    let location = props.location
    let images, setImages = useState<Map<string, string>> Map.empty

    useEffect (
        fun () ->
            props.location.photoNames
            |> Array.map (fun photoName ->
                let storageRef = Storage.Exports.ref (storage, $"locations/{photoName}")

                Storage.Exports.getDownloadURL storageRef
                |> Promise.map (fun url -> Some (photoName, url))
                |> Promise.catch (fun _ -> None)
            )
            |> Promise.all
            |> Promise.iter (Array.choose id >> Map.ofArray >> setImages)

        , [| props.location.photoNames.Length |]
    )

    let priceText =
        if location.currency = "EUR" then $"€%.2f{location.price}"
        elif location.currency = "USD" then $"$%.2f{location.price}"
        elif location.currency = "GBP" then $"£%.2f{location.price}"
        else $"{location.price} {location.currency}"

    let by =
        if Map.isEmpty props.users then
            null
        else
            strong [ Key $"%s{props.id}-by" ] [
                match Map.tryFind location.userId props.users with
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
            div [ Key $"%s{props.id}-image" ; ClassName "image-container" ] [
                img [
                    Loading "lazy"
                    Src imageUrl
                    OnLoad (fun ev -> ev.currentTarget :?> HTMLElement |> fun e -> e.classList.add "loaded")
                ]
                JSX.create Loader.Loader []
            ]

    let alsoPresent =
        if Map.isEmpty props.users || Array.isEmpty location.otherUserIds then
            null
        else
            let others =
                location.otherUserIds
                |> Array.choose (fun otherId -> Map.tryFind otherId props.users)
                |> String.concat ", "
                |> fun text -> strong [] [ str text ]

            p [ ClassName "others" ] [ str "Ook present " ; others ]

    let description =
        if String.IsNullOrWhiteSpace location.remark then
            null
        else
            p [] [ str location.remark ]

    styledComponent StyledElement [
        by
        photo
        div [ Key $"%s{props.id}-details" ; ClassName "details" ] [
            link [ Key $"%s{props.id}-link" ; ReactRouterProp.To $"/detail/{props.id}" ] [
                str location.name
                icon [ IconProp.Icon "ion:open-outline" ]
            ]
            div [ ClassName "end" ] [
                strong [ Key $"%s{props.id}-price" ] [ str $"%s{priceText}" ]
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
        alsoPresent
        description
        time [ Key $"%s{props.id}-date" ] [
            str (formatDistanceToNow ((location.date.toDate ()), {| addSuffix = true |}))
        ]
    ]
