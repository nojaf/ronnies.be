module Ronnies.Shared

type UsersData = {| includeCurrentUser : bool |}
type CustomClaims = {| ``member`` : bool ; admin : bool |}
type FCMTokenData = {| tokens : string array |}
