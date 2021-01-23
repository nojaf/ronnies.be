import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, option_type, bool_type, string_type, class_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { Identifier_Parse_244AC511, Event$, Event$$reflection, NonEmptyString_Read_10C27AE8, Identifier_Read_4C3C6BC4 } from "../shared/Domain.js";
import { ofSeq, empty, singleton, ofArray, fold } from "../.fable/fable-library.3.1.1/List.js";
import { toString } from "../.fable/fable-library.3.1.1/Date.js";
import { subscriptionHeader, authHeader, readCurrency } from "../Common.js";
import { fromString, string, object } from "../.fable/Thoth.Json.4.1.0/Decode.fs.js";
import { parse } from "../.fable/fable-library.3.1.1/Guid.js";
import { useReact_useEffect_Z101E1A95, useReact_useMemo_CF4EA67, useFeliz_React__React_useState_Static_1505, useReact_useContext_37FA55CF } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "../Components/EventContext.js";
import { RolesHook__get_IsAdmin, useAuth0, useRoles, RolesHook__get_IsEditorOrAdmin } from "../Auth0.js";
import { replace, printf, toText, isNullOrWhiteSpace } from "../.fable/fable-library.3.1.1/String.js";
import { Types_RequestProperties, Types_HttpRequestHeaders, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { uncurry } from "../.fable/fable-library.3.1.1/Util.js";
import { map, some } from "../.fable/fable-library.3.1.1/Option.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { useNavigate } from "../Components/Navigation.js";
import { classNames } from "../Styles.js";
import { DOMAttr, DangerousHtml } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { successToast } from "../ReactToastify.js";
import { page } from "../Components/Page.js";
import { empty as empty_1, singleton as singleton_1, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { loading } from "../Components/Loading.js";

class LocationDetail extends Record {
    constructor(Id, Name, Creator, Created, Price, IsDraft, Remark, IsCancelled, NoLongerSellsRonnies) {
        super();
        this.Id = Id;
        this.Name = Name;
        this.Creator = Creator;
        this.Created = Created;
        this.Price = Price;
        this.IsDraft = IsDraft;
        this.Remark = Remark;
        this.IsCancelled = IsCancelled;
        this.NoLongerSellsRonnies = NoLongerSellsRonnies;
    }
}

function LocationDetail$reflection() {
    return record_type("Ronnies.Client.Pages.Detail.LocationDetail", [], LocationDetail, () => [["Id", class_type("System.Guid")], ["Name", string_type], ["Creator", string_type], ["Created", string_type], ["Price", string_type], ["IsDraft", bool_type], ["Remark", option_type(string_type)], ["IsCancelled", bool_type], ["NoLongerSellsRonnies", bool_type]]);
}

function LocationDetail_get_Empty() {
    return new LocationDetail("00000000-0000-0000-0000-000000000000", "???", "", "", "", false, void 0, false, false);
}

export function equalsId(id, value) {
    return value === Identifier_Read_4C3C6BC4(id);
}

function getLocation(events, id) {
    return fold((acc, event) => ((event.tag === 0) ? (equalsId(event.fields[0].Id, id) ? (new LocationDetail(id, NonEmptyString_Read_10C27AE8(event.fields[0].Name), NonEmptyString_Read_10C27AE8(event.fields[0].Creator), toString(event.fields[0].Created, "dd/MM/yy"), readCurrency(event.fields[0].Price), event.fields[0].IsDraft, event.fields[0].Remark, acc.IsCancelled, acc.NoLongerSellsRonnies)) : ((event.tag === 1) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, true, acc.NoLongerSellsRonnies)) : ((event.tag === 2) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, acc.IsCancelled, true)) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc)))) : ((event.tag === 2) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, acc.IsCancelled, true)) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))))) : ((event.tag === 1) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, true, acc.NoLongerSellsRonnies)) : ((event.tag === 2) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, acc.IsCancelled, true)) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc)))) : ((event.tag === 2) ? (equalsId(event.fields[0], id) ? (new LocationDetail(acc.Id, acc.Name, acc.Creator, acc.Created, acc.Price, acc.IsDraft, acc.Remark, acc.IsCancelled, true)) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))) : ((event.tag === 1) ? acc : ((event.tag === 2) ? acc : acc))))), LocationDetail_get_Empty(), events);
}

