module Components.Toast

open Fable.Core
open type React.React
open React.DSL
open type React.DSL.DOMProps
open ReactRouterDom
open StyledComponents

type ToastProps =
    {|
        url : string
        title : string
        body : string
        onClick: unit -> unit
    |}

let StyledElement : JSX.ElementType =
    mkStyleComponent
        "div"
        """
& {
    color: var(--ronny-400);
    background-color: var(--white);
    position: fixed;
    bottom: var(--spacing-500);
    right: var(--spacing-300);
    padding: var(--spacing-100) var(--spacing-200);
    border: 1px solid var(--ronny-300);
    cursor: pointer;
    transition: 300ms all;
    z-index: 1000;
    box-shadow: 0 7px 7px -7px #00000040;
}

&:hover {
    background-color: var(--ronny-300);
    color: var(--white);
}

img {
    width: 30px;
    display: inline-block;
    vertical-align: center;
}

strong {
    display: flex;
    align-items: center;
    font-weight: 500;
}

p {
    padding-left: var(--spacing-100);
    margin-block: var(--spacing-100);
    color: var(--dark);
    font-size: var(--font-100);
}
"""

[<ExportDefault>]
let Toast (props : ToastProps) : JSX.Element =
    let navigate = useNavigate ()
    let onClick _ =
        navigate props.url
        props.onClick ()

    JSX.create StyledElement [
        "onClick", box onClick
        "children",
        [
            strong [] [ img [ Src "/images/ronny.png" ] ; str props.title ]
            p [] [ str props.body ]
        ]
    ]
