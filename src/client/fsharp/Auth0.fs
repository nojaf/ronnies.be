module Auth0

open Fable.Core
open Fable.Core.JsInterop

type Auth0User =
    abstract picture : string
    abstract nickname : string

[<AllowNullLiteral>]
type IdToken =
    abstract __raw : string with get, set
    abstract name : string option with get, set
    abstract given_name : string option with get, set
    abstract family_name : string option with get, set
    abstract middle_name : string option with get, set
    abstract nickname : string option with get, set
    abstract preferred_username : string option with get, set
    abstract profile : string option with get, set
    abstract picture : string option with get, set
    abstract website : string option with get, set
    abstract email : string option with get, set
    abstract email_verified : bool option with get, set
    abstract gender : string option with get, set
    abstract birthdate : string option with get, set
    abstract zoneinfo : string option with get, set
    abstract locale : string option with get, set
    abstract phone_number : string option with get, set
    abstract phone_number_verified : bool option with get, set
    abstract address : string option with get, set
    abstract updated_at : string option with get, set
    abstract iss : string option with get, set
    abstract aud : string option with get, set
    abstract exp : float option with get, set
    abstract nbf : float option with get, set
    abstract iat : float option with get, set
    abstract jti : string option with get, set
    abstract azp : string option with get, set
    abstract nonce : string option with get, set
    abstract auth_time : string option with get, set
    abstract at_hash : string option with get, set
    abstract c_hash : string option with get, set
    abstract acr : string option with get, set
    abstract amr : string option with get, set
    abstract sub_jwk : string option with get, set
    abstract cnf : string option with get, set
    abstract sid : string option with get, set

    [<Emit("$0[\"https://ronnies.be/roles\"]")>]
    abstract roles : string array

    [<Emit "$0[$1]{{=$2}}">]
    abstract Item : key:string -> obj option with get, set

type Auth0Hook =
    abstract isLoading : bool
    abstract isAuthenticated : bool
    abstract error : obj option
    abstract user : Auth0User
    abstract loginWithRedirect : unit -> JS.Promise<unit>
    abstract logout : unit -> unit
    abstract getAccessTokenSilently : unit -> JS.Promise<string>
    abstract getIdTokenClaims : unit -> JS.Promise<IdToken>

let useAuth0 () : Auth0Hook = import "useAuth0" "@auth0/auth0-react"
