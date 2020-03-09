import React from "react";
import PropTypes from "prop-types";
import { useUserLocation } from "../bin/Hooks";
import { Loading } from "./index";

const LocationGuard = ({ render }) => {
  const { location } = useUserLocation();
  const isValidLocation = location[0] !== 0 && location[1] !== 0;
  return isValidLocation ? render(location) : <Loading />;
};

LocationGuard.propTypes = {
  render: PropTypes.func.isRequired
};

export default LocationGuard;
