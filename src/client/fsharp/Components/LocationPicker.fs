module Ronnies.Client.Components.LocationPicker

open Fable.React
open Fable.React.Props
open Feliz
open Ronnies.Client.Components.ReactMapGL
open Ronnies.Client.Components.Loading

type LatLng = float * float

[<NoEquality>]
[<NoComparison>]
type LocationPickerProps =
    { OnChange : LatLng -> LatLng -> unit
      ExistingLocations : (string * LatLng) list }

let LocationPicker =
    React.functionComponent
        ("LocationPicker",
         (fun (props : LocationPickerProps) ->
             let geolocation = useGeolocation ()
             let (userLatitude, setUserLatitude) = React.useState (50.946139)
             let (userLongitude, setUserLongitude) = React.useState (3.138671)
             let (ronnyLatitude, setRonnyLatitude) = React.useState (userLatitude)
             let (ronnyLongitude, setRonnyLongitude) = React.useState (userLongitude)

             let (viewport, setViewport) =
                 React.useState
                     ({ width = "100%"
                        height = "100%"
                        latitude = userLatitude
                        longitude = userLongitude
                        zoom = 16 })

             React.useEffect
                 ((fun () ->
                     if not geolocation.loading then
                         setUserLatitude (geolocation.latitude)
                         setUserLongitude (geolocation.longitude)

                         setViewport
                             ({ width = "100%"
                                height = "100%"
                                latitude = geolocation.latitude
                                longitude = geolocation.longitude
                                zoom = 16 })

                         setRonnyLatitude (geolocation.latitude)
                         setRonnyLongitude (geolocation.longitude)

                         props.OnChange
                             (geolocation.latitude, geolocation.longitude)
                             (geolocation.latitude, geolocation.longitude)),
                  [| box geolocation.loading |])

             let onMapClick (ev : MapClickEvent) =
                 let (lat, lng) = ev.LatLng()
                 setRonnyLatitude lat
                 setRonnyLongitude lng
                 props.OnChange (userLatitude, userLongitude) (lat, lng)

             let existingRonnies =
                 props.ExistingLocations
                 |> List.map (fun (name, (lat, lng)) ->
                     Marker [ MarkerLatitude lat
                              MarkerLongitude lng
                              MarkerKey name ] [
                         img [ Src "/assets/r-black.png" ]
                         strong [] [ str name ]
                     ])

             if geolocation.loading then
                 loading "locatie aan het zoeken.."
             else
                 ReactMapGL [ OnViewportChange setViewport
                              OnClick onMapClick
                              MapWidth viewport.width
                              MapHeight viewport.height
                              Latitude viewport.latitude
                              Longitude viewport.longitude
                              Zoom viewport.zoom
                              MapClassName "add-location-map"
                              MapStyle "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1" ] [
                     ofList existingRonnies
                     Marker [ MarkerKey "ronny"
                              MarkerLatitude ronnyLatitude
                              MarkerLongitude ronnyLongitude
                              OffsetTop 0
                              OffsetLeft 0 ] [
                         img [ Src "/assets/ronny.png"
                               HTMLAttr.Width 24
                               HTMLAttr.Height 24 ]
                     ]
                     Marker [ MarkerKey "user"
                              MarkerLatitude userLatitude
                              MarkerLongitude userLongitude
                              OffsetTop 0
                              OffsetLeft 0 ] [
                         UserIcon
                     ]
                 ]))
