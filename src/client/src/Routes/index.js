import React from "react";
import { Home, TokenPage, AddLocation } from "../Pages";

export default {
  "/": () => <Home />,
  "/token": () => <TokenPage />,
  "/add-location": () => <AddLocation />
};
