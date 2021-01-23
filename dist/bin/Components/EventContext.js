import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { Event$$reflection } from "../shared/Domain.js";
import { record_type, lambda_type, class_type, unit_type, list_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { append, concat, empty } from "../.fable/fable-library.3.1.1/List.js";
import { React_contextProvider_34D9BBBD, useReact_useEffectOnce_3A5B6456, useFeliz_React__React_useState_Static_1505, React_createContext_1AE444D8 } from "../.fable/Feliz.1.31.1/React.fs.js";
import { removeAllEvents, persistEvents, syncLatestEvents, getAllEvents } from "../IdbKeyVal.js";
import { useAuth0 } from "../Auth0.js";

export class EventContext extends Record {
    constructor(Events, AddEvents, ClearCache) {
        super();
        this.Events = Events;
        this.AddEvents = AddEvents;
        this.ClearCache = ClearCache;
    }
}

export function EventContext$reflection() {
    return record_type("Ronnies.Client.Components.EventContext.EventContext", [], EventContext, () => [["Events", list_type(Event$$reflection())], ["AddEvents", lambda_type(list_type(Event$$reflection()), class_type("Fable.Core.JS.Promise`1", [unit_type]))], ["ClearCache", lambda_type(unit_type, class_type("Fable.Core.JS.Promise`1", [unit_type]))]]);
}

const emptyEventContext = new EventContext(empty(), (_arg1) => (Promise.resolve(undefined)), () => (Promise.resolve(undefined)));

export const eventContext = React_createContext_1AE444D8(void 0, emptyEventContext);

function fetchLatestEvents(setEvents) {
    let pr_1;
    const pr = getAllEvents();
    pr_1 = (pr.then(((existingEvents) => {
        setEvents(existingEvents);
        const newEvents = syncLatestEvents();
        return Promise.all([Promise.resolve(existingEvents), newEvents]);
    })));
    pr_1.then(((arg) => {
        setEvents(concat(arg));
    }));
}

export function Events(props) {
    const patternInput = useFeliz_React__React_useState_Static_1505(empty());
    const setEvents = patternInput[1];
    const events = patternInput[0];
    const auth0 = useAuth0();
    useReact_useEffectOnce_3A5B6456(() => {
        fetchLatestEvents(setEvents);
    });
    return React_contextProvider_34D9BBBD(eventContext, new EventContext(events, (appendEvents) => {
        let pr_1;
        const pr = auth0.getAccessTokenSilently();
        pr_1 = (pr.then(((authToken) => persistEvents(appendEvents, authToken))));
        return pr_1.then(((persistedEvents) => {
            setEvents(append(events, persistedEvents));
        }));
    }, () => {
        removeAllEvents();
        const pr_2 = syncLatestEvents();
        return pr_2.then(setEvents);
    }), props.children);
}

