module Ronnies.Client.Model

type Role =
    | Admin
    | Editor
    | Visitor

type Model =
    { Events: obj list
      AuthorizationToken: string option
      Role: Role }

type Msg = SetToken of string
