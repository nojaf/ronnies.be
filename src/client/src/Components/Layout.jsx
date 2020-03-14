import React, { useEffect } from "react";
import { useRoutes } from "hookrouter";
import routes from "../Routes";
import { NotFound } from "../Pages";
import Navigation from "./Navigation";
import { useAuth0 } from "../Auth";
import Loading from "./Loading";
import {useSetToken, useAppLoading, useAppException} from "../bin/Hooks";
import AppError from "./AppError";
import {Container} from "reactstrap";

const Layout = () => {
  const routeResult = useRoutes(routes);
  let { loading:auth0Loading, user, getTokenSilently } = useAuth0();
  const appLoading = useAppLoading();
  const setToken = useSetToken();
  const appException = useAppException()

  useEffect(
    function() {
      if (!auth0Loading && user) {
        getTokenSilently().then(setToken);
      }
    }, // eslint-disable-next-line react-hooks/exhaustive-deps
    [auth0Loading, user]
  );

  const showLoading = auth0Loading || appLoading;

  return showLoading ? (
    <Loading />
  ) : (
    <main>
      <Navigation role={"Admin"} />
      <Container className={"py-2"}>
      {(appException && <AppError error={appException}/>) || routeResult || <NotFound />}
      </Container>
    </main>
  );
};

export default Layout;
