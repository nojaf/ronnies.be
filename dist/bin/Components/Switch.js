import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, lambda_type, unit_type, bool_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { classNames } from "../Styles.js";
import { ofSeq } from "../.fable/fable-library.3.1.1/List.js";
import { singleton, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { DOMAttr, HTMLAttr } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";

export class SwitchProps extends Record {
    constructor(TrueLabel, FalseLabel, OnChange, Value, Disabled) {
        super();
        this.TrueLabel = TrueLabel;
        this.FalseLabel = FalseLabel;
        this.OnChange = OnChange;
        this.Value = Value;
        this.Disabled = Disabled;
    }
}

export function SwitchProps$reflection() {
    return record_type("Ronnies.Client.Components.Switch.SwitchProps", [], SwitchProps, () => [["TrueLabel", string_type], ["FalseLabel", string_type], ["OnChange", lambda_type(bool_type, unit_type)], ["Value", bool_type], ["Disabled", bool_type]]);
}

export function Switch(props) {
    return react.createElement("div", keyValueList([classNames(["btn-group", "my-2"])], 1), react.createElement("button", keyValueList([classNames(ofSeq(delay(() => append(singleton("btn"), delay(() => (props.Value ? singleton("btn-primary") : singleton("btn-outline-primary"))))))), new HTMLAttr(79, props.Disabled), new DOMAttr(40, (ev) => {
        ev.preventDefault();
        props.OnChange(true);
    })], 1), props.TrueLabel), react.createElement("button", keyValueList([classNames(ofSeq(delay(() => append(singleton("btn"), delay(() => ((!props.Value) ? singleton("btn-primary") : singleton("btn-outline-primary"))))))), new HTMLAttr(79, props.Disabled), new DOMAttr(40, (ev_1) => {
        ev_1.preventDefault();
        props.OnChange(false);
    })], 1), props.FalseLabel));
}

