import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { useNavigate as useNavigate_1, Link as Link_1 } from "../../../_snowpack/pkg/@reach/router.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { RolesHook__get_IsEditorOrAdmin, RolesHook__get_IsAdmin, LogoutOptions, useRoles, useAuth0 } from "../Auth0.js";
import { classNames } from "../Styles.js";
import { ofArray, empty, singleton as singleton_1, ofSeq } from "../.fable/fable-library.3.1.1/List.js";
import { empty as empty_1, append, singleton, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { Prop, HTMLAttr, DOMAttr } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { some } from "../.fable/fable-library.3.1.1/Option.js";

export function To(url) {
    return ["to", url];
}

export function Link(props, children) {
    return react.createElement(Link_1, keyValueList(props, 1), ...children);
}

export const useNavigate = useNavigate_1;

function Navigation() {
    const patternInput = useFeliz_React__React_useState_Static_1505(false);
    const setMobileOpen = patternInput[1];
    const mobileOpen = patternInput[0];
    const auth0 = useAuth0();
    const roles = useRoles();
    const loginLink = react.createElement("li", keyValueList([classNames(["nav-item"])], 1), react.createElement("a", {
        href: "#",
        className: "nav-link",
        onClick: (ev) => {
            ev.preventDefault();
            const pr = auth0.loginWithRedirect();
            pr.then();
        },
    }, "Inloggen"));
    const logoutLink = react.createElement("li", keyValueList([classNames(["nav-item"])], 1), react.createElement("a", {
        href: "#",
        className: "nav-link",
        onClick: (ev_1) => {
            ev_1.preventDefault();
            const value = auth0.logout(new LogoutOptions(window.location.origin));
            void value;
        },
    }, "logout"));
    const rightNavbar = react.createElement("ul", {
        className: "navbar-nav",
    }, ...ofSeq(delay(() => {
        let user;
        return (!auth0.isAuthenticated) ? singleton(loginLink) : [logoutLink, (user = auth0.user, react.createElement("li", keyValueList([classNames(["nav-item"])], 1), Link([classNames(["nav-link", "active"]), To("/settings"), new DOMAttr(40, (_arg1) => {
            setMobileOpen(false);
        })], [react.createElement("img", keyValueList([new HTMLAttr(149, user.picture), ["style", {
            marginTop: "-2px",
        }], classNames(["avatar"]), new HTMLAttr(8, "user avatar")], 1)), user.nickname])))];
    })));
    const menuLink = (path, label) => react.createElement("li", keyValueList([classNames(["nav-item"]), new Prop(0, toText(printf("menu-%s"))(path))], 1), Link([To(path), new HTMLAttr(64, "nav-link"), new DOMAttr(40, (_arg2) => {
        setMobileOpen(false);
    })], [label]));
    const adminItems = RolesHook__get_IsAdmin(roles) ? singleton_1(react.createElement("li", keyValueList([classNames(["nav-item"]), new Prop(0, "bearerButton"), new DOMAttr(40, (ev_2) => {
        ev_2.preventDefault();
        if ((!auth0.isLoading) ? auth0.isAuthenticated : false) {
            const pr_1 = auth0.getAccessTokenSilently();
            pr_1.then(((token) => {
                console.log(some(token));
            }));
        }
    })], 1), react.createElement("a", {
        href: "#",
        className: "nav-link",
    }, "Bearer"))) : empty();
    const editorItems = RolesHook__get_IsEditorOrAdmin(roles) ? ofArray([menuLink("/add-location", "E nieuwen toevoegen"), menuLink("/leaderboard", "Klassement"), menuLink("/rules", "Manifesto")]) : empty();
    return react.createElement("nav", keyValueList([classNames(["navbar", "navbar-expand-md", "navbar-dark", "bg-primary"])], 1), Link([To("/"), new HTMLAttr(64, "navbar-brand"), new DOMAttr(40, (_arg3) => {
        setMobileOpen(false);
    })], [react.createElement("img", {
        src: "/assets/r-white.png",
        alt: "logo ronnies.be",
    })]), react.createElement("button", {
        className: "navbar-toggler",
        onClick: (_arg4) => {
            setMobileOpen(!mobileOpen);
        },
    }, react.createElement("span", {
        className: "navbar-toggler-icon",
    })), react.createElement("div", keyValueList([classNames(ofSeq(delay(() => append(singleton("collapse"), delay(() => append(singleton("navbar-collapse"), delay(() => (mobileOpen ? singleton("show") : empty_1()))))))))], 1), ...ofSeq(delay(() => append(singleton(react.createElement("ul", keyValueList([classNames(["navbar-nav", "mr-auto"])], 1), menuLink("/overview", "Overzicht"), Array.from(editorItems), Array.from(adminItems))), delay(() => ((!auth0.isLoading) ? singleton(rightNavbar) : empty_1())))))));
}

export default (() => createElement(Navigation, null));

