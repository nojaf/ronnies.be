import auth0Config from "./config";
export { Auth0Provider, useAuth0 } from "./react-auth0-wrapper";
export const config = auth0Config;
export const redirectUri = auth0Config.redirect_uri;
