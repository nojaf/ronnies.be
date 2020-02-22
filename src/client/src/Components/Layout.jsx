import React, { useEffect } from "react";
import { useRoutes } from "hookrouter";
import routes from "../Routes";
import { NotFound } from "../Pages";
import Navigation from "./Navigation";
import { useAuth0 } from "../Auth";
import Loading from "./Loading";
import { useSetToken } from "../bin/Hooks";

const Layout = () => {
  const routeResult = useRoutes(routes);
  let { loading, user, getTokenSilently } = useAuth0();
  const setToken = useSetToken();

  useEffect(
    function() {
      if (!loading && user) {
        getTokenSilently().then(setToken);
      }
    }, // eslint-disable-next-line react-hooks/exhaustive-deps
    [loading, user]
  );

  const showLoading = loading;

  return showLoading ? (
    <Loading />
  ) : (
    <main>
      <Navigation role={"Admin"} />
      {routeResult || <NotFound />}
    </main>
  );
};

export default Layout;
