import { Record, FSharpRef, Union } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, option_type, bool_type, float64_type, class_type, string_type, list_type, union_type, int32_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { map, singleton, concat } from "../.fable/fable-library.3.1.1/List.js";
import { replace, isNullOrWhiteSpace, printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { map as map_1, index, datetimeOffset as datetimeOffset_1, bool, guid as guid_1, string, float, tuple2 as tuple2_1, object as object_1, succeed, andThen, fail } from "../.fable/Thoth.Json.4.1.0/Decode.fs.js";
import { tryParse, newGuid } from "../.fable/fable-library.3.1.1/Guid.js";
import { fromParts, compare, tryParse as tryParse_1 } from "../.fable/fable-library.3.1.1/Decimal.js";
import Decimal from "../.fable/fable-library.3.1.1/Decimal.js";
import { datetimeOffset, option, decimal, tuple2, guid, object } from "../.fable/Thoth.Json.4.1.0/Encode.fs.js";
import { uncurry } from "../.fable/fable-library.3.1.1/Util.js";

export class ValidationErrorType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["InvalidLatitude", "InvalidLongitude", "EmptyString", "InvalidStringLength", "InvalidNumber", "NegativeNumber", "InvalidGuidString", "EmptyGuid"];
    }
}

export function ValidationErrorType$reflection() {
    return union_type("Ronnies.Domain.ValidationErrorType", [], ValidationErrorType, () => [[], [], [], [["expected", int32_type], ["actual", int32_type]], [], [], [], []]);
}

function curry(f, a, b) {
    return f([a, b]);
}

export class ValidationResult$2 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Success", "Failure"];
    }
}

export function ValidationResult$2$reflection(gen0, gen1) {
    return union_type("Ronnies.Domain.ValidationResult`2", [gen0, gen1], ValidationResult$2, () => [[["Item", gen0]], [["Item", list_type(gen1)]]]);
}

export function ValidationResult_map(f, xResult) {
    if (xResult.tag === 1) {
        return new ValidationResult$2(1, xResult.fields[0]);
    }
    else {
        return new ValidationResult$2(0, f(xResult.fields[0]));
    }
}

export function ValidationResult_lift(x) {
    return new ValidationResult$2(0, x);
}

export function ValidationResult_apply(fResult, xResult) {
    const matchValue = [fResult, xResult];
    if (matchValue[0].tag === 1) {
        if (matchValue[1].tag === 1) {
            return new ValidationResult$2(1, concat([matchValue[0].fields[0], matchValue[1].fields[0]]));
        }
        else {
            return new ValidationResult$2(1, matchValue[0].fields[0]);
        }
    }
    else if (matchValue[1].tag === 1) {
        return new ValidationResult$2(1, matchValue[1].fields[0]);
    }
    else {
        return new ValidationResult$2(0, matchValue[0].fields[0](matchValue[1].fields[0]));
    }
}

export function ValidationResult_bind(f, xResult) {
    if (xResult.tag === 1) {
        return new ValidationResult$2(1, xResult.fields[0]);
    }
    else {
        return f(xResult.fields[0]);
    }
}

function op_LessBangGreater() {
    return (f) => ((xResult) => ValidationResult_map(f, xResult));
}

function op_LessMultiplyGreater() {
    return (fResult) => ((xResult) => ValidationResult_apply(fResult, xResult));
}

export function Decode_failWithDomainErrors(errors) {
    const msg = toText(printf("Invalid value according to the domain: %A"))(errors);
    return (path) => ((arg20$0040) => fail(msg, path, arg20$0040));
}

export class NonEmptyString extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["NonEmptyString"];
    }
}

export function NonEmptyString$reflection() {
    return union_type("Ronnies.Domain.NonEmptyString", [], NonEmptyString, () => [[["Item", string_type]]]);
}

export function NonEmptyString_Read_10C27AE8(_arg1) {
    return _arg1.fields[0];
}

export function NonEmptyString_Parse_Z721C83C5(v) {
    if (isNullOrWhiteSpace(v)) {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(2)));
    }
    else {
        return new ValidationResult$2(0, new NonEmptyString(0, v));
    }
}

export class Identifier extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Identifier"];
    }
}

export function Identifier$reflection() {
    return union_type("Ronnies.Domain.Identifier", [], Identifier, () => [[["Item", class_type("System.Guid")]]]);
}

export function Identifier_Create() {
    return new Identifier(0, newGuid());
}

export function Identifier_Read_4C3C6BC4(_arg1) {
    return _arg1.fields[0];
}

export function Identifier_Parse_Z721C83C5(v) {
    let matchValue;
    let outArg = "00000000-0000-0000-0000-000000000000";
    matchValue = [tryParse(v, new FSharpRef(() => outArg, (v_1) => {
        outArg = v_1;
    })), outArg];
    if (matchValue[0]) {
        return new ValidationResult$2(0, new Identifier(0, matchValue[1]));
    }
    else {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(6)));
    }
}

export function Identifier_Parse_244AC511(v) {
    return new Identifier(0, v);
}

