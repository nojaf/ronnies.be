import React, { useState } from "react";
import { Button, Jumbotron, Alert } from "reactstrap";

const AppError = ({ error }) => {
  const [showError, setShowError] = useState(false);

  return (
    <div className={"p-4"}>
      <Jumbotron className={"border border-danger"}>
        <h1 className={"display-3"}>Tis ol na de klootn</h1>
        <p className={"text-danger"}>En de keuning is e blootn</p>
        <hr className={"my-2"} />
        <p>
          Euhm, jah, oe goat dat oal <span role={"img"} aria-label={""}>😅</span> ,<br />
          bugje in de software. Kan gebeuren zekers
        </p>
        <p className="lead">
          <Button color="primary" onClick={() => setShowError(!showError)}>
            Toon details
          </Button>
        </p>
        {showError && (
          <Alert color={"danger"}>
            <pre>{error.toString()}</pre>
          </Alert>
        )}
      </Jumbotron>
    </div>
  );
};

export default AppError;
