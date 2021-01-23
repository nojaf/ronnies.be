import * as __SNOWPACK_ENV__ from '../_snowpack/env.js';
import.meta.env = __SNOWPACK_ENV__;

import React from "../_snowpack/pkg/react.js";
import ReactDOM from "../_snowpack/pkg/react-dom.js";
import {Router} from "../_snowpack/pkg/@reach/router.js";
import {Auth0Provider, withAuthenticationRequired} from "../_snowpack/pkg/@auth0/auth0-react.js";
import {ToastContainer} from "../_snowpack/pkg/react-toastify.js";
import AddLocationPage from "./bin/Pages/AddLocation.js";
import Settings2 from "./bin/Pages/Settings.js";
import "./style.css.proxy.js";
import {Events} from "./bin/Components/EventContext.js";
import Navigation2 from "./bin/Components/Navigation.js";
import HomePage from "./bin/Pages/Home.js";
import DetailPage from "./bin/Pages/Detail.js";
import OverviewPage from "./bin/Pages/Overview.js";
import RulesPage from "./bin/Pages/Rules.js";
import LeaderboardPage from "./bin/Pages/Leaderboard.js";
const AddLocation2 = withAuthenticationRequired(AddLocationPage);
const Rules2 = withAuthenticationRequired(RulesPage);
const Leaderbord = withAuthenticationRequired(LeaderboardPage);
const App = () => {
  return /* @__PURE__ */ React.createElement(React.StrictMode, null, /* @__PURE__ */ React.createElement(Auth0Provider, {
    domain: __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AUTH0_DOMAIN,
    clientId: __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AUTH0_CIENT_ID,
    audience: __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AUTH0_AUDIENCE,
    scope: __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AUTH0_SCOPE,
    redirectUri: window.location.origin
  }, /* @__PURE__ */ React.createElement(Events, null, /* @__PURE__ */ React.createElement("main", null, /* @__PURE__ */ React.createElement(Navigation2, null), /* @__PURE__ */ React.createElement(Router, null, /* @__PURE__ */ React.createElement(HomePage, {
    path: "/"
  }), /* @__PURE__ */ React.createElement(AddLocation2, {
    path: "/add-location"
  }), /* @__PURE__ */ React.createElement(Settings2, {
    path: "/settings"
  }), /* @__PURE__ */ React.createElement(DetailPage, {
    path: "/detail/:id"
  }), /* @__PURE__ */ React.createElement(OverviewPage, {
    path: "/overview"
  }), /* @__PURE__ */ React.createElement(Rules2, {
    path: "/rules"
  }), /* @__PURE__ */ React.createElement(Leaderbord, {
    path: "/leaderboard"
  })), /* @__PURE__ */ React.createElement(ToastContainer, null)))));
};
ReactDOM.render(/* @__PURE__ */ React.createElement(React.StrictMode, null, /* @__PURE__ */ React.createElement(App, null)), document.getElementById("root"));
if ("serviceWorker" in navigator) {
  navigator.serviceWorker.register("/sw.js", {scope: "./"}).then((swr) => {
    console.log(`Service worker registered`);
  });
}
if (undefined /* [snowpack] import.meta.hot */ ) {
  undefined /* [snowpack] import.meta.hot */ .accept();
}
