module Ronnies.Server.Authentication

open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Protocols
open Microsoft.IdentityModel.Protocols.OpenIdConnect
open Microsoft.IdentityModel.Tokens
open Ronnies.Shared
open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims

let private Auth0Domain =
    Environment.GetEnvironmentVariable("Auth0Domain")

let private Auth0Audiences =
    Environment.GetEnvironmentVariable("Auth0Audience")
    |> Array.singleton

let private collectClaims (user: ClaimsPrincipal) =
    user.Claims
    |> Seq.choose (fun c -> if c.Type = "permissions" then Some c.Value else None)
    |> Seq.toList

let private authenticateRequest (logger: ILogger) header =
    let token =
        System.Text.RegularExpressions.Regex.Replace(header, "(b|B)earer\s?", System.String.Empty)
#if DEBUG
    printfn "token: %s" token
#endif
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII <- true
    let parameters = TokenValidationParameters()
    parameters.ValidIssuer <- (sprintf "https://%s/" Auth0Domain)
    parameters.ValidAudiences <- Auth0Audiences
    parameters.ValidateIssuer <- true
    parameters.NameClaimType <- ClaimTypes.NameIdentifier // Auth0 related, see https://community.auth0.com/t/access-token-doesnt-contain-a-sub-claim/17671/2

    let manager =
        ConfigurationManager<OpenIdConnectConfiguration>
            (sprintf "https://%s/.well-known/openid-configuration" Auth0Domain, OpenIdConnectConfigurationRetriever())

    let handler = JwtSecurityTokenHandler()

    try
        task {
            let! config = manager.GetConfigurationAsync().ConfigureAwait(false)
            parameters.IssuerSigningKeys <- config.SigningKeys

            let user, _ =
                handler.ValidateToken((token: string), parameters)

            return Ok(user.Identity.Name, collectClaims user)
        }
    with exn ->
        logger.LogError(sprintf "Could not authenticate token %s\n%A" token exn)
        task { return Error "invalid or empty token" }

type HttpRequest with
    member this.Authenticate(logger: ILogger) = authenticateRequest logger (this.Headers.["Authorization"].ToString())

let mayWriteEvent permissions event =
    match event with
    | LocationAdded _ -> Seq.contains "write:location" permissions
    | _ -> false
