import { ofSeq, groupBy, map, fold, singleton, empty as empty_1, ofArray } from "../.fable/fable-library.3.1.1/List.js";
import { Union, Record } from "../.fable/fable-library.3.1.1/Types.js";
import { union_type, record_type, class_type, list_type, bool_type, float64_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { ofList, tryFind, containsKey, remove, add, empty } from "../.fable/fable-library.3.1.1/Map.js";
import { Cmd_ofSub, Cmd_none } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { now } from "../.fable/fable-library.3.1.1/DateOffset.js";
import { join, printf, toConsole, isNullOrWhiteSpace } from "../.fable/fable-library.3.1.1/String.js";
import { Event$, Identifier_Create, AddLocation_Parse } from "../shared/Domain.js";
import { stringHash } from "../.fable/fable-library.3.1.1/Util.js";
import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { useReact_useEffect_Z101E1A95, useFeliz_React__React_useState_Static_1505, useReact_useContext_37FA55CF } from "../.fable/Feliz.1.31.1/React.fs.js";
import { eventContext } from "../Components/EventContext.js";
import { useNavigate } from "../Components/Navigation.js";
import { RolesHook__get_IsEditorOrAdmin, useRoles, useAuth0 } from "../Auth0.js";
import { Feliz_React__React_useElmish_Static_645B1FB7 } from "../.fable/Feliz.UseElmish.1.2.2/UseElmish.fs.js";
import { HTMLAttr, DOMAttr } from "../.fable/Fable.React.7.2.0/Fable.React.Props.fs.js";
import { Browser_Types_Event__Event_get_Value } from "../.fable/Fable.React.7.2.0/Fable.React.Extensions.fs.js";
import { map as map_1 } from "../.fable/fable-library.3.1.1/Option.js";
import { keyValueList } from "../.fable/fable-library.3.1.1/MapUtil.js";
import { classNames } from "../Styles.js";
import { page } from "../Components/Page.js";
import { empty as empty_2, singleton as singleton_1, append, delay } from "../.fable/fable-library.3.1.1/Seq.js";
import { loading } from "../Components/Loading.js";
import { LocationPicker } from "../Components/LocationPicker.js";
import { SwitchProps, Switch } from "../Components/Switch.js";

const currencies = ofArray([["EUR", "Euro"], ["AED", "VAE-dirham"], ["AFN", "Afghani"], ["ALL", "Albanese lek"], ["AMD", "Armeense dram"], ["ANG", "Antilliaanse gulden"], ["AOA", "Angolese kwanza"], ["ARS", "Argentijnse peso"], ["AUD", "Australische dollar"], ["AWG", "Arubaanse florin"], ["AZN", "Azerbeidzjaanse manat"], ["BAM", "Bosnische inwisselbare mark"], ["BBD", "Barbadiaanse dollar"], ["BDT", "Bengalese taka"], ["BGN", "Nieuwe Bulgaarse lev (sinds 1999)"], ["BHD", "Bahreinse dinar"], ["BIF", "Burundese frank"], ["BMD", "Bermudaanse dollar"], ["BND", "Bruneise dollar"], ["BOB", "Boliviaanse boliviano"], ["BOV", "Boliviaanse Mvdol (aandelen)"], ["BRL", "Braziliaanse real"], ["BSD", "Bahamaanse dollar"], ["BTN", "Bhutaanse ngultrum"], ["BWP", "Botswaanse pula"], ["BYN", "Wit-Russische roebel"], ["BZD", "Belizaanse dollar"], ["CAD", "Canadese dollar"], ["CDF", "Congolese frank"], ["CHF", "Zwitserse frank"], ["CLF", "Chileense Unidades de fomento (aandelen)"], ["CLP", "Chileense peso"], ["CNY", "Chinese renminbi"], ["COP", "Colombiaanse peso"], ["COU", "Colombiaanse unidad de valor real"], ["CRC", "Costa Ricaanse colon"], ["CUC", "Convertibele peso (munteenheid voor toeristen in Cuba)"], ["CUP", "Cubaanse peso"], ["CVE", "Kaapverdische escudo"], ["CZK", "Tsjechische kroon"], ["DJF", "Djiboutiaanse frank"], ["DKK", "Deense kroon"], ["DOP", "Dominicaanse peso"], ["DZD", "Algerijnse dinar"], ["EEK", "Estische kroon"], ["EGP", "Egyptisch pond"], ["ERN", "Eritrese nakfa"], ["ETB", "Ethiopische birr"], ["FJD", "Fiji-dollar"], ["FKP", "Falklandeilands pond"], ["GBP", "Brits pond sterling"], ["GEL", "Georgische lari"], ["GHS", "Ghanese cedi (Geldig per 1 juli 2007, 1 GHS = 10.000 GHC)"], ["GIP", "Gibraltarees pond"], ["GMD", "Gambiaanse dalasi"], ["GNF", "Guineese frank"], ["GTQ", "Guatemalaanse quetzal"], ["GYD", "Guyaanse dollar"], ["HKD", "Hongkongse dollar"], ["HNL", "Hondurese lempira"], ["HRK", "Kroatische kuna"], ["HTG", "Haïtiaanse gourde"], ["HUF", "Hongaarse forint"], ["IDR", "Indonesische roepia"], ["ILS", "Nieuwe Israëlische sjekel"], ["INR", "Indiase roepie"], ["IQD", "Iraakse dinar"], ["IRR", "Iraanse rial"], ["ISK", "IJslandse kroon"], ["JMD", "Jamaicaanse dollar"], ["JOD", "Jordaanse dinar"], ["JPY", "Japanse yen"], ["KES", "Keniaanse shilling"], ["KGS", "Kirgizische som"], ["KHR", "Cambodjaanse riel"], ["KMF", "Comorese frank"], ["KPW", "Noord-Koreaanse won"], ["KRW", "Zuid-Koreaanse won"], ["KWD", "Koeweitse dinar"], ["KYD", "Kaaimaneilandse dollar"], ["KZT", "Kazachse tenge"], ["LAK", "Laotiaanse kip"], ["LBP", "Libanees pond"], ["LKR", "Sri Lankaanse roepie"], ["LRD", "Liberiaanse dollar"], ["LSL", "Lesothaanse loti"], ["LYD", "Libische dinar"], ["MAD", "Marokkaanse dirham"], ["MDL", "Moldavische leu"], ["MGA", "Malagassische ariary"], ["MKD", "Macedonische denar"], ["MMK", "Myanmarese kyat"], ["MNT", "Mongolische tugrik"], ["MOP", "Macause pataca"], ["MRU", "Mauritaanse ouguiya"], ["MUR", "Mauritiaanse roepie"], ["MVR", "Maldivische rufiyaa"], ["MWK", "Malawische kwacha"], ["MXN", "Mexicaanse peso"], ["MXV", "Mexicaanse Unidad de Inversion (UDI) (Funds code)"], ["MYR", "Maleisische ringgit"], ["MZN", "Mozambikaanse metical"], ["NAD", "Namibische dollar"], ["NGN", "Nigeriaanse naira"], ["NIO", "Nicaraguaanse córdoba Oro"], ["NOK", "Noorse kroon"], ["NPR", "Nepalese roepie"], ["NZD", "Nieuw-Zeelandse dollar"], ["OMR", "Omaanse rial"], ["PAB", "Panamese balboa"], ["PEN", "Nieuwe Peruviaanse sol"], ["PGK", "Papoease kina"], ["PHP", "Filipijnse peso"], ["PKR", "Pakistaanse roepie"], ["PLN", "Poolse zloty"], ["PYG", "Paraguayaanse guaraní"], ["QAR", "Qatarese rial"], ["RON", "Roemeense leu"], ["RSD", "Servische dinar"], ["RUB", "Russische roebel"], ["RWF", "Rwandese frank"], ["SAR", "Saoedi-Arabische riyal"], ["SBD", "Salomon-dollar"], ["SCR", "Seychelse roepie"], ["SDG", "Soedanees pond"], ["SEK", "Zweedse kroon"], ["SGD", "Singaporese dollar"], ["SHP", "Sint-Heleens pond"], ["SLL", "Sierra Leoonse leone"], ["SOS", "Somalische shilling"], ["SSP", "Zuid-Soedanees pond"], ["SRD", "Surinaamse dollar"], ["STN", "Santomese dobra"], ["SYP", "Syrisch pond"], ["SZL", "Swazische lilangeni"], ["THB", "Thaise baht"], ["TJS", "Tadzjiekse somoni"], ["TMT", "Turkmeense manat"], ["TND", "Tunesische dinar"], ["TOP", "Tongaanse pa\u0027anga"], ["TRY", "Nieuwe Turkse lira (sinds 1 januari 2005)"], ["TTD", "Trinidad en Tobagodollar"], ["TWD", "Nieuwe Taiwanese dollar"], ["TZS", "Tanzaniaanse shilling"], ["UAH", "Oekraïense grivna"], ["UGX", "Oegandese shilling"], ["USD", "Amerikaanse dollar"], ["USN", "Amerikaanse dollar (Next day) (aandelen)"], ["USS", "Amerikaanse dollar (Same day) (aandelen)"], ["UYU", "Uruguayaanse peso"], ["UZS", "Oezbeekse sum"], ["VEF", "Venezolaanse bolivar"], ["VND", "Vietnamese dong"], ["VUV", "Vanuatuaanse vatu"], ["WST", "Samoaanse tala"], ["XAF", "CFA-frank BEAC"], ["XCD", "Oost-Caribische dollar"], ["XPD", "1 Troy ounce palladium"], ["XPF", "CFP-frank"], ["XPT", "1 Troy ounce platina"], ["YER", "Jemenitische rial"], ["ZAR", "Zuid-Afrikaanse rand"], ["ZMW", "Zambiaanse kwacha"]]);

class Model extends Record {
    constructor(Name, Price, Currency, Latitude, Longitude, IsDraft, Remark, Errors) {
        super();
        this.Name = Name;
        this.Price = Price;
        this.Currency = Currency;
        this.Latitude = Latitude;
        this.Longitude = Longitude;
        this.IsDraft = IsDraft;
        this.Remark = Remark;
        this.Errors = Errors;
    }
}

function Model$reflection() {
    return record_type("Ronnies.Client.Views.AddLocationPage.View.Model", [], Model, () => [["Name", string_type], ["Price", string_type], ["Currency", string_type], ["Latitude", float64_type], ["Longitude", float64_type], ["IsDraft", bool_type], ["Remark", string_type], ["Errors", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, list_type(string_type)])]]);
}

