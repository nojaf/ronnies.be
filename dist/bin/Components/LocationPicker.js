import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { ReactMapGLProp, UserIcon, Viewport, useGeolocation } from "./ReactMapGL.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { ofArray, map } from "../.fable/fable-library.3.1.1/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { Marker } from "../../../_snowpack/pkg/react-map-gl.js";
import react$002Dmap$002Dgl from "../../../_snowpack/pkg/react-map-gl.js";
import { loading } from "./Loading.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";

export function LocationPicker(props) {
    let li;
    const geolocation = useGeolocation();
    const patternInput = useFeliz_React__React_useState_Static_1505(50.946139);
    const userLatitude = patternInput[0];
    const patternInput_1 = useFeliz_React__React_useState_Static_1505(3.138671);
    const userLongitude = patternInput_1[0];
    const patternInput_2 = useFeliz_React__React_useState_Static_1505(userLatitude);
    const setRonnyLatitude = patternInput_2[1];
    const patternInput_3 = useFeliz_React__React_useState_Static_1505(userLongitude);
    const setRonnyLongitude = patternInput_3[1];
    const patternInput_4 = useFeliz_React__React_useState_Static_1505(new Viewport("100%", "100%", userLatitude, userLongitude, 16));
    const viewport = patternInput_4[0];
    const setViewport = patternInput_4[1];
    useReact_useEffect_Z101E1A95(() => {
        if (!geolocation.loading) {
            patternInput[1](geolocation.latitude);
            patternInput_1[1](geolocation.longitude);
            setViewport(new Viewport("100%", "100%", geolocation.latitude, geolocation.longitude, 16));
            setRonnyLatitude(geolocation.latitude);
            setRonnyLongitude(geolocation.longitude);
            props.OnChange([geolocation.latitude, geolocation.longitude], [geolocation.latitude, geolocation.longitude]);
        }
    }, [geolocation.loading]);
    const existingRonnies = map((tupledArg) => {
        const name = tupledArg[0];
        const _arg1 = tupledArg[1];
        return react.createElement(Marker, {
            latitude: _arg1[0],
            longitude: _arg1[1],
            key: name,
        }, react.createElement("img", {
            src: "/assets/r-black.png",
        }), react.createElement("strong", {}, name));
    }, props.ExistingLocations);
    if (geolocation.loading) {
        return loading("locatie aan het zoeken..");
    }
    else {
        const children_8 = [Array.from(existingRonnies), react.createElement(Marker, {
            key: "ronny",
            latitude: patternInput_2[0],
            longitude: patternInput_3[0],
            offsetTop: 0,
            offsetLeft: 0,
        }, react.createElement("img", {
            src: "/assets/ronny.png",
            width: 24,
            height: 24,
        })), react.createElement(Marker, {
            key: "user",
            latitude: userLatitude,
            longitude: userLongitude,
            offsetTop: 0,
            offsetLeft: 0,
        }, UserIcon)];
        return react.createElement(react$002Dmap$002Dgl, (li = ofArray([new ReactMapGLProp(9, __SNOWPACK_ENV__.SNOWPACK_PUBLIC_MAPBOX), new ReactMapGLProp(5, setViewport), new ReactMapGLProp(6, (ev) => {
            const patternInput_5 = ev.lngLat.reverse();
            const lng = patternInput_5[1];
            const lat = patternInput_5[0];
            setRonnyLatitude(lat);
            setRonnyLongitude(lng);
            props.OnChange([userLatitude, userLongitude], [lat, lng]);
        }), new ReactMapGLProp(0, viewport.width), new ReactMapGLProp(1, viewport.height), new ReactMapGLProp(2, viewport.latitude), new ReactMapGLProp(3, viewport.longitude), new ReactMapGLProp(4, viewport.zoom), new ReactMapGLProp(7, "add-location-map"), new ReactMapGLProp(8, "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1")]), keyValueList(li, 1)), ...children_8);
    }
}

