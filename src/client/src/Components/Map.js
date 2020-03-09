import React, { memo, useState } from "react";
import PropTypes from "prop-types";
import ReactMapGL, { Marker } from "react-map-gl";
import { usePath } from "hookrouter";
import { fitBounds } from "viewport-mercator-project";
import { getBoundingBox } from "geolocation-utils";

const Map = ({ ronnies }) => {
  const { clientWidth: width, clientHeight: height } = document.documentElement;
  const boundingBox = getBoundingBox(ronnies, 50000);
  const bounds = fitBounds({
    bounds: [
      [boundingBox.topLeft.lng, boundingBox.topLeft.lat],
      [boundingBox.bottomRight.lng, boundingBox.bottomRight.lat]
    ],
    width: width,
    height
  });

  const [viewport, setViewport] = useState({
    width: "100%",
    height: "100%",
    latitude: bounds.latitude,
    longitude: bounds.longitude,
    zoom: bounds.zoom,
    mapboxApiAccessToken: process.env.REACT_APP_MAPBOX
  });
  const path = usePath();
  const containerClassName = `mapContainer ${path === "/" ? "home" : ""}`;

  const pins = ronnies.map((ronny, i) => {
    return (
      <Marker key={i} longitude={ronny.lng} latitude={ronny.lat}>
        <img src={"/assets/favicon-16x16.png"} alt={"ronny icon"} />
      </Marker>
    );
  });

  return (
    <div className={containerClassName}>
      <ReactMapGL
        {...viewport}
        mapStyle={"mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1"}
        onViewportChange={viewport => setViewport(viewport)}
      >
        {pins}
      </ReactMapGL>
    </div>
  );
};

const ronnyProptype = PropTypes.shape({
  creator: PropTypes.shape({
    id: PropTypes.string.isRequired,
    nickname: PropTypes.string.isRequired
  }).isRequired,
  id: PropTypes.string.isRequired,
  lat: PropTypes.number.isRequired,
  lng: PropTypes.number.isRequired,
  name: PropTypes.string.isRequired,
  price: PropTypes.number.isRequired
});

Map.propTypes = {
  ronnies: PropTypes.arrayOf(ronnyProptype)
};

export default memo(Map);
