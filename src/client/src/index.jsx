import React from 'react';
import ReactDOM from 'react-dom';
import { Router, Link } from "@reach/router"
import AddLocationPage from './bin/Pages/AddLocation';
import './style.css';
import { Events } from './bin/Components/EventContext';
import WorldMap from "./bin/Components/WorldMap";
import Navigation from './bin/Components/Navigation';
import HomePage from './bin/Pages/Home';

const App = () => {
  return (
    <React.StrictMode>
      <Events>
        <main>
          <Navigation />
            <Router>
              <HomePage path="/" />
              <AddLocationPage path="/add-location" />
            </Router>
          <WorldMap />
        </main>
      </Events>
    </React.StrictMode>
  );
};

ReactDOM.render(<App />, document.getElementById('root'));

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/#hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept();
}