export class Latitude extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Latitude"];
    }
}

export function Latitude$reflection() {
    return union_type("Ronnies.Domain.Latitude", [], Latitude, () => [[["Item", float64_type]]]);
}

export function Latitude_Read_71E327F7(_arg1) {
    return _arg1.fields[0];
}

export function Latitude_Parse_5E38073B(v) {
    if ((v >= -90) ? (v <= 90) : false) {
        return new ValidationResult$2(0, new Latitude(0, v));
    }
    else {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(0)));
    }
}

export class Longitude extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Longitude"];
    }
}

export function Longitude$reflection() {
    return union_type("Ronnies.Domain.Longitude", [], Longitude, () => [[["Item", float64_type]]]);
}

export function Longitude_Read_5F4D9B24(_arg1) {
    return _arg1.fields[0];
}

export function Longitude_Parse_5E38073B(v) {
    if ((v >= -180) ? (v <= 180) : false) {
        return new ValidationResult$2(0, new Longitude(0, v));
    }
    else {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(1)));
    }
}

export class Location extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Location"];
    }
}

export function Location$reflection() {
    return union_type("Ronnies.Domain.Location", [], Location, () => [[["Item1", Latitude$reflection()], ["Item2", Longitude$reflection()]]]);
}

export function Location_Read_44B20A7A(_arg1) {
    return [_arg1.fields[0].fields[0], _arg1.fields[1].fields[0]];
}

export function Location_Parse(lat, lng) {
    return op_LessMultiplyGreater()(op_LessBangGreater()((a) => ((b) => curry((tupledArg) => (new Location(0, tupledArg[0], tupledArg[1])), a, b)))(Latitude_Parse_5E38073B(lat)))(Longitude_Parse_5E38073B(lng));
}

export class ThreeLetterString extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["ThreeLetterString"];
    }
}

export function ThreeLetterString$reflection() {
    return union_type("Ronnies.Domain.ThreeLetterString", [], ThreeLetterString, () => [[["Item", string_type]]]);
}

export function ThreeLetterString_Read_301C3B42(_arg1) {
    return _arg1.fields[0];
}

export function ThreeLetterString_Parse_Z721C83C5(s) {
    if (isNullOrWhiteSpace(s)) {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(2)));
    }
    else if (s.length !== 3) {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(3, 3, s.length)));
    }
    else {
        return new ValidationResult$2(0, new ThreeLetterString(0, s));
    }
}

export class Currency extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Currency"];
    }
}

export function Currency$reflection() {
    return union_type("Ronnies.Domain.Currency", [], Currency, () => [[["Item1", class_type("System.Decimal")], ["Item2", ThreeLetterString$reflection()]]]);
}

export function Currency_Read_6D69B0C0(_arg1) {
    return [_arg1.fields[0], _arg1.fields[1].fields[0]];
}

export function Currency_Parse(value, currencyType) {
    let matchValue;
    let outArg = new Decimal(0);
    matchValue = [tryParse_1(replace(value, ",", "."), new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        const value_1 = matchValue[1];
        if (compare(value_1, fromParts(0, 0, 0, false, 0)) <= 0) {
            return new ValidationResult$2(1, singleton(new ValidationErrorType(5)));
        }
        else {
            return ValidationResult_map((currencyType_1) => (new Currency(0, value_1, currencyType_1)), ThreeLetterString_Parse_Z721C83C5(currencyType));
        }
    }
    else {
        return new ValidationResult$2(1, singleton(new ValidationErrorType(4)));
    }
}

function mapValidationError(propertyName, v) {
    if (v.tag === 1) {
        return new ValidationResult$2(1, map((e) => [propertyName, e], v.fields[0]));
    }
    else {
        return new ValidationResult$2(0, v.fields[0]);
    }
}

export class AddLocation extends Record {
    constructor(Id, Name, Location, Price, IsDraft, Remark, Created, Creator) {
        super();
        this.Id = Id;
        this.Name = Name;
        this.Location = Location;
        this.Price = Price;
        this.IsDraft = IsDraft;
        this.Remark = Remark;
        this.Created = Created;
        this.Creator = Creator;
    }
}

export function AddLocation$reflection() {
    return record_type("Ronnies.Domain.AddLocation", [], AddLocation, () => [["Id", Identifier$reflection()], ["Name", NonEmptyString$reflection()], ["Location", Location$reflection()], ["Price", Currency$reflection()], ["IsDraft", bool_type], ["Remark", option_type(string_type)], ["Created", class_type("System.DateTimeOffset")], ["Creator", NonEmptyString$reflection()]]);
}

export function AddLocation_Parse(id, name, lat, lng, price, currency, isDraft, remark, created, creator) {
    let _arg1;
    return op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessMultiplyGreater()(op_LessBangGreater()((id_1) => ((name_1) => ((location) => ((price_1) => ((isDraft_1) => ((remark_1) => ((created_1) => ((creator_1) => (new AddLocation(id_1, name_1, location, price_1, isDraft_1, remark_1, created_1, creator_1))))))))))(mapValidationError("id", (_arg1 = id, (_arg1.fields[0] === "00000000-0000-0000-0000-000000000000") ? (new ValidationResult$2(1, singleton(new ValidationErrorType(7)))) : (new ValidationResult$2(0, _arg1))))))(mapValidationError("name", NonEmptyString_Parse_Z721C83C5(name))))(mapValidationError("location", Location_Parse(lat, lng))))(mapValidationError("price", Currency_Parse(price, currency))))(mapValidationError("draft", ValidationResult_lift(isDraft))))(mapValidationError("remark", ValidationResult_lift(remark))))(mapValidationError("created", ValidationResult_lift(created))))(mapValidationError("creator", NonEmptyString_Parse_Z721C83C5(creator)));
}

