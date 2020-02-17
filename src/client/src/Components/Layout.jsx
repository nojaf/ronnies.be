import React, { useEffect } from "react";
import { useRoutes, navigate } from "hookrouter";
import routes from "../Routes";
import { NotFound } from "../Pages";
import Navigation from "./Navigation";
import { useAuth0 } from "../Auth";
import Loading from "./Loading";
import { useSetToken } from "../bin/Hooks"

const Layout = () => {
  const routeResult = useRoutes(routes);
  let { loading, user, getTokenSilently } = useAuth0();
  // Redirect from the 404 page.
  useEffect(() => {
    const path = localStorage.getItem("path");
    if (path) {
      localStorage.removeItem("path");
      navigate(path);
    }
  }, []);

  const setToken = useSetToken();
  useEffect(function () {
    if(!loading && user){
      getTokenSilently().then(setToken);
    }
  }, [loading, user, getTokenSilently, setToken]);

  const showLoading = loading;

  return showLoading ? (
    <Loading />
  ) : (
    <main>
      <Navigation role={"admin"} />
      {routeResult || <NotFound />}
    </main>
  );
};

export default Layout;
