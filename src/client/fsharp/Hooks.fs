module Ronnies.Client.Hooks

open System
open Browser.Types
open Fable.Core
open Fable.React
open Ronnies.Client.Model
open Ronnies.Client.View
open Ronnies.Shared

let private f g = System.Func<_, _>(g)

let private useModel() =
    let { Model = model } = Hooks.useContext (appContext)
    model

let private useEvents() =
    let { Events = events } = useModel()
    events

let private useDispatch() =
    let { Dispatch = dispatch } = Hooks.useContext (appContext)
    dispatch

let useSetToken() =
    let dispatch = useDispatch()
    f (fun token -> SetToken token |> dispatch)

let useDump() =
    let model = useModel()
    JS.JSON.stringify (model, space = 4)

let useRole() =
    let model = useModel()
    model.Role

let useTotalRonnyCount() =
    let events = useEvents()
    Projections.getTotal events

let private deg2rad deg = deg * Math.PI / 180.0
let private rad2deg rad = (rad / Math.PI * 180.0)

let private distanceBetweenTwoPoints (latA, lngA) (latB, lngB) =
    if latA = latB && lngA = lngB then
        0.
    else
        let theta = lngA - lngB

        let dist =
            Math.Sin(deg2rad (latA)) * Math.Sin(deg2rad (latB))
            + Math.Cos(deg2rad (latA)) * Math.Cos(deg2rad (latB)) * Math.Cos(deg2rad (theta))
            |> Math.Acos
            |> rad2deg
            |> (*) (60. * 1.1515 * 1.609344)
        dist

[<Global("navigator.geolocation")>]
let private geolocation: Browser.Types.Geolocation option = jsNative

let useUserLocation() =
    let error = Hooks.useState<string option> (None)
    let location = Hooks.useState<Ronnies.Shared.Location> ((0., 0.))

    let onLocationChange (event: Position) =
        let roundedLocation = Math.Round(event.coords.latitude, 5), Math.Round(event.coords.longitude, 5)
        if roundedLocation <> location.current then
            location.update roundedLocation

    let onError (event: PositionError) = error.update (Some event.message)

    Hooks.useEffectDisposable
        ((fun () ->
            let mutable watchId: float = 0.
            match geolocation with
            | Some geo ->
                watchId <- geo.watchPosition (onLocationChange, onError)
                { new System.IDisposable with
                    member this.Dispose() =
                        if watchId <> 0. then geo.clearWatch (watchId) }
            | None ->
                error.update (Some "Geolocation is not supported")
                { new System.IDisposable with
                    member this.Dispose() = () }), Array.empty)

    {| errors = error.current
       location = location.current |}


let useRonniesNearUserLocation() =
    let events = useEvents()
    let radius = 0.5 //0.250
    f (fun userLocation ->
        let nearbyRonnies = Hooks.useState ([||])
        let ronniesWithLocation = Projections.getRonniesWithLocation events

        Hooks.useEffect
            ((fun () ->
                    ronniesWithLocation
                    |> List.filter
                        (snd >> (fun ronnyLocation -> distanceBetweenTwoPoints userLocation ronnyLocation <= radius))
                    |> List.toArray
                    |> Array.map (fun (name, location) ->
                        {| name = name
                           lat = fst location
                           lng = snd location |})
                    |> nearbyRonnies.update), [| userLocation |])

        nearbyRonnies.current)

let useRonniesList() =
    let events = useEvents()
    Projections.getRonnies (fun la ->
        {| id = la.Id
           name = la.Name
           date = la.Date.ToString("dd/MM/yyyy") |}) events
    |> List.toArray
    |> Array.sortBy (fun p -> p.name.ToLower())

let useAddLocationEvent() =
    let dispatch = useDispatch()
    let { UserId = userId } = useModel()
    f (fun (addLocation: {| name: string; location: Ronnies.Shared.Location; price: float; isDraft: bool; remark: string option |}) ->
            Event.LocationAdded
                ({ Id = Ronnies.Shared.newId()
                   Name = addLocation.name
                   Location = addLocation.location
                   Price = addLocation.price
                   IsDraft = addLocation.isDraft
                   Remark = addLocation.remark
                   Creator = Option.defaultValue "" userId
                   Date = System.DateTime.Now })
            |> AddLocation
            |> dispatch)

let isFurtherAwayThen =
    System.Func<_, _, _, _>(fun km a b ->
    distanceBetweenTwoPoints a b  > km)

let useAppLoading() =
    let { IsLoading = isLoading } = useModel()
    isLoading

let useAppException() =
    let { AppException = ae } = useModel()
    ae