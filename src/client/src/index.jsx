import React from 'react';
import ReactDOM from 'react-dom';
import { Router } from '@reach/router';
import { Auth0Provider } from '@auth0/auth0-react';
import AddLocationPage from './bin/Pages/AddLocation';
import './style.css';
import { Events } from './bin/Components/EventContext';
import Navigation from './bin/Components/Navigation';
import HomePage from './bin/Pages/Home';

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
              <AddLocationPage path="/add-location" />
            </Router>
          </main>
        </Events>
      </Auth0Provider>
    </React.StrictMode>
  );
};

ReactDOM.render(<App />, document.getElementById('root'));

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/#hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept();
}
