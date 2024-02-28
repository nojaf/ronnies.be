module Components.Loader

open Fable.Core
open type React.React
open React.DSL
open type React.DSL.DOMProps
open StyledComponents

let private StyledDiv : JSX.ElementType =
    mkStyleComponent
        "div"
        """
& {
    display: flex;
    align-items: center;
    justify-content: center;
    margin-top: var(--unit-8);
}

> div {
    display: inline-block;
    width: 80px;
    height: 80px; 
}

> div:after {
    content: " ";
    display: block;
    width: 64px;
    height: 64px;
    margin: 8px;
    border-radius: 50%;
    border: 6px solid var(--ronny-600);
    border-color: var(--ronny-600) transparent var(--ronny-600) transparent;
    animation: lds-dual-ring 1.2s linear infinite;
}

@keyframes lds-dual-ring {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
}
"""

let Loader () = styleComponent StyledDiv [ div [] [] ]
