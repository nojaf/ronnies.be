import React from "react";
import { Layout } from "./Components";
import { Auth0Provider, ronnyOptions } from "./Auth";

function App() {
  return (
    <Auth0Provider {...ronnyOptions}>
      <Layout />
    </Auth0Provider>
  );
}

export default App;

// florian.verdonck@outlook.com
// f6rNDkBssQoEUd
