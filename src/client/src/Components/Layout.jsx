import React from "react";
import { useRoutes } from "hookrouter";
import routes from "../Routes";
import { NotFound } from "../Pages";
import Navigation from "./Navigation";
import { useAuth0 } from "../Auth";
import Loading from "./Loading";

const Layout = () => {
  const routeResult = useRoutes(routes);
  let { loading } = useAuth0();

  const showLoading = loading;

  return showLoading ? (
    <Loading />
  ) : (
    <main>
      <Navigation role={'admin'} />
      {routeResult || <NotFound />}
    </main>
  );
};

export default Layout;
