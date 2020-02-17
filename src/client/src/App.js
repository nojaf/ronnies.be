import React from "react";
import { Layout } from "./Components";
import { Auth0Provider, config } from "./Auth";
import { ElmishCapture } from "./bin/View";

function App() {
  return (
    <Auth0Provider {...config}>
      <ElmishCapture>
        <Layout />
      </ElmishCapture>
    </Auth0Provider>
  );
}

export default App;

// florian.verdonck@outlook.com
// f6rNDkBssQoEUd
