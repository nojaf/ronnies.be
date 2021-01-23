import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { Union, Record } from "../.fable/fable-library.3.1.1/Types.js";
import { union_type, record_type, bool_type, class_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { ofSeq, singleton, ofArray, sortBy, empty, cons, map, filter, fold } from "../.fable/fable-library.3.1.1/List.js";
import { Currency_Read_6D69B0C0, NonEmptyString_Read_10C27AE8, Identifier_Read_4C3C6BC4 } from "../shared/Domain.js";
import { authHeader, subscriptionHeader, readCurrency } from "../Common.js";
import { getTicks, toString } from "../.fable/fable-library.3.1.1/Date.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505, useReact_useMemo_CF4EA67, useReact_useContext_37FA55CF } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "../Components/EventContext.js";
import { uncurry, comparePrimitives, compareArrays } from "../.fable/fable-library.3.1.1/Util.js";
import { keyValuePairs, fromString, string, object } from "../.fable/Thoth.Json.4.1.0/Decode.fs.js";
import { tryFind, ofList, empty as empty_1 } from "../.fable/fable-library.3.1.1/Map.js";
import { RolesHook__get_IsEditorOrAdmin, useRoles, useAuth0 } from "../Auth0.js";
import { Types_RequestProperties, Types_HttpRequestHeaders, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { map as map_1, some } from "../.fable/fable-library.3.1.1/Option.js";
import { errorToast } from "../ReactToastify.js";
import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { classNames } from "../Styles.js";
import { empty as empty_2, singleton as singleton_1, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { To, Link } from "../Components/Navigation.js";
import { page } from "../Components/Page.js";

class Location extends Record {
    constructor(Id, Name, Price, PriceValue, Creator, Date$, Ticks, NoLongerSellsRonnies) {
        super();
        this.Id = Id;
        this.Name = Name;
        this.Price = Price;
        this.PriceValue = PriceValue;
        this.Creator = Creator;
        this.Date = Date$;
        this.Ticks = Ticks;
        this.NoLongerSellsRonnies = NoLongerSellsRonnies;
    }
}

function Location$reflection() {
    return record_type("Ronnies.Client.Pages.Overview.Location", [], Location, () => [["Id", string_type], ["Name", string_type], ["Price", string_type], ["PriceValue", class_type("System.Decimal")], ["Creator", string_type], ["Date", string_type], ["Ticks", class_type("System.Int64")], ["NoLongerSellsRonnies", bool_type]]);
}

function getLocations(events) {
    return fold((acc, event) => {
        let copyOfStruct;
        switch (event.tag) {
            case 1: {
                let id_1;
                let copyOfStruct_1 = Identifier_Read_4C3C6BC4(event.fields[0]);
                id_1 = copyOfStruct_1;
                return filter((l) => (l.Id !== id_1), acc);
            }
            case 2: {
                let id_3;
                let copyOfStruct_2 = Identifier_Read_4C3C6BC4(event.fields[0]);
                id_3 = copyOfStruct_2;
                return map((l_1) => ((l_1.Id === id_3) ? (new Location(l_1.Id, l_1.Name, l_1.Price, l_1.PriceValue, l_1.Creator, l_1.Date, l_1.Ticks, true)) : l_1), acc);
            }
            default: {
                const la = event.fields[0];
                return cons(new Location((copyOfStruct = Identifier_Read_4C3C6BC4(la.Id), copyOfStruct), NonEmptyString_Read_10C27AE8(la.Name), readCurrency(la.Price), Currency_Read_6D69B0C0(la.Price)[0], NonEmptyString_Read_10C27AE8(la.Creator), toString(la.Created, "dd/MM/yy"), getTicks(la.Created), false), acc);
            }
        }
    }, empty(), events);
}

function useLocations() {
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    return useReact_useMemo_CF4EA67(() => getLocations(eventCtx.Events), [eventCtx.Events]);
}

class SortBy extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Name", "Price", "Date"];
    }
}

function SortBy$reflection() {
    return union_type("Ronnies.Client.Pages.Overview.SortBy", [], SortBy, () => [[], [], []]);
}

function sortFn(sort) {
    switch (sort.tag) {
        case 1: {
            return (list_1) => sortBy((l_1) => [l_1.PriceValue, l_1.Name], list_1, {
                Compare: compareArrays,
            });
        }
        case 2: {
            return (list_2) => sortBy((l_2) => [l_2.Ticks, l_2.Name], list_2, {
                Compare: compareArrays,
            });
        }
        default: {
            return (list) => sortBy((l) => l.Name, list, {
                Compare: comparePrimitives,
            });
        }
    }
}

const nameDecoder = (path_1) => ((v) => object((get$) => get$.Required.Field("name", string), path_1, v));

function useGetUsers() {
    const patternInput = useFeliz_React__React_useState_Static_1505(empty_1());
    const auth0 = useAuth0();
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
                const result = fromString((path, value) => keyValuePairs(uncurry(2, nameDecoder), path, value), json);
                if (result.tag === 1) {
                    console.error(some(result.fields[0]));
                }
                else {
                    patternInput[1](ofList(result.fields[0]));
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

function OverviewPage() {
    const locations = useLocations();
    const patternInput = useFeliz_React__React_useState_Static_1505(new SortBy(0));
    const setSort = patternInput[1];
    const roles = useRoles();
    const users = useGetUsers();
    const locationRows = map((loc) => {
        const creator_1 = map_1((creator) => react.createElement("td", keyValueList([classNames(["text-right", "text-sm-left"])], 1), creator), tryFind(loc.Creator, users));
        return react.createElement("tr", {
            key: loc.Id,
        }, ...ofSeq(delay(() => append(singleton_1(react.createElement("td", {}, Link([To(toText(printf("/detail/%s"))(loc.Id)), classNames(ofSeq(delay(() => (loc.NoLongerSellsRonnies ? ["strike", "text-muted"] : empty_2()))))], [loc.Name]))), delay(() => append(singleton_1(react.createElement("td", {}, loc.Price)), delay(() => append(singleton_1(react.createElement("td", keyValueList([classNames(["text-center", "text-sm-left"])], 1), loc.Date)), delay(() => {
            let o;
            return RolesHook__get_IsEditorOrAdmin(roles) ? singleton_1((o = creator_1, (o == null) ? null : o)) : empty_2();
        })))))))));
    }, sortFn(patternInput[0])(locations));
    return page([], [react.createElement("h1", {}, "Overzicht"), react.createElement("table", keyValueList([classNames(["table", "table-striped"])], 1), react.createElement("thead", {}, react.createElement("tr", keyValueList([classNames(["text-primary", "pointer"])], 1), ...ofSeq(delay(() => append(singleton_1(react.createElement("th", {
        onClick: (_arg1) => {
            setSort(new SortBy(0));
        },
    }, "Naam")), delay(() => append(singleton_1(react.createElement("th", {
        onClick: (_arg2) => {
            setSort(new SortBy(1));
        },
    }, "Prijs")), delay(() => append(singleton_1(react.createElement("th", {
        onClick: (_arg3) => {
            setSort(new SortBy(2));
        },
    }, "Datum toegevoegd")), delay(() => (RolesHook__get_IsEditorOrAdmin(roles) ? singleton_1(react.createElement("th", {}, "Door")) : empty_2()))))))))))), react.createElement("tbody", {
        className: "overview-tbody",
    }, Array.from(locationRows)))]);
}

export default (() => createElement(OverviewPage, null));

