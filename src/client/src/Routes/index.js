import React from "react";
import { Home, TokenPage, AddLocation } from "../Pages";
import LocationGuard from "../Components/LocationGuard";

export default {
  "/": () => <Home />,
  "/token": () => <TokenPage />,
  "/add-location": () => <LocationGuard render={location => <AddLocation userLocation={location} />} />
};
