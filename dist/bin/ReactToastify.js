import { Union } from "./.fable/fable-library.3.1.1/Types.js";
import { union_type, bool_type, string_type } from "./.fable/fable-library.3.1.1/Reflection.js";
import { toast as toast_1 } from "../../_snowpack/pkg/react-toastify.js";

export class ToastOptions extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["position", "HideProgressBar"];
    }
}

export function ToastOptions$reflection() {
    return union_type("ReactToastify.ToastOptions", [], ToastOptions, () => [[["Item", string_type]], [["Item", bool_type]]]);
}

const toast = toast_1;

const defaultToastOptions = {
    position: "bottom-right",
    hideProgressBar: true,
};

export function successToast(title) {
    toast.success(title, defaultToastOptions);
}

export function infoToast(title) {
    toast.info(title, defaultToastOptions);
}

export function errorToast(title) {
    toast.error(title, defaultToastOptions);
}

