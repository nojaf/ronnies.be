import { Record } from "./.fable/fable-library.3.1.1/Types.js";
import { array_type, record_type, string_type } from "./.fable/fable-library.3.1.1/Reflection.js";
import { useAuth0 as useAuth0_1 } from "../../_snowpack/pkg/@auth0/auth0-react.js";
import { contains } from "./.fable/fable-library.3.1.1/Array.js";
import { stringHash } from "./.fable/fable-library.3.1.1/Util.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505 } from "./.fable/Feliz.1.31.1/React.fs.js";

export class LogoutOptions extends Record {
    constructor(returnTo) {
        super();
        this.returnTo = returnTo;
    }
}

export function LogoutOptions$reflection() {
    return record_type("Auth0.LogoutOptions", [], LogoutOptions, () => [["returnTo", string_type]]);
}

export const useAuth0 = useAuth0_1;

export class RolesHook extends Record {
    constructor(Roles) {
        super();
        this.Roles = Roles;
    }
}

export function RolesHook$reflection() {
    return record_type("Auth0.RolesHook", [], RolesHook, () => [["Roles", array_type(string_type)]]);
}

export function RolesHook__get_IsEditor(this$) {
    return contains("editor", this$.Roles, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    });
}

export function RolesHook__get_IsAdmin(this$) {
    return contains("admin", this$.Roles, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    });
}

export function RolesHook__get_IsEditorOrAdmin(this$) {
    if (RolesHook__get_IsEditor(this$)) {
        return true;
    }
    else {
        return RolesHook__get_IsAdmin(this$);
    }
}

export function useRoles() {
    const patternInput = useFeliz_React__React_useState_Static_1505([]);
    const auth0 = useAuth0();
    useReact_useEffect_Z101E1A95(() => {
        if ((!auth0.isLoading) ? auth0.isAuthenticated : false) {
            patternInput[1](auth0.user["https://ronnies.be/roles"]);
        }
    }, [auth0.user, auth0.isLoading]);
    return new RolesHook(patternInput[0]);
}

