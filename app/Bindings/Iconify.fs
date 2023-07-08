module Iconify

open React

#nowarn "1182"

[<RequireQualifiedAccess>]
type IconProp =
    | Icon of string
    | Width of int
    | Height of int

    interface IProp

let inline Icon (props : IProp seq) =
    ofImportWithoutChildren "Icon" "@iconify/react" props
