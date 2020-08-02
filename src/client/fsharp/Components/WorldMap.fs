module Ronnies.Client.Components.WorldMap

open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Domain
open Ronnies.Client.Styles
open Ronnies.Client.Components.ReactMapGL
open Ronnies.Client.Components.EventContext
open Ronnies.Client.Components.Navigation

type private RonnyLocation =
    { id : string
      name : string
      latitude : float
      longitude : float }

let private getLocations (events : Event list) =
    List.fold (fun acc event ->
        match event with
        | LocationAdded locationAdded ->
            let id, name =
                (Identifier.Read locationAdded.Id).ToString(), NonEmptyString.Read locationAdded.Name

            let lat, lng = Location.Read locationAdded.Location
            { id = id
              name = name
              latitude = lat
              longitude = lng }
            :: acc

        | LocationCancelled id
        | LocationNoLongerSellsRonnies id ->
            let idAsString = (Identifier.Read id).ToString()
            List.filter (fun rl -> rl.id <> idAsString) acc) [] events

let WorldMap =
    React.functionComponent
        ("WordMap",
         (fun () ->
             let geolocation = useGeolocation ()

             let (viewport, setViewport) =
                 React.useState
                     ({ width = "100%"
                        height = "100%"
                        latitude = 50.946143
                        longitude = 3.138635
                        zoom = 12 })

             React.useEffect
                 ((fun () ->
                     if not geolocation.loading
                        && Option.isNone geolocation.error then
                         setViewport
                             ({ viewport with
                                    latitude = geolocation.latitude
                                    longitude = geolocation.longitude })),
                  [| box geolocation.loading |])

             let eventCtx = React.useContext (eventContext)

             let locations =
                 React.useMemo ((fun () -> getLocations eventCtx.Events), [| eventCtx.Events |])

             let icons =
                 locations
                 |> List.map (fun loc ->
                     Marker [ MarkerLatitude loc.latitude
                              MarkerLongitude loc.longitude
                              MarkerKey loc.id
                              OffsetLeft 0
                              OffsetTop 0 ] [
                         Link [ To(sprintf "/detail/%s" loc.id) ] [
                             img [ Src "/assets/ronny.png"
                                   HTMLAttr.Height "20"
                                   HTMLAttr.Width "20"
                                   ClassName Bootstrap.Pointer ]
                         ]
                     ])

             let userIcon =
                 if not geolocation.loading
                    && Option.isNone geolocation.error then
                     Marker [ MarkerKey "user"
                              MarkerLatitude geolocation.latitude
                              MarkerLongitude geolocation.longitude
                              OffsetTop 0
                              OffsetLeft 0 ] [
                         UserIcon
                     ]
                     |> Some
                 else
                     None

             div [ Id "world-map" ] [
                 ReactMapGL [ OnViewportChange setViewport
                              MapWidth viewport.width
                              MapHeight viewport.height
                              Latitude viewport.latitude
                              Longitude viewport.longitude
                              Zoom viewport.zoom
                              MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1" ] [
                     ofOption userIcon
                     ofList icons
                 ]
             ]))
