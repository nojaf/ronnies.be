import * as __SNOWPACK_ENV__ from '../../_snowpack/env.js';

import { Types_HttpRequestHeaders } from "./.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { Currency_Read_6D69B0C0 } from "./shared/Domain.js";

export const subscriptionHeader = ["Ocp-Apim-Subscription-Key", __SNOWPACK_ENV__.SNOWPACK_PUBLIC_SUBSCRIPTION_KEY];

export function authHeader(token) {
    return new Types_HttpRequestHeaders(5, toText(printf("Bearer %s"))(token));
}

export const vapidKey = "BKjykdL6nZKMNQcO9viWqf6TbA_XegmhbCneNMBX4AWu5D8DD6e6KjeSMxXmUycsNPeGkYHtca-i_-eePtzQn3w";

export function readCurrency(price) {
    const patternInput = Currency_Read_6D69B0C0(price);
    const value = patternInput[0];
    const currency = patternInput[1];
    switch (currency) {
        case "EUR": {
            return toText(printf("€%.2f"))(value);
        }
        case "USD": {
            return toText(printf("$%.2f"))(value);
        }
        case "GBP": {
            return toText(printf("£%.2f"))(value);
        }
        default: {
            return toText(printf("%.2f %s"))(value)(currency);
        }
    }
}

