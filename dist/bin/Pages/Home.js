import { createElement } from "../../../_snowpack/pkg/react.js";
import { WorldMap } from "../Components/WorldMap.js";

function HomePage() {
    return createElement(WorldMap, null);
}

export default (() => createElement(HomePage, null));

