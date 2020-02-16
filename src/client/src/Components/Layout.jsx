import React, { useEffect } from "react";
import { useRoutes, navigate } from "hookrouter";
import routes from "../Routes";
import { NotFound } from "../Pages";
import Navigation from "./Navigation";
import { useAuth0 } from "../Auth";
import Loading from "./Loading";

const Layout = () => {
  const routeResult = useRoutes(routes);
  let { loading } = useAuth0();
  // Redirect from the 404 page.
  useEffect(() => {
    const path = localStorage.getItem("path");
    if (path) {
      localStorage.removeItem("path");
      navigate(path);
    }
  }, []);

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