function init(_arg1) {
    return [new Model("", "", "EUR", 0, 0, false, "", empty()), empty_1()];
}

class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["UpdateName", "UpdatePrice", "UpdateCurrency", "UpdateLocation", "UpdateIsDraft", "UpdateRemark", "UpdateLocationError", "Submit"];
    }
}

function Msg$reflection() {
    return union_type("Ronnies.Client.Views.AddLocationPage.View.Msg", [], Msg, () => [[["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item1", float64_type], ["Item2", float64_type]], [["Item", bool_type]], [["Item", string_type]], [["isError", bool_type]], [["userId", string_type]]]);
}

function update(onSubmit, msg, model) {
    let matchValue;
    if (msg.tag === 1) {
        return [new Model(model.Name, msg.fields[0], model.Currency, model.Latitude, model.Longitude, model.IsDraft, model.Remark, model.Errors), Cmd_none()];
    }
    else if (msg.tag === 2) {
        return [new Model(model.Name, model.Price, msg.fields[0], model.Latitude, model.Longitude, model.IsDraft, model.Remark, model.Errors), Cmd_none()];
    }
    else if (msg.tag === 3) {
        return [new Model(model.Name, model.Price, model.Currency, msg.fields[0], msg.fields[1], model.IsDraft, model.Remark, model.Errors), Cmd_none()];
    }
    else if (msg.tag === 4) {
        return [new Model(model.Name, model.Price, model.Currency, model.Latitude, model.Longitude, msg.fields[0], model.Remark, model.Errors), Cmd_none()];
    }
    else if (msg.tag === 5) {
        return [new Model(model.Name, model.Price, model.Currency, model.Latitude, model.Longitude, model.IsDraft, msg.fields[0], model.Errors), Cmd_none()];
    }
    else if (msg.tag === 6) {
        return [new Model(model.Name, model.Price, model.Currency, model.Latitude, model.Longitude, model.IsDraft, model.Remark, msg.fields[0] ? add("distance", singleton("De gekozen locatie is te ver van jouw locatie! Das de bedoeling niet veugel."), model.Errors) : remove("distance", model.Errors)), Cmd_none()];
    }
    else if (msg.tag === 7) {
        if (containsKey("distance", model.Errors)) {
            return [model, Cmd_none()];
        }
        else if (msg.tag === 7) {
            let result;
            const arg80 = now();
            const arg70 = isNullOrWhiteSpace(model.Remark) ? (void 0) : model.Remark;
            result = AddLocation_Parse(Identifier_Create(), model.Name, model.Latitude, model.Longitude, model.Price, model.Currency, model.IsDraft, arg70, arg80, msg.fields[0]);
            if (result.tag === 1) {
                return [new Model(model.Name, model.Price, model.Currency, model.Latitude, model.Longitude, model.IsDraft, model.Remark, fold((acc, tupledArg) => add(tupledArg[0], map((arg) => {
                    const errorType = arg[1];
                    switch (errorType.tag) {
                        case 0:
                        case 1: {
                            return "Toch iets raars met je locatie";
                        }
                        case 2: {
                            return "Veld mag niet leeg zijn";
                        }
                        case 5:
                        case 4: {
                            return "Geen geldige prijs";
                        }
                        default: {
                            return "Hmm, bugje in de software";
                        }
                    }
                }, tupledArg[1]), acc), (matchValue = tryFind("distance", model.Errors), (matchValue == null) ? empty() : ofList(singleton(["distance", matchValue]))), groupBy((tuple) => tuple[0], result.fields[0], {
                    Equals: (x, y) => (x === y),
                    GetHashCode: stringHash,
                }))), Cmd_none()];
            }
            else {
                const locationAdded = result.fields[0];
                toConsole(printf("valid location %A"))(locationAdded);
                return [new Model(model.Name, model.Price, model.Currency, model.Latitude, model.Longitude, model.IsDraft, model.Remark, empty()), Cmd_ofSub((_arg1) => {
                    onSubmit(locationAdded);
                })];
            }
        }
        else {
            throw (new Error("The match cases were incomplete"));
        }
    }
    else {
        return [new Model(msg.fields[0], model.Price, model.Currency, model.Latitude, model.Longitude, model.IsDraft, model.Remark, model.Errors), Cmd_none()];
    }
}

