module Components

open Fable.Core
open React
open type React.DSL.DOMProps
open StyledComponents
open ReactRouterDom
open UseHooksTs
open Iconify
open type Firebase.Auth.Exports
open type Firebase.Hooks.Exports

let StyledNav : JSX.ElementType =
    mkStyleComponent
        "nav"
        """
& {
    background-color: var(--ronny-500);
    color: white;
    padding-top: var(--spacing-300);
    display: grid;
    grid-template-rows: auto auto;
    grid-template-columns: auto auto;
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    z-index: 10;
}

> a {
    margin-left: var(--spacing-300);
    margin-bottom: var(--spacing-300);
    grid-row: 1;
    grid-column: 1;
    justify-self: start;
}

> a img {
    display: block;
    height: var(--spacing-400);
}

a:hover img, nav button:hover svg {
    background: rgba(255, 255, 255, .25);
    border-radius: var(--radius);
}

button {
    cursor: pointer;
    color: var(--white);
    background-color: transparent;
    border: none;
    padding: 0;
    max-height: var(--spacing-400);
    grid-row: 1;
    grid-column: 2;
    justify-self: end;
    margin-right: var(--spacing-200);
    margin-bottom: var(--spacing-200);
}

button:hover {
    color: var(--grey);
    background: transparent;
}

@keyframes menu-open {
    from {
        opacity: 0;
        visibility: collapse;
        height: 0;
        border-top: none;
    }
    to {
        opacity: 1;
        visibility: visible;
        height: initial;
        border-top: 1px solid var(--white);
    }
}

ul {
    grid-row: 2;
    grid-column: 1 / span 2;
    padding: 0;
    margin: 0;
    list-style: none;
    opacity: 0;
    visibility: collapse;
    height: 0;
    box-sizing: content-box;
}

ul.show {
    animation: menu-open 0.2s ease-in forwards;
}

ul li a {
    display: block;
    padding: var(--spacing-200);
    border-bottom: 1px solid var(--grey-border);
    background-color: var(--ronny-600);
    color: white;
    text-decoration: none;
}

ul #user a {
    display: flex;
    align-items: center;
    padding: var(--spacing-200);
    font-weight: 500;
}

ul #user a:hover {
    text-decoration: none;
}

ul #user a svg {
    margin-right: var(--spacing-50);
}

ul li a:hover {
    background-color: var(--ronny-400);
    cursor: pointer;
}

ul li a:active, nav ul li a.active {
    background-color: var(--ronny-700);
    font-weight: 500;
}

@media screen and (min-width: 960px) {
    button {
        display: none;
    }

    ul {
        grid-row: 1;
        grid-column: 2;
        justify-self: end;
        display: flex;
        justify-content: flex-end;
        align-items: center;
        height: var(--spacing-400);
        animation: initial;
        opacity: 1;
        visibility: visible;
        border: none;
    }

    ul li a, nav ul #user {
        background-color: transparent;
        border: none;
        margin-right: var(--spacing-400);
        padding: 0;
    }

    ul li a:hover {
        background-color: transparent;
        text-decoration: underline;
    }

    ul li a:focus {
        text-decoration: none;
    }
}
"""

let LogoutComponent () =
    let navigate = useNavigate ()

    let logoutHandler (ev : Browser.Types.Event) =
        ev.preventDefault ()
        signOut auth |> Promise.iter (fun () -> navigate "/")

    a [ Href "#" ; OnClick logoutHandler ] [ str "uitloggen" ]

[<ExportDefault>]
let Navigation () : JSX.Element =
    let isTablet = useMediaQuery "screen and (min-width: 960px)"
    let isMenuOpen, setIsMenuOpen = React.useState false
    let user, _, _ = useAuthState auth
    let tokenResult, _, _ = useAuthIdTokenResult<CustomClaims> auth

    let menuClass = if isMenuOpen then "show" else ""

    let mkNavLinkAux too id content =
        li [
            Key too
            OnClick (fun _ -> setIsMenuOpen false)
            Id (if System.String.IsNullOrWhiteSpace id then null else id)
        ] [ navLink [ ReactRouterProp.To too ; Key too ] [ content ] ]

    let mkNavLink too text = mkNavLinkAux too "" (str text)

    styleComponent StyledNav [
        link [ ReactRouterProp.To "/" ; OnClick (fun _ -> setIsMenuOpen false) ] [ img [ Src "/images/r-white.png" ] ]
        button [
            OnClick (fun ev ->
                ev.preventDefault ()
                setIsMenuOpen (not isMenuOpen)
            )
        ] [
            icon [
                Key "mobile-menu-icon"
                IconProp.Icon "ic:baseline-menu"
                IconProp.Width 24
                IconProp.Height 24
            ]
        ]
        ul [ ClassName menuClass ] [
            yield mkNavLink "/overview" "Overzicht"
            yield mkNavLink "/legacy" "De vorige keer"

            match user with
            | None -> yield mkNavLink "/login" "Inloggen"
            | Some user ->
                yield mkNavLink "/add-location" "E nieuwen toevoegen"
                yield mkNavLink "/leaderboard" "Klassement"
                yield mkNavLink "/rules" "Manifesto"

                match tokenResult with
                | Some tokenResult when tokenResult.claims.admin -> yield mkNavLink "/admin" "Admin"
                | _ -> ()

                yield li [ Key "logout" ; OnClick (fun _ -> setIsMenuOpen false) ] [ JSX.create LogoutComponent [] ]

                yield
                    mkNavLinkAux
                        "/settings"
                        "user"
                        (fragment [] [
                            icon [ IconProp.Icon "clarity:user-line" ; IconProp.Height 24 ; IconProp.Width 24 ]
                            str user.displayName
                        ])
        ]
    ]
