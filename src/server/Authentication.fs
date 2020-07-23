module Ronnies.Server.Authentication

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Ronnies.Domain

type User =
    { Id : string
      Permissions : string list }

type HttpRequest with

    member this.GetUser (logger : ILogger) : User =
        let authorizationHeader =
            this.Headers.["Authorization"].ToString()

        let token =
            authorizationHeader.Replace("Bearer ", System.String.Empty).Replace("bearer ", System.String.Empty)

        let handler =
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()

        let tokenContent = handler.ReadJwtToken(token)
        let id = tokenContent.Subject

        let permissions =
            tokenContent.Claims
            |> Seq.choose (fun claim ->
                if claim.Type = "permissions" then
                    Some claim.Value
                else
                    None)
            |> Seq.toList

        let user = { Id = id; Permissions = permissions }

#if DEBUG
        logger.LogDebug(sprintf "User: %A" user)
#endif

        user

let mayWriteEvent permissions event =
    match event with
    | LocationAdded _ -> Seq.contains "write:location" permissions
