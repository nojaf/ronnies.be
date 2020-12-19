import React from "react";
import ReactDOM from "react-dom";
import { Router } from "@reach/router";
import { Auth0Provider, withAuthenticationRequired } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import AddLocationPage from "./bin/Pages/AddLocation";
import Settings from "./bin/Pages/Settings";
import "./style.sass";
import { Events } from "./bin/Components/EventContext";
import Navigation from "./bin/Components/Navigation";
import HomePage from "./bin/Pages/Home";
import DetailPage from "./bin/Pages/Detail";
import OverviewPage from "./bin/Pages/Overview";
import RulesPage from "./bin/Pages/Rules";
import LeaderboardPage from "./bin/Pages/Leaderboard";

const AddLocation = withAuthenticationRequired(AddLocationPage);
const Rules = withAuthenticationRequired(RulesPage);
const Leaderbord = withAuthenticationRequired(LeaderboardPage);

const App = () => {
  return (
    <React.StrictMode>
      <Auth0Provider
        domain={import.meta.env.SNOWPACK_PUBLIC_AUTH0_DOMAIN}
        clientId={import.meta.env.SNOWPACK_PUBLIC_AUTH0_CIENT_ID}
        audience={import.meta.env.SNOWPACK_PUBLIC_AUTH0_AUDIENCE}
        scope={import.meta.env.SNOWPACK_PUBLIC_AUTH0_SCOPE}
        redirectUri={window.location.origin}
      >
        <Events>
          <main>
            <Navigation />
            <Router>
              <HomePage path="/" />
              <AddLocation path="/add-location" />
              <Settings path="/settings" />
              <DetailPage path="/detail/:id" />
              <OverviewPage path="/overview" />
              <Rules path="/rules" />
              <Leaderbord path="/leaderboard" />
            </Router>
            <ToastContainer />
          </main>
        </Events>
      </Auth0Provider>
    </React.StrictMode>
  );
};

ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
// serviceWorker.register();

if ("serviceWorker" in navigator) {
  navigator.serviceWorker.register("/sw.js", { scope: "./" }).then((swr) => {
    console.log(`Service worker registered`);
  });
}

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/concepts/hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept();
}