export function AddLocation_get_Encoder() {
    return (addLocation) => {
        const patternInput_1 = addLocation.Location;
        const patternInput_2 = addLocation.Price;
        return object([["id", guid(addLocation.Id.fields[0])], ["name", NonEmptyString_Read_10C27AE8(addLocation.Name)], ["location", tuple2((value_1) => value_1, (value_3) => value_3, patternInput_1.fields[0].fields[0], patternInput_1.fields[1].fields[0])], ["price", tuple2(decimal, (value_6) => value_6, patternInput_2.fields[0], patternInput_2.fields[1].fields[0])], ["isDraft", addLocation.IsDraft], ["remark", option((value_9) => value_9)(addLocation.Remark)], ["created", datetimeOffset(addLocation.Created)], ["creator", addLocation.Creator.fields[0]]]);
    };
}

export function AddLocation_get_Decoder() {
    return (path_11) => ((value_10) => andThen(uncurry(3, (tupledArg) => {
        const result = AddLocation_Parse(tupledArg[0], tupledArg[1], tupledArg[2], tupledArg[3], tupledArg[4], tupledArg[5], tupledArg[6], tupledArg[7], tupledArg[8], tupledArg[9]);
        if (result.tag === 1) {
            return Decode_failWithDomainErrors(result.fields[0]);
        }
        else {
            return (arg10$0040) => ((arg20$0040) => succeed(result.fields[0], arg10$0040, arg20$0040));
        }
    }), (path_10, v) => object_1((get$) => {
        const patternInput = get$.Required.Field("location", uncurry(2, tuple2_1(float, float)));
        const patternInput_1 = get$.Required.Field("price", uncurry(2, tuple2_1(string, string)));
        return [Identifier_Parse_244AC511(get$.Required.Field("id", guid_1)), get$.Required.Field("name", string), patternInput[0], patternInput[1], patternInput_1[0], patternInput_1[1], get$.Required.Field("isDraft", bool), get$.Optional.Field("remark", string), get$.Required.Field("created", datetimeOffset_1), get$.Required.Field("creator", string)];
    }, path_10, v), path_11, value_10));
}

export class Event$ extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["LocationAdded", "LocationCancelled", "LocationNoLongerSellsRonnies"];
    }
}

export function Event$$reflection() {
    return union_type("Ronnies.Domain.Event", [], Event$, () => [[["Item", AddLocation$reflection()]], [["Item", Identifier$reflection()]], [["Item", Identifier$reflection()]]]);
}

export function Event_get_Encoder() {
    return (event) => ((event.tag === 1) ? ["locationCancelled", guid(Identifier_Read_4C3C6BC4(event.fields[0]))] : ((event.tag === 2) ? ["locationNoLongerSellsRonnies", guid(Identifier_Read_4C3C6BC4(event.fields[0]))] : ["locationAdded", AddLocation_get_Encoder()(event.fields[0])]));
}

export function Event_get_Decoder() {
    return (path_11) => ((value_10) => andThen(uncurry(3, (caseName) => {
        switch (caseName) {
            case "locationAdded": {
                let d1;
                const decoder_1 = AddLocation_get_Decoder();
                d1 = ((path_2) => ((value_2) => index(1, uncurry(2, decoder_1), path_2, value_2)));
                return (path_3) => ((value_3) => map_1((arg0) => (new Event$(0, arg0)), uncurry(2, d1), path_3, value_3));
            }
            case "locationCancelled": {
                return (path_6) => ((value_6) => map_1((id) => (new Event$(1, Identifier_Parse_244AC511(id))), (path_5, value_5) => index(1, guid_1, path_5, value_5), path_6, value_6));
            }
            case "locationNoLongerSellsRonnies": {
                return (path_9) => ((value_9) => map_1((id_1) => (new Event$(2, Identifier_Parse_244AC511(id_1))), (path_8, value_8) => index(1, guid_1, path_8, value_8), path_9, value_9));
            }
            default: {
                const msg = toText(printf("`%s` is not a valid case for Event"))(caseName);
                return (path_10) => ((arg20$0040) => fail(msg, path_10, arg20$0040));
            }
        }
    }), (path_1, value_1) => index(0, string, path_1, value_1), path_11, value_10));
}

