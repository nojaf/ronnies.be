import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import ReactMapGL, { Marker } from "react-map-gl";
import { useRonniesNearUserLocation } from "../bin/Hooks";

const UserIcon = () => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      x="0px"
      y="0px"
      width={24}
      height={24}
      viewBox="0 0 172 172"
      style={{ fill: "#000000" }}
    >
      <g
        fill="none"
        fillRule="nonzero"
        stroke="none"
        strokeWidth={1}
        strokeLinecap="butt"
        strokeLinejoin="miter"
        strokeMiterlimit={10}
        strokeDasharray={1}
        strokeDashoffset={0}
        fontFamily="none"
        fontWeight="none"
        fontSize="none"
        textAnchor="none"
        style={{ mixBlendMode: "normal" }}
      >
        <path d="M0,172v-172h172v172z" fill="none" />
        <g fill="#791716">
          <path d="M106.41156,111.89406c-0.51063,-5.54969 -0.30906,-9.41969 -0.30906,-14.48563c2.51281,-1.31687 7.01438,-9.71531 7.76688,-16.81031c1.97531,-0.16125 5.09281,-2.08281 6.00656,-9.68844c0.48375,-4.085 -1.46469,-6.38281 -2.66062,-7.10844c3.21156,-9.66156 9.89,-39.56 -12.33563,-42.65062c-2.29781,-4.01781 -8.15656,-6.04688 -15.76219,-6.04688c-30.46281,0.56438 -34.13125,23.005 -27.45281,48.6975c-1.19594,0.72563 -3.14437,3.02344 -2.66062,7.10844c0.92719,7.60563 4.03125,9.52719 6.00656,9.68844c0.7525,7.095 5.42875,15.49344 7.955,16.81031c0,5.06594 0.18812,8.93594 -0.3225,14.48563c-6.02,16.20562 -46.68188,11.65031 -48.56313,42.90594h130.72c-1.88125,-31.25562 -42.36844,-26.70031 -48.38844,-42.90594z" />
        </g>
      </g>
    </svg>
  );
};

const AddLocationPicker = ({ onChange, userLocation }) => {
  const [viewport, setViewport] = useState({
    width: "100%",
    height: "100%",
    latitude: userLocation[0],
    longitude: userLocation[1],
    zoom: 16,
    mapboxApiAccessToken: process.env.REACT_APP_MAPBOX
  });

  useEffect(() => {
    if (viewport.latitude === 0.0 || viewport.longitude === 0.0) {
      const nextViewport = Object.assign({}, viewport, {
        latitude: userLocation[0],
        longitude: userLocation[1]
      });
      setViewport(nextViewport);
    }
  }, [userLocation, viewport]);

  const nearbyRonniesHelper = useRonniesNearUserLocation();
  const nearbyRonnies = nearbyRonniesHelper(userLocation);
  const [ronnyLocation, setRonnyLocation] = useState(userLocation);
  // Set the ronny location equal to the users start location
  useEffect(() => {
    if (userLocation && ronnyLocation[0] === 0.0 && ronnyLocation[1] === 0.0) {
      setRonnyLocation(userLocation);
      onChange({ ronny: userLocation, user: userLocation });
    }
  }, [userLocation, ronnyLocation, onChange]);

  const onMapClick = ev => {
    const coords = ev.lngLat.reverse();
    setRonnyLocation(coords);
    onChange({ ronny: coords, user: userLocation });
  };

  const existingRonny = ({ lat, lng }, i) => {
    return (
      <Marker latitude={lat} longitude={lng} key={i}>
        <img src="/assets/r-black.png" width={20} height={20} alt={"icon"} />
        <strong className={"text-dark close-ronny-index"}>{i + 1}</strong>{" "}
      </Marker>
    );
  };

  return (
    <div className={"add-location-picker border p-1"}>
      <div className={"row"}>
        <div className={"col-lg-6 map-wrapper"}>
          <ReactMapGL
            {...viewport}
            onViewportChange={setViewport}
            onClick={onMapClick}
            className={"add-location-map"}
            mapStyle={"mapbox://styles/nojaf/ck7846xt30p791inshughvnc0"}
          >
            {nearbyRonnies.map(existingRonny)}
            <Marker
              key={"ronny"}
              latitude={ronnyLocation[0]}
              longitude={ronnyLocation[1]}
              offsetLeft={0}
              offsetTop={0}
            >
              <img src="/assets/ronny.png" width={24} height={24} alt="" />
            </Marker>
            <Marker
              key={"user"}
              latitude={userLocation[0]}
              longitude={userLocation[1]}
              offsetLeft={0}
              offsetTop={0}
            >
              <UserIcon />
            </Marker>
          </ReactMapGL>
        </div>
        <div className={"col-lg-6 my-2 mx-1 mx-md-0"}>
          {nearbyRonnies.length > 0 && (
            <h5>Eerder gevonden ronnies in de buurt</h5>
          )}
          <ol id={"existing-ronnies"}>
            {nearbyRonnies.map((r, i) => (
              <li key={i}>{r.name}</li>
            ))}
          </ol>
        </div>
      </div>
    </div>
  );
};

AddLocationPicker.propTypes = {
  onChange: PropTypes.func.isRequired
};

export default AddLocationPicker;
