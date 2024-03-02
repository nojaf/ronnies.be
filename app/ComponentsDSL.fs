module ComponentsDSL

open Fable.Core
open React.Plugin

#nowarn "1182"

[<JSX(nameof Components.Loader.Loader, "Components/Loader.fs")>]
let loader (props : JSX.Prop seq) : JSX.Element = null

[<RequireQualifiedAccess>]
type ToggleProp =
    [<Emit "trueLabel">]
    static member TrueLabel (value : string) : JSX.Prop = "trueLabel", box value

    [<Emit "falseLabel">]
    static member FalseLabel (value : string) : JSX.Prop = "falseLabel", box value

    [<Emit "onChange">]
    static member OnChange (value : bool -> unit) : JSX.Prop = "onChange", box value

    [<Emit "value">]
    static member Value (value : bool) : JSX.Prop = "value", box value

    [<Emit "disabled">]
    static member Disabled (value : bool) : JSX.Prop = "disabled", box value

[<JSX(nameof Components.Toggle.Toggle, "Components/Toggle.fs")>]
let toggle (props : JSX.Prop seq) : JSX.Element = null

[<RequireQualifiedAccess>]
type LocationPickerProp =
    [<Emit "onChange">]
    static member OnChange (value : LatLng * LatLng -> unit) : JSX.Prop = "onChange", box value

    [<Emit "existingLocations">]
    static member ExistingLocations (value : (string * LatLng) array) : JSX.Prop = "existingLocations", box value

[<JSX(nameof Components.LocationPicker.LocationPicker, "Components/LocationPicker.fs")>]
let locationPicker (props : JSX.Prop seq) : JSX.Element = null

[<JSX(nameof Components.Navigation, "Components/Navigation.fs")>]
let navigation (props : JSX.Prop seq) : JSX.Element = null
