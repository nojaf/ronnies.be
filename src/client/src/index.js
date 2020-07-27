import React from "react";
import ReactDOM from "react-dom";
import * as serviceWorker from "./serviceWorker";
import { Router } from "@reach/router";
import { Auth0Provider } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import AddLocationPage from "./bin/Pages/AddLocation";
import Settings from "./bin/Pages/Settings";
import "./style.css";
import { Events } from "./bin/Components/EventContext";
import Navigation from "./bin/Components/Navigation";
import HomePage from "./bin/Pages/Home";

const App = () => {
  return (
    <React.StrictMode>
      <Auth0Provider
        domain={process.env.REACT_APP_AUTH0_DOMAIN}
        clientId={process.env.REACT_APP_AUTH0_CIENT_ID}
        audience={process.env.REACT_APP_AUTH0_AUDIENCE}
        scope={process.env.REACT_APP_AUTH0_SCOPE}
        redirectUri={window.location.origin}
      >
        <Events>
          <main>
            <Navigation />
            <Router>
              <HomePage path="/" />
              <AddLocationPage path="/add-location" />
              <Settings path="/settings" />
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

if ('serviceWorker' in navigator) {
    navigator.serviceWorker
        .register('/sw.js', { scope: './' })
        .then(swr => {
            console.log(`Service worker registered`);
        })
}

