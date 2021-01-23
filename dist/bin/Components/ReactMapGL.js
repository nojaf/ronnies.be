import { useGeolocation as useGeolocation_1 } from "../../../_snowpack/pkg/react-use.js";
import { Union, Record } from "../.fable/fable-library.3.1.1/Types.js";
import { union_type, class_type, lambda_type, unit_type, record_type, int32_type, float64_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import * as react from "../../../_snowpack/pkg/react.js";

export const useGeolocation = useGeolocation_1;

export class Viewport extends Record {
    constructor(width, height, latitude, longitude, zoom) {
        super();
        this.width = width;
        this.height = height;
        this.latitude = latitude;
        this.longitude = longitude;
        this.zoom = (zoom | 0);
    }
}

export function Viewport$reflection() {
    return record_type("Ronnies.Client.Components.ReactMapGL.Viewport", [], Viewport, () => [["width", string_type], ["height", string_type], ["latitude", float64_type], ["longitude", float64_type], ["zoom", int32_type]]);
}

export class ReactMapGLProp extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["width", "height", "Latitude", "Longitude", "Zoom", "OnViewportChange", "OnClick", "className", "MapStyle", "MapboxApiAccessToken"];
    }
}

export function ReactMapGLProp$reflection() {
    return union_type("Ronnies.Client.Components.ReactMapGL.ReactMapGLProp", [], ReactMapGLProp, () => [[["Item", string_type]], [["Item", string_type]], [["Item", float64_type]], [["Item", float64_type]], [["Item", int32_type]], [["Item", lambda_type(Viewport$reflection(), unit_type)]], [["Item", lambda_type(class_type("Ronnies.Client.Components.ReactMapGL.MapClickEvent"), unit_type)]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]]]);
}

export class MarkerProp extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["key", "latitude", "longitude", "OffsetLeft", "OffsetTop"];
    }
}

export function MarkerProp$reflection() {
    return union_type("Ronnies.Client.Components.ReactMapGL.MarkerProp", [], MarkerProp, () => [[["Item", string_type]], [["Item", float64_type]], [["Item", float64_type]], [["Item", int32_type]], [["Item", int32_type]]]);
}

export const UserIcon = react.createElement("svg", {
    xmlns: "http://www.w3.org/2000/svg",
    x: "0px",
    y: "0px",
    width: 24,
    height: 24,
    viewBox: "0 0 172 172",
    style: {
        fill: "#000",
    },
}, react.createElement("g", {
    fill: "none",
    fillRule: "nonzero",
    stroke: "none",
    strokeWidth: "{1}",
    strokeLinecap: "butt",
    strokeLinejoin: "miter",
    strokeMiterlimit: "{10}",
    strokeDasharray: "{1}",
    strokeDashoffset: "{0}",
    fontFamily: "none",
    fontWeight: "none",
    fontSize: "none",
    textAnchor: "none",
}, react.createElement("path", {
    d: "M0,172v-172h172v172z",
    fill: "none",
}), react.createElement("g", {
    fill: "#791716",
}, react.createElement("path", {
    d: "M106.41156,111.89406c-0.51063,-5.54969 -0.30906,-9.41969 -0.30906,-14.48563c2.51281,-1.31687 7.01438,-9.71531 7.76688,-16.81031c1.97531,-0.16125 5.09281,-2.08281 6.00656,-9.68844c0.48375,-4.085 -1.46469,-6.38281 -2.66062,-7.10844c3.21156,-9.66156 9.89,-39.56 -12.33563,-42.65062c-2.29781,-4.01781 -8.15656,-6.04688 -15.76219,-6.04688c-30.46281,0.56438 -34.13125,23.005 -27.45281,48.6975c-1.19594,0.72563 -3.14437,3.02344 -2.66062,7.10844c0.92719,7.60563 4.03125,9.52719 6.00656,9.68844c0.7525,7.095 5.42875,15.49344 7.955,16.81031c0,5.06594 0.18812,8.93594 -0.3225,14.48563c-6.02,16.20562 -46.68188,11.65031 -48.56313,42.90594h130.72c-1.88125,-31.25562 -42.36844,-26.70031 -48.38844,-42.90594z",
}))));

