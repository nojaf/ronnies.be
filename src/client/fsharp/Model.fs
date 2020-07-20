module Ronnies.Client.Model

open Ronnies.Shared

type Toast =
    { Icon : string
      Title : string
      Body : string }

type Role =
    | Admin
    | Editor
    | Visitor

type Model =
    { Events : Event list
      AuthorizationToken : string option
      UserId : string option
      Role : Role
      Toasts : Map<int, Toast>
      IsLoading : bool
      AppException : exn option }

type Msg =
    | SetToken of string
    | EventsReceived of Event list
    | AppException of exn
    | AddLocation of Event
    | LocationAdded of unit
    //| PersistEvents of Event list
    | ShowToast of Toast
    | ClearToast of int