function mapToCurrencyItem(currencyCode, description) {
    return react.createElement("option", {
        className: "dropdown-item",
        key: currencyCode,
        value: currencyCode,
        title: description,
    }, currencyCode);
}

function deg2rad(deg) {
    return (deg * 3.141592653589793) / 180;
}

function rad2deg(rad) {
    return (rad / 3.141592653589793) * 180;
}

function distanceBetweenTwoPoints(latA, lngA, latB, lngB) {
    if ((latA === latB) ? (lngA === lngB) : false) {
        return 0;
    }
    else {
        const theta = lngA - lngB;
        return ((60 * 1.1515) * 1.609344) * rad2deg(Math.acos((Math.sin(deg2rad(latA)) * Math.sin(deg2rad(latB))) + ((Math.cos(deg2rad(latA)) * Math.cos(deg2rad(latB))) * Math.cos(deg2rad(theta)))));
    }
}

function AddLocationPage() {
    const eventCtx = useReact_useContext_37FA55CF(eventContext);
    const navigate = useNavigate();
    const patternInput = useFeliz_React__React_useState_Static_1505(false);
    const auth0 = useAuth0();
    const patternInput_1 = useFeliz_React__React_useState_Static_1505("");
    const roles = useRoles();
    useReact_useEffect_Z101E1A95(() => {
        if (!(auth0.user == null)) {
            patternInput_1[1](auth0.user.sub);
        }
    }, [auth0.user]);
    const patternInput_2 = Feliz_React__React_useElmish_Static_645B1FB7(init, (msg, model) => update((addEvent) => {
        patternInput[1](true);
        const pr = eventCtx.AddEvents(singleton(new Event$(0, addEvent)));
        pr.then((() => {
            navigate("/");
        }));
    }, msg, model), new Array(0));
    const model_1 = patternInput_2[0];
    const dispatch = patternInput_2[1];
    const updateOnChange = (msg_1) => (new DOMAttr(9, (ev) => {
        dispatch(msg_1(Browser_Types_Event__Event_get_Value(ev)));
    }));
    const locationError = map_1((errors) => react.createElement("div", keyValueList([classNames(["alert", "alert-danger"])], 1), join("\n", errors)), tryFind("distance", model_1.Errors));
    const hasErrors = (key) => containsKey(key, model_1.Errors);
    const inputErrors = (key_1) => {
        const o_1 = map_1((errors_1) => react.createElement("div", {
            className: "invalid-feedback",
        }, join("\n", errors_1)), tryFind(key_1, model_1.Errors));
        if (o_1 == null) {
            return null;
        }
        else {
            return o_1;
        }
    };
    return page([], ofSeq(delay(() => append(singleton_1(react.createElement("h1", {}, "E nieuwen toevoegen")), delay(() => {
        let o_3;
        return patternInput[0] ? singleton_1(loading("ant opslaan...")) : ((!RolesHook__get_IsEditorOrAdmin(roles)) ? singleton_1("Sorry, je bent geen patron") : singleton_1(react.createElement("form", keyValueList([classNames(["col-md-6", "p-0"]), new DOMAttr(11, (ev_1) => {
            ev_1.preventDefault();
            dispatch(new Msg(7, patternInput_1[0]));
        })], 1), react.createElement("div", {
            className: "form-group",
        }, react.createElement("label", {}, "Naam*"), react.createElement("input", keyValueList([classNames(ofSeq(delay(() => append(singleton_1("form-control"), delay(() => (hasErrors("name") ? singleton_1("is-invalid") : empty_2())))))), new HTMLAttr(1, model_1.Name), updateOnChange((arg0_2) => (new Msg(0, arg0_2)))], 1)), inputErrors("name")), react.createElement("div", {
            className: "form-group",
        }, react.createElement("label", {}, "Prijs*"), react.createElement("div", {
            className: "input-group",
        }, react.createElement("input", keyValueList([classNames(ofSeq(delay(() => append(singleton_1("form-control"), delay(() => (hasErrors("price") ? singleton_1("is-invalid") : empty_2())))))), new HTMLAttr(159, "number"), new HTMLAttr(154, "0.01"), new HTMLAttr(1, model_1.Price), updateOnChange((arg0_3) => (new Msg(1, arg0_3)))], 1)), react.createElement("div", {
            className: "input-group-append",
        }, react.createElement("select", keyValueList([classNames(["custom-select"]), updateOnChange((arg0_4) => (new Msg(2, arg0_4))), ["style", {
            background: "none",
            borderTopLeftRadius: "0",
            borderBottomLeftRadius: "0",
        }]], 1), Array.from(map((tupledArg_1) => mapToCurrencyItem(tupledArg_1[0], tupledArg_1[1]), currencies)))), inputErrors("price"))), react.createElement("div", {}, react.createElement("label", {}, "Locatie*"), react.createElement("br", {}), react.createElement("div", {
            id: "locationPickerContainer",
        }, createElement(LocationPicker, {
            ExistingLocations: empty_1(),
            OnChange: (userLocation, ronnyLocation) => {
                let tupledArg;
                dispatch((tupledArg = ronnyLocation, new Msg(3, tupledArg[0], tupledArg[1])));
                dispatch(new Msg(6, distanceBetweenTwoPoints(userLocation[0], userLocation[1], ronnyLocation[0], ronnyLocation[1]) > 0.25));
            },
        })), (o_3 = locationError, (o_3 == null) ? null : o_3)), react.createElement("div", {
            className: "form-group",
        }, react.createElement("label", {}, "Ist van vat?"), react.createElement("br", {}), Switch(new SwitchProps("Joat", "Nint", (arg) => {
            dispatch(new Msg(4, arg));
        }, model_1.IsDraft, false))), react.createElement("div", {
            className: "form-group",
        }, react.createElement("label", {}, "Opmerking"), react.createElement("textarea", keyValueList([new HTMLAttr(64, "form-control"), new HTMLAttr(1, model_1.Remark), updateOnChange((arg0_6) => (new Msg(5, arg0_6)))], 1))), react.createElement("div", keyValueList([classNames(["text-right", "pb-2"])], 1), react.createElement("button", keyValueList([classNames(["btn", "btn-primary"])], 1), "Save!")))));
    })))));
}

export default (() => createElement(AddLocationPage, null));