export const nameDecoder = (path_1) => ((v) => object((get$) => get$.Required.Field("name", string), path_1, v));

function useLocationDetail(auth0, roles, id) {
    const id_1 = parse(id);
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const patternInput = useFeliz_React__React_useState_Static_1505(void 0);
    const location = useReact_useMemo_CF4EA67(() => getLocation(eventCtx.Events, id_1), [eventCtx.Events, id_1]);
    useReact_useEffect_Z101E1A95(() => {
        if (RolesHook__get_IsEditorOrAdmin(roles) ? (!isNullOrWhiteSpace(location.Creator)) : false) {
            let pr_2;
            let pr_1;
            const pr = auth0.getAccessTokenSilently();
            pr_1 = (pr.then(((authToken) => {
                let arg10, headers;
                return fetch$((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_BACKEND), toText(printf("%s/users/%s"))(arg10)(location.Creator)), singleton((headers = ofArray([new Types_HttpRequestHeaders(11, "application/json"), authHeader(authToken), subscriptionHeader]), new Types_RequestProperties(1, keyValueList(headers, 0)))));
            })));
            pr_2 = (pr_1.then(((res) => res.text())));
            pr_2.then(((json) => {
                const usersResult = fromString(uncurry(2, nameDecoder), json);
                if (usersResult.tag === 1) {
                    console.log(some(usersResult.fields[0]));
                }
                else {
                    patternInput[1](usersResult.fields[0]);
                }
            }));
        }
    }, [roles.Roles, location.Creator]);
    return [location, patternInput[0]];
}

function fact(label, value) {
    return ofArray([react.createElement("dt", {
        className: "col-6",
    }, label), react.createElement("dd", {
        className: "col-6",
    }, value)]);
}

export class ActionModal extends Record {
    constructor(Title, Description, Event$, SuccessToastMessage, FailureToastMessage) {
        super();
        this.Title = Title;
        this.Description = Description;
        this.Event = Event$;
        this.SuccessToastMessage = SuccessToastMessage;
        this.FailureToastMessage = FailureToastMessage;
    }
}

export function ActionModal$reflection() {
    return record_type("Ronnies.Client.Pages.Detail.ActionModal", [], ActionModal, () => [["Title", string_type], ["Description", string_type], ["Event", Event$$reflection()], ["SuccessToastMessage", string_type], ["FailureToastMessage", string_type]]);
}

