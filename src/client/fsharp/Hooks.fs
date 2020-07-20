module Ronnies.Client.Hooks

open System
open Fable.Core
open Fable.React
open Ronnies.Client.Model
open Ronnies.Client.View
open Ronnies.Shared

let private f g = System.Func<_, _>(g)

let private useModel () =
    let { Model = model } = Hooks.useContext (appContext)
    model

let private useEvents () =
    let { Events = events } = useModel ()
    events

let private useDispatch () =
    let { Dispatch = dispatch } = Hooks.useContext (appContext)
    dispatch

let useSetToken () =
    let dispatch = useDispatch ()
    f (fun token -> SetToken token |> dispatch)

let useDump () =
    let model = useModel ()
    JS.JSON.stringify (model, space = 4)

let useRole () =
    let model = useModel ()
    model.Role

let useTotalRonnyCount () =
    let events = useEvents ()
    Projections.getTotal events

let private deg2rad deg = deg * Math.PI / 180.0
let private rad2deg rad = (rad / Math.PI * 180.0)

let private distanceBetweenTwoPoints (latA, lngA) (latB, lngB) =
    if latA = latB && lngA = lngB then
        0.
    else
        let theta = lngA - lngB

        let dist =
            Math.Sin(deg2rad (latA))
            * Math.Sin(deg2rad (latB))
            + Math.Cos(deg2rad (latA))
              * Math.Cos(deg2rad (latB))
              * Math.Cos(deg2rad (theta))
            |> Math.Acos
            |> rad2deg
            |> (*) (60. * 1.1515 * 1.609344)

        dist

[<Global("navigator.geolocation")>]
let private geolocation : Browser.Types.Geolocation = jsNative

open Fable.Core.JsInterop

let useGeolocation ()
                   : {| loading : bool
                        latitude : float
                        longitude : float
                        error : obj |} =
    import "useGeolocation" "react-use"

let isDefaultLocation (a : Ronnies.Shared.Location) = a = (0., 0.)

let useRonniesNearUserLocation () =
    let events = useEvents ()
    let radius = 0.5 //0.250
    let nearbyRonnies = Hooks.useState ([||])

    let update userLocation =
        let ronniesWithLocation =
            Projections.getRonniesWithLocation events

        ronniesWithLocation
        |> List.filter
            (snd
             >> (fun ronnyLocation ->
                 distanceBetweenTwoPoints userLocation ronnyLocation
                 <= radius))
        |> List.toArray
        |> Array.map (fun (name, location) ->
            {| name = name
               lat = fst location
               lng = snd location |})
        |> nearbyRonnies.update

    (nearbyRonnies.current, update)

let useRonniesList () =
    let events = useEvents ()
    Projections.getRonnies (fun la ->
        {| id = la.Id
           name = la.Name
           date = la.Date.ToString("dd/MM/yyyy") |}) events
    |> List.toArray
    |> Array.sortBy (fun p -> p.name.ToLower())

[<Emit("parseFloat(parseFloat($0).toFixed(2))")>]
let parseAndTrim (_ : string) : float = jsNative

let useAddLocationEvent () =
    let dispatch = useDispatch ()
    let { UserId = userId } = useModel ()
    f (fun (addLocation : {| name : string
                             location : Ronnies.Shared.Location
                             price : string
                             isDraft : bool
                             remark : string option |}) ->
            let addLocation =
                { Id = Ronnies.Shared.newId ()
                  Name = addLocation.name
                  Location = addLocation.location
                  Price = parseAndTrim addLocation.price
                  IsDraft = addLocation.isDraft
                  Remark = addLocation.remark
                  Creator = Option.defaultValue "" userId
                  Date = System.DateTime.Now }

            Event.LocationAdded addLocation
            |> AddLocation
            |> dispatch)

let isFurtherAwayThen =
    System.Func<_, _, _, _>(fun km a b -> distanceBetweenTwoPoints a b > km)

let useAppLoading () =
    let { IsLoading = isLoading } = useModel ()
    isLoading

let useAppException () =
    let { AppException = ae } = useModel ()
    ae
