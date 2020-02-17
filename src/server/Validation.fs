module Ronnies.Server.Validation

let private createLongitude (v: float) =
    if v >= -180.0 && v <= 180. then Some v else None

let private createLatitude (v: float) =
    if (v >= -85.0) && (v <= 85.0) then Some v else None

let createPrice (v: string) =
    let vd = v.Replace(",", ".")
    match System.Double.TryParse(vd) with
    | true, v -> Some v
    | _ -> None
