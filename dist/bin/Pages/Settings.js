import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "../.fable/Fable.Promise.2.1.0/Promise.fs.js";
import { promise } from "../.fable/Fable.Promise.2.1.0/PromiseImpl.fs.js";
import { toText, printf, toConsole } from "../.fable/fable-library.3.1.1/String.js";
import { subscriptionHeader, authHeader, vapidKey } from "../Common.js";
import { some } from "../.fable/fable-library.3.1.1/Option.js";
import { Types_HttpRequestHeaders, Types_RequestProperties, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { ofSeq, ofArray } from "../.fable/fable-library.3.1.1/List.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { errorToast, infoToast } from "../ReactToastify.js";
import { useReact_useEffectOnce_3A5B6456, useReact_useEffect_Z101E1A95, useReact_useContext_37FA55CF, useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "../Components/EventContext.js";
import { RolesHook__get_IsEditorOrAdmin, useRoles, useAuth0 } from "../Auth0.js";
import { page } from "../Components/Page.js";
import { empty, singleton, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { classNames } from "../Styles.js";
import { HTMLAttr, DOMAttr } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { SwitchProps, Switch } from "../Components/Switch.js";
import { loading } from "../Components/Loading.js";

const browserSupportsNotifications = ('PushManager' in window) ? ('serviceWorker' in navigator) : false;

function urlB64ToUint8Array(value) {
    const padding = '='.repeat((4 - value.length % 4) % 4);
    const base64 = (value + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');
    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);
    for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

function hasSubscription() {
    const matchValue = navigator.serviceWorker;
    if (matchValue == null) {
        return Promise.resolve(false);
    }
    else {
        let pr_1;
        const pr = matchValue.ready;
        pr_1 = (pr.then(((_sw) => (_sw.pushManager.getSubscription()))));
        return pr_1.then(((option) => (option != null)));
    }
}

function addSubscription(token) {
    const matchValue = navigator.serviceWorker;
    if (matchValue == null) {
    }
    else {
        let pr_5;
        let pr_4;
        const pr = matchValue.ready;
        pr_4 = (pr.then(((sw_1) => PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => ((sw_1.pushManager.getSubscription()).then(((_arg1) => (Promise.resolve([sw_1, _arg1]))))))))));
        pr_5 = (pr_4.then(((tupledArg) => {
            const sub_1 = tupledArg[1];
            if (sub_1 != null) {
                const sub_2 = sub_1;
                toConsole(printf("unsubscribed"));
                const pr_3 = sub_2.unsubscribe();
                return pr_3.then(((value) => {
                    void value;
                }));
            }
            else {
                const key = urlB64ToUint8Array(vapidKey);
                let pr_2;
                const pr_1 = tupledArg[0].pushManager.subscribe({userVisibleOnly: true, applicationServerKey: key});
                pr_2 = (pr_1.then(((subscription) => {
                    let arg10, headers;
                    const json = JSON.stringify(subscription.toJSON());
                    console.log(some(subscription));
                    console.log(some(json));
                    return fetch$((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/subscriptions"))(arg10)), ofArray([new Types_RequestProperties(0, "POST"), new Types_RequestProperties(2, json), (headers = ofArray([new Types_HttpRequestHeaders(11, "application/json"), authHeader(token), subscriptionHeader]), new Types_RequestProperties(1, keyValueList(headers, 0)))]));
                })));
                return pr_2.then(((_arg1_1) => {
                    infoToast("Notificaties check!");
                }));
            }
        })));
        pr_5.then(void 0, ((err) => {
            console.error(some(err));
            errorToast("Notificaties aanzetten niet echt gelukt");
        }));
    }
}

function removeSubscription(token) {
    const matchValue = navigator.serviceWorker;
    if (matchValue == null) {
    }
    else {
        let pr_4;
        let pr_3;
        const pr = matchValue.ready;
        pr_3 = (pr.then(((_sw) => (_sw.pushManager.getSubscription()))));
        pr_4 = (pr_3.then(((sub) => {
            if (sub != null) {
                const subscription = sub;
                let pr_2;
                const pr_1 = subscription.unsubscribe();
                pr_2 = (pr_1.then(((_arg1) => {
                    let arg10, headers;
                    return fetch$((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/subscriptions"))(arg10)), ofArray([new Types_RequestProperties(0, "DELETE"), new Types_RequestProperties(2, subscription.endpoint), (headers = ofArray([new Types_HttpRequestHeaders(11, "application/json"), authHeader(token), subscriptionHeader]), new Types_RequestProperties(1, keyValueList(headers, 0)))]));
                })));
                return pr_2.then(((_arg2) => {
                    infoToast("Notificaties uitgezet!");
                }));
            }
            else {
                return Promise.resolve(undefined);
            }
        })));
        pr_4.then(void 0, ((err) => {
            console.error(some(err));
            errorToast("Notificaties uitzetten niet echt gelukt");
        }));
    }
}

function Settings() {
    const patternInput = useFeliz_React__React_useState_Static_1505(false);
    const setIsLoading = patternInput[1];
    const isLoading = patternInput[0];
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const auth0 = useAuth0();
    const patternInput_1 = useFeliz_React__React_useState_Static_1505("");
    const token = patternInput_1[0];
    const roles = useRoles();
    useReact_useEffect_Z101E1A95(() => {
        if (auth0.isAuthenticated) {
            const pr_1 = auth0.getAccessTokenSilently();
            pr_1.then(patternInput_1[1]);
        }
    }, [auth0.isAuthenticated]);
    const patternInput_2 = useFeliz_React__React_useState_Static_1505(false);
    const setNotifications = patternInput_2[1];
    const notifications = patternInput_2[0];
    useReact_useEffectOnce_3A5B6456(() => {
        const pr_2 = hasSubscription();
        pr_2.then(setNotifications);
    });
    return page([], ofSeq(delay(() => append(singleton(react.createElement("h1", {}, "Settings")), delay(() => append(singleton(react.createElement("button", keyValueList([classNames(["btn", "btn-outline-primary", "my-4"]), new DOMAttr(40, (_arg1) => {
        setIsLoading(true);
        const pr = eventCtx.ClearCache();
        pr.then((() => {
            setIsLoading(false);
            infoToast("Cache reset!");
        }));
    }), new HTMLAttr(79, isLoading)], 1), "Reset cache")), delay(() => append(singleton(react.createElement("h4", {}, "Notificaties?")), delay(() => append((browserSupportsNotifications ? RolesHook__get_IsEditorOrAdmin(roles) : false) ? singleton(Switch(new SwitchProps("Aan", "Uit", (value) => {
        if (value !== notifications) {
            setIsLoading(true);
            if (value) {
                addSubscription(token);
            }
            else {
                removeSubscription(token);
            }
            setIsLoading(false);
            setNotifications(value);
        }
    }, notifications, isLoading))) : singleton(react.createElement("div", keyValueList([classNames(["alert", "alert-warning"])], 1), "Je browser ondersteunt geen notificaties")), delay(() => (isLoading ? singleton(loading("Syncen met de server...")) : empty()))))))))))));
}

export default (() => createElement(Settings, null));