function DetailPage(props) {
    const roles = useRoles();
    const auth0 = useAuth0();
    const navigate = useNavigate();
    const patternInput = useLocationDetail(auth0, roles, props.id);
    const locationDetail = patternInput[0];
    const creatorName = patternInput[1];
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const patternInput_1 = useFeliz_React__React_useState_Static_1505(false);
    const setIsLoading = patternInput_1[1];
    const isLoading = patternInput_1[0];
    const patternInput_2 = useFeliz_React__React_useState_Static_1505(void 0);
    const setActionModal = patternInput_2[1];
    const isDraftValue = locationDetail.IsDraft ? "Joat" : "Nint";
    const creator = (creatorName == null) ? empty() : fact("Patron", creatorName);
    let remark_1;
    const matchValue = [creatorName, locationDetail.Remark];
    let pattern_matching_result, name_1, remark;
    if (matchValue[0] != null) {
        if (matchValue[1] != null) {
            pattern_matching_result = 0;
            name_1 = matchValue[0];
            remark = matchValue[1];
        }
        else {
            pattern_matching_result = 1;
        }
    }
    else {
        pattern_matching_result = 1;
    }
    switch (pattern_matching_result) {
        case 0: {
            remark_1 = react.createElement("blockquote", keyValueList([classNames(["blockquote", "text-center", "bg-light", "p-2", "mt-4"])], 1), react.createElement("p", keyValueList([classNames(["mb-0"])], 1), replace(remark, "\\n", "\n")), react.createElement("footer", {
                className: "blockquote-footer",
            }, react.createElement("cite", {}, name_1)));
            break;
        }
        case 1: {
            const o = void 0;
            remark_1 = ((o == null) ? null : o);
            break;
        }
    }
    const modalWindow = map((modalInfo_1) => react.createElement("div", {
        className: "modal fade show d-block",
        tabIndex: -1,
        role: "dialog",
    }, react.createElement("div", {
        className: "modal-dialog",
    }, react.createElement("div", {
        className: "modal-content",
    }, react.createElement("div", {
        className: "modal-header",
    }, react.createElement("h5", {
        className: "modal-title",
    }, modalInfo_1.Title), react.createElement("button", {
        type: "button",
        className: "close",
        onClick: (_arg2) => {
            setActionModal(void 0);
        },
    }, react.createElement("span", {
        dangerouslySetInnerHTML: new DangerousHtml("\u0026times;"),
    }))), react.createElement("div", {
        className: "modal-body",
    }, react.createElement("p", {}, modalInfo_1.Description)), react.createElement("div", {
        className: "modal-footer",
    }, react.createElement("button", {
        type: "button",
        className: "btn btn-secondary",
        onClick: (_arg3) => {
            setActionModal(void 0);
        },
    }, "Toe doen"), react.createElement("button", {
        type: "button",
        className: "btn btn-primary",
        onClick: (_arg4) => {
            const modalInfo = modalInfo_1;
            setActionModal(void 0);
            setIsLoading(true);
            let pr_1;
            const pr = eventCtx.AddEvents(singleton(modalInfo.Event));
            pr_1 = (pr.then((() => {
                successToast(modalInfo.SuccessToastMessage);
                setIsLoading(false);
                navigate("/");
            })));
            pr_1.then(void 0, ((err) => {
                console.error(some(err));
                setIsLoading(false);
            }));
        },
    }, "Bevestig"))))), patternInput_2[0]);
    return page([], ofSeq(delay(() => {
        let o_2;
        return append(singleton_1((o_2 = modalWindow, (o_2 == null) ? null : o_2)), delay(() => append(singleton_1(react.createElement("h1", keyValueList([classNames(["pb-4"])], 1), ...ofSeq(delay(() => (locationDetail.NoLongerSellsRonnies ? [react.createElement("span", {
            dangerouslySetInnerHTML: new DangerousHtml("\u0026#10014;"),
        }), react.createElement("strong", {}, "RIP "), locationDetail.Name] : singleton_1(locationDetail.Name)))))), delay(() => append(singleton_1(react.createElement("div", keyValueList([classNames(["row"])], 1), ...ofSeq(delay(() => append(fact("Prijs", locationDetail.Price), delay(() => append(fact("Vant vat?", isDraftValue), delay(() => append(fact("Toegevoegd op", locationDetail.Created), delay(() => creator)))))))))), delay(() => append(singleton_1(remark_1), delay(() => ((((!locationDetail.NoLongerSellsRonnies) ? RolesHook__get_IsEditorOrAdmin(roles) : false) ? (!isLoading) : false) ? singleton_1(react.createElement("div", {}, ...ofSeq(delay(() => append(singleton_1(react.createElement("hr", {})), delay(() => append(RolesHook__get_IsAdmin(roles) ? singleton_1(react.createElement("button", keyValueList([classNames(["btn", "btn-danger"]), new DOMAttr(40, (_arg1_1) => {
            setActionModal(new ActionModal("Cancel location", "Wil je deze plekke uitschakelen? Dit is echt vo aj je met een misse zit.\nToedoen aj kei zit.", new Event$(1, Identifier_Parse_244AC511(locationDetail.Id)), toText(printf("%s werd geannuleerd!"))(locationDetail.Name), toText(printf("Kon %s niet annuleren!"))(locationDetail.Name)));
        })], 1), "Cancel location")) : empty_1(), delay(() => append(RolesHook__get_IsAdmin(roles) ? singleton_1(react.createElement("br", {})) : empty_1(), delay(() => singleton_1(react.createElement("button", keyValueList([classNames(["btn", "btn-warning", "mt-2"]), new DOMAttr(40, (_arg2_1) => {
            setActionModal(new ActionModal(toText(printf("%s verkoopt gin ronnies meer."))(locationDetail.Name), "Ben je zeker dat ze hier gin ronnies meer verkopen?\nToedoen aj kei zit.", new Event$(2, Identifier_Parse_244AC511(locationDetail.Id)), toText(printf("%s werd gemarkt als ronnies plekke no more!"))(locationDetail.Name), toText(printf("Kon %s niet updaten!"))(locationDetail.Name)));
        })], 1), "Verkoopt gin ronnies nie meer")))))))))))) : (isLoading ? singleton_1(loading("Syncen met de server...")) : empty_1()))))))))));
    })));
}

export default (DetailPage);

