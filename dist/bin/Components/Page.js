import * as react from "../../../_snowpack/pkg/react.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { ofSeq } from "../.fable/fable-library.3.1.1/List.js";
import { singleton, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { HTMLAttr } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { classNames } from "../Styles.js";

export function page(attributes, children) {
    return react.createElement("div", keyValueList(ofSeq(delay(() => append(singleton(new HTMLAttr(64, "page")), delay(() => attributes)))), 1), react.createElement("div", keyValueList([classNames(["container", "p-3"])], 1), ...children));
}

