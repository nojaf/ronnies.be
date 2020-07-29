module Ronnies.Client.Components.Switch

open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Styles

[<NoComparison>]
[<NoEquality>]
type SwitchProps =
    { TrueLabel : string
      FalseLabel : string
      OnChange : bool -> unit
      Value : bool
      Disabled : bool }

let Switch =
    React.functionComponent
        ("Switch",
         (fun (props : SwitchProps) ->
             div [ classNames [ Bootstrap.BtnGroup
                                Bootstrap.My2 ] ] [
                 button [ classNames [ Bootstrap.Btn
                                       if props.Value then
                                           Bootstrap.BtnPrimary
                                       else
                                           Bootstrap.BtnOutlinePrimary ]
                          Disabled props.Disabled
                          OnClick(fun ev ->
                              ev.preventDefault ()
                              props.OnChange true) ] [
                     str props.TrueLabel
                 ]
                 button [ classNames [ Bootstrap.Btn
                                       if not props.Value then
                                           Bootstrap.BtnPrimary
                                       else
                                           Bootstrap.BtnOutlinePrimary ]
                          Disabled props.Disabled
                          OnClick(fun ev ->
                              ev.preventDefault ()
                              props.OnChange false) ] [
                     str props.FalseLabel
                 ]
             ]))
