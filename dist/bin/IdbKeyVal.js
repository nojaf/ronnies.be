import * as __SNOWPACK_ENV__ from '../../_snowpack/env.js';

import { Store, clear as clear_1, keys as keys_2, set as set$_1, get as get$_1 } from "../../_snowpack/pkg/idb-keyval.js";
import { map, max } from "./.fable/fable-library.3.1.1/Array.js";
import { uncurry, comparePrimitives } from "./.fable/fable-library.3.1.1/Util.js";
import { keyValuePairs, fromString, fromValue } from "./.fable/Thoth.Json.4.1.0/Decode.fs.js";
import { Event_get_Encoder, Event_get_Decoder } from "./shared/Domain.js";
import { reject } from "./.fable/Fable.Promise.2.1.0/Promise.fs.js";
import { singleton, concat, empty, map as map_1, ofArray } from "./.fable/fable-library.3.1.1/List.js";
import { parse } from "./.fable/fable-library.3.1.1/Int32.js";
import { Types_HttpRequestHeaders, Types_RequestProperties, fetch$ } from "./.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { keyValueList } from "./.fable/fable-library.3.1.1/MapUtil.js";
import { authHeader, subscriptionHeader } from "./Common.js";
import { list as list_1, toString } from "./.fable/Thoth.Json.4.1.0/Encode.fs.js";

const get$ = get$_1;

const set$ = set$_1;

const keys = keys_2;

const clear = clear_1;

const ronniesStore = new Store("ronnies.be","events");

function getLastEvent() {
    const pr = keys(ronniesStore);
    return pr.then(((keys_1) => {
        if (keys_1.length === 0) {
            return void 0;
        }
        else {
            return max(keys_1, {
                Compare: comparePrimitives,
            });
        }
    }));
}

export function addEvent(version, event) {
    return set$(version, event, ronniesStore);
}

export function getAllEvents() {
    const pr_3 = keys(ronniesStore);
    return pr_3.then(((keys_1) => {
        let pr_2;
        const pr_1 = map((key) => {
            const pr = get$(key, ronniesStore);
            return pr.then(((evJson) => {
                const matchValue = fromValue("$root", uncurry(2, Event_get_Decoder()), evJson);
                if (matchValue.tag === 1) {
                    return reject(matchValue.fields[0]);
                }
                else {
                    return Promise.resolve(matchValue.fields[0]);
                }
            }));
        }, keys_1);
        pr_2 = (Promise.all(pr_1));
        return pr_2.then((ofArray));
    }));
}

function addEventsToIdb(response) {
    const pr_3 = response.text();
    return pr_3.then(((json) => {
        let decoder;
        const result = fromString(uncurry(2, (decoder = Event_get_Decoder(), (path) => ((value) => keyValuePairs(uncurry(2, decoder), path, value)))), json);
        if (result.tag === 1) {
            return reject(result.fields[0]);
        }
        else {
            const events = result.fields[0];
            let persistEventsPromise;
            let pr_1;
            const pr = map_1((tupledArg) => addEvent(parse(tupledArg[0], 511, false, 32), Event_get_Encoder()(tupledArg[1])), events);
            pr_1 = (Promise.all(pr));
            persistEventsPromise = (pr_1.then(((_arg1) => empty())));
            const newEventsPromise = Promise.resolve(map_1((tuple) => tuple[1], events));
            const pr_2 = Promise.all([newEventsPromise, persistEventsPromise]);
            return pr_2.then((concat));
        }
    }));
}

export function syncLatestEvents() {
    let pr_1;
    const pr = getLastEvent();
    pr_1 = (pr.then(((lastEvent) => {
        let arg10_1, id, arg10;
        return fetch$((lastEvent == null) ? (arg10_1 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/events"))(arg10_1)) : (id = (lastEvent | 0), (arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/events?lastEvent=%i"))(arg10)(id))), singleton(new Types_RequestProperties(1, keyValueList([new Types_HttpRequestHeaders(11, "application/json"), subscriptionHeader], 0))));
    })));
    return pr_1.then((addEventsToIdb));
}

export function persistEvents(events, authToken) {
    let arg10, headers;
    const pr = fetch$((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/events"))(arg10)), ofArray([new Types_RequestProperties(0, "POST"), new Types_RequestProperties(2, toString(4, list_1(map_1(Event_get_Encoder(), events)))), (headers = ofArray([new Types_HttpRequestHeaders(11, "application/json"), authHeader(authToken), subscriptionHeader]), new Types_RequestProperties(1, keyValueList(headers, 0)))]));
    return pr.then((addEventsToIdb));
}

export function removeAllEvents() {
    clear(ronniesStore);
}

