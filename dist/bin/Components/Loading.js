import * as react from "../../../_snowpack/pkg/react.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { classNames } from "../Styles.js";

export function loading(message) {
    return react.createElement("div", keyValueList([classNames(["text-center"])], 1), react.createElement("div", keyValueList([classNames(["d-flex", "justify-content-center"])], 1), react.createElement("div", keyValueList([classNames(["spinner-border", "text-primary"])], 1))), react.createElement("p", keyValueList([classNames(["mt-3", "text-primary", "font-italic"])], 1), message));
}

