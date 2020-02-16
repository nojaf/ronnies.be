import React, { useEffect, useState } from "react";
import { useAuth0 } from "../Auth";

const TokenPage = () => {
  const { getTokenSilently } = useAuth0();
  const [token, setToken] = useState();
  useEffect(() => {
    getTokenSilently().then(token => {
      setToken(token);
    });
  }, [getTokenSilently, token]);

  return <div>{token}</div>;
};

export default TokenPage;
