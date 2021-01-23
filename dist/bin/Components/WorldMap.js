import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, float64_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { ofArray, map, empty, filter, cons, fold } from "../.fable/fable-library.3.1.1/List.js";
import { Location_Read_44B20A7A, NonEmptyString_Read_10C27AE8, Identifier_Read_4C3C6BC4 } from "../shared/Domain.js";
import { ReactMapGLProp, UserIcon, Viewport, useGeolocation } from "./ReactMapGL.js";
import { useReact_useMemo_CF4EA67, useReact_useContext_37FA55CF, useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "./EventContext.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { Marker } from "../../../_snowpack/pkg/react-map-gl.js";
import react$002Dmap$002Dgl from "../../../_snowpack/pkg/react-map-gl.js";
import { To, Link } from "./Navigation.js";
import { printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";

class RonnyLocation extends Record {
    constructor(id, name, latitude, longitude) {
        super();
        this.id = id;
        this.name = name;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}

function RonnyLocation$reflection() {
    return record_type("Ronnies.Client.Components.WorldMap.RonnyLocation", [], RonnyLocation, () => [["id", string_type], ["name", string_type], ["latitude", float64_type], ["longitude", float64_type]]);
}

function getLocations(events) {
    return fold((acc, event) => {
        let copyOfStruct;
        let pattern_matching_result, id_1;
        switch (event.tag) {
            case 1: {
                pattern_matching_result = 1;
                id_1 = event.fields[0];
                break;
            }
            case 2: {
                pattern_matching_result = 1;
                id_1 = event.fields[0];
                break;
            }
            default: pattern_matching_result = 0}
        switch (pattern_matching_result) {
            case 0: {
                const locationAdded = event.fields[0];
                const patternInput = [(copyOfStruct = Identifier_Read_4C3C6BC4(locationAdded.Id), copyOfStruct), NonEmptyString_Read_10C27AE8(locationAdded.Name)];
                const patternInput_1 = Location_Read_44B20A7A(locationAdded.Location);
                return cons(new RonnyLocation(patternInput[0], patternInput[1], patternInput_1[0], patternInput_1[1]), acc);
            }
            case 1: {
                let idAsString;
                let copyOfStruct_1 = Identifier_Read_4C3C6BC4(id_1);
                idAsString = copyOfStruct_1;
                return filter((rl) => (rl.id !== idAsString), acc);
            }
        }
    }, empty(), events);
}

export function WorldMap() {
    let children_4, o, li;
    const geolocation = useGeolocation();
    const patternInput = useFeliz_React__React_useState_Static_1505(new Viewport("100%", "100%", 50.946143, 3.138635, 12));
    const viewport = patternInput[0];
    const setViewport = patternInput[1];
    useReact_useEffect_Z101E1A95(() => {
        if ((!geolocation.loading) ? (geolocation.error == null) : false) {
            setViewport(new Viewport(viewport.width, viewport.height, geolocation.latitude, geolocation.longitude, viewport.zoom));
        }
    }, [geolocation.loading]);
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const icons = map((loc) => react.createElement(Marker, {
        latitude: loc.latitude,
        longitude: loc.longitude,
        key: loc.id,
        offsetLeft: 0,
        offsetTop: 0,
    }, Link([To(toText(printf("/detail/%s"))(loc.id))], [react.createElement("img", {
        src: "/assets/ronny.png",
        height: "20",
        width: "20",
        className: "pointer",
    })])), useReact_useMemo_CF4EA67(() => getLocations(eventCtx.Events), [eventCtx.Events]));
    return react.createElement("div", {
        id: "world-map",
    }, (children_4 = [(o = (((!geolocation.loading) ? (geolocation.error == null) : false) ? react.createElement(Marker, {
        key: "user",
        latitude: geolocation.latitude,
        longitude: geolocation.longitude,
        offsetTop: 0,
        offsetLeft: 0,
    }, UserIcon) : (void 0)), (o == null) ? null : o), Array.from(icons)], react.createElement(react$002Dmap$002Dgl, (li = ofArray([new ReactMapGLProp(9, __SNOWPACK_ENV__.SNOWPACK_PUBLIC_MAPBOX), new ReactMapGLProp(5, setViewport), new ReactMapGLProp(0, viewport.width), new ReactMapGLProp(1, viewport.height), new ReactMapGLProp(2, viewport.latitude), new ReactMapGLProp(3, viewport.longitude), new ReactMapGLProp(4, viewport.zoom), new ReactMapGLProp(8, "mapbox://styles/nojaf/ck0wtbppf0jal1cq72o8i8vm1")]), keyValueList(li, 1)), ...children_4)));
}

