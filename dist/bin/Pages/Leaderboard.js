import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, int32_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { keyValuePairs, fromString, string, object } from "../.fable/Thoth.Json.4.1.0/Decode.fs.js";
import { tryFind, empty, find, add, containsKey, map } from "../.fable/fable-library.3.1.1/Map.js";
import { map as map_1, choose, sortByDescending, ofArray, empty as empty_1, filter, singleton, cons, fold, length } from "../.fable/fable-library.3.1.1/List.js";
import { NonEmptyString_Read_10C27AE8 } from "../shared/Domain.js";
import { comparePrimitives, uncurry, equals } from "../.fable/fable-library.3.1.1/Util.js";
import { useAuth0 } from "../Auth0.js";
import { useReact_useEffect_Z101E1A95, useReact_useMemo_CF4EA67, useReact_useContext_37FA55CF, useFeliz_React__React_useState_Static_1505 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "../Components/EventContext.js";
import { Types_RequestProperties, Types_HttpRequestHeaders, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { authHeader, subscriptionHeader } from "../Common.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { defaultArg, some } from "../.fable/fable-library.3.1.1/Option.js";
import { errorToast } from "../ReactToastify.js";
import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { page } from "../Components/Page.js";
import { classNames } from "../Styles.js";

class User extends Record {
    constructor(Name, Picture, Score) {
        super();
        this.Name = Name;
        this.Picture = Picture;
        this.Score = (Score | 0);
    }
}

function User$reflection() {
    return record_type("Ronnies.Client.Pages.Leaderboard.User", [], User, () => [["Name", string_type], ["Picture", string_type], ["Score", int32_type]]);
}

const userDecoder = (path_2) => ((v) => object((get$) => (new User(get$.Required.Field("name", string), get$.Required.Field("picture", string), 0)), path_2, v));

export function getScores(events) {
    return map((_arg2, v_1) => length(v_1), fold((acc, ev) => {
        let pattern_matching_result, id;
        switch (ev.tag) {
            case 1: {
                pattern_matching_result = 1;
                id = ev.fields[0];
                break;
            }
            case 2: {
                pattern_matching_result = 1;
                id = ev.fields[0];
                break;
            }
            default: pattern_matching_result = 0}
        switch (pattern_matching_result) {
            case 0: {
                const la = ev.fields[0];
                const creator = NonEmptyString_Read_10C27AE8(la.Creator);
                return containsKey(creator, acc) ? add(creator, cons(la.Id, find(creator, acc)), acc) : add(creator, singleton(la.Id), acc);
            }
            case 1: {
                return map((_arg1, locations) => filter((y) => (!equals(id, y)), locations), acc);
            }
        }
    }, empty(), events));
}

function useUserScore() {
    const auth0 = useAuth0();
    const patternInput = useFeliz_React__React_useState_Static_1505(empty_1());
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const scores = useReact_useMemo_CF4EA67(() => getScores(eventCtx.Events), [eventCtx.Events]);
    useReact_useEffect_Z101E1A95(() => {
        if (auth0.isAuthenticated) {
            let pr_3;
            let pr_2;
            let pr_1;
            const pr = auth0.getAccessTokenSilently();
            pr_1 = (pr.then(((token) => {
                let arg10, headers;
                return fetch$((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/users"))(arg10)), singleton((headers = ofArray([new Types_HttpRequestHeaders(11, "application/json"), subscriptionHeader, authHeader(token)]), new Types_RequestProperties(1, keyValueList(headers, 0)))));
            })));
            pr_2 = (pr_1.then(((res) => res.text())));
            pr_3 = (pr_2.then(((json) => {
                const result = fromString((path, value) => keyValuePairs(uncurry(2, userDecoder), path, value), json);
                if (result.tag === 1) {
                    console.error(some(result.fields[0]));
                }
                else {
                    patternInput[1](sortByDescending((u_1) => u_1.Score, choose((tupledArg) => {
                        const u = tupledArg[1];
                        const userScore = defaultArg(tryFind(tupledArg[0], scores), 0) | 0;
                        if (userScore > 0) {
                            return new User(u.Name, u.Picture, userScore);
                        }
                        else {
                            return void 0;
                        }
                    }, result.fields[0]), {
                        Compare: comparePrimitives,
                    }));
                }
            })));
            pr_3.then(void 0, ((err_1) => {
                console.error(some(err_1));
                errorToast("Kon de patrons niet ophalen");
            }));
        }
    }, [auth0.isAuthenticated]);
    return patternInput[0];
}

function LeaderboardPage() {
    const userRows = map_1((us) => react.createElement("tr", {
        key: us.Name,
    }, react.createElement("td", {}, react.createElement("img", {
        src: us.Picture,
        height: "30px",
        width: "30x",
    })), react.createElement("td", {}, us.Name), react.createElement("td", {}, react.createElement("strong", {}, us.Score))), useUserScore());
    return page([], [react.createElement("h1", {}, "Klassement"), react.createElement("table", keyValueList([classNames(["table", "table-striped", "table-light"])], 1), react.createElement("thead", {}, react.createElement("tr", {}, react.createElement("th", {}), react.createElement("th", {}, "Naam"), react.createElement("th", {}, "Score"))), react.createElement("tbody", {}, Array.from(userRows)))]);
}

export default (() => createElement(LeaderboardPage, null));

