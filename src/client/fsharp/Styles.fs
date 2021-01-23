module Ronnies.Client.Styles

open Zanaptak.TypedCssClasses

[<Literal>]
let private ResolutionFolder = __SOURCE_DIRECTORY__ + "/.."

type Bootstrap = CssClasses<"src/style.sass", Naming.PascalCase, commandFile="yarn", argumentPrefix = "sass", resolutionFolder = ResolutionFolder>

let classNames names =
    String.concat " " names
    |> Fable.React.Props.ClassName
