import { HTMLAttr } from "./.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { join } from "./.fable/fable-library.3.1.1/String.js";

export function classNames(names) {
    return new HTMLAttr(64, join(" ", names));
}

