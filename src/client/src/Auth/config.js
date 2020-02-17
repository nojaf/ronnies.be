import { navigate } from "hookrouter";
// eslint-disable-next-line no-restricted-globals
const currentDomain = `${location.protocol}//${location.host}`;

const auth0Config = {
    domain: process.env.REACT_APP_AUTH0_DOMAIN,
    client_id: process.env.REACT_APP_AUTH0_CIENT_ID,
    audience: process.env.REACT_APP_AUTH0_AUDIENCE,
    redirect_uri: `${currentDomain}/oauth`,
    scope: process.env.REACT_APP_AUTH0_SCOPE,
    onRedirectCallback: () => navigate("/")
};

export default auth0Config;