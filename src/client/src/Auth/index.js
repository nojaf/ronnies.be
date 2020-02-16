import { navigate } from "hookrouter";

const isProduction = process.env.NODE_ENV === "production";

export const redirectUri = isProduction
  ? "https://ronnies.be/oauth"
  : "http://localhost:3000/oauth";

export const ronnyOptions = {
  domain: "nojaf.eu.auth0.com",
  client_id: "9QM23irmnI7ZcxxPJFAv5Sxc8HZymuUS",
  redirect_uri: redirectUri,
  scope: "openid profile email",
  audience: "https://ronnies.be",
  onRedirectCallback: () => navigate("/")
};

export { Auth0Provider, useAuth0 } from "./react-auth0-wrapper";
