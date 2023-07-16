self.importScripts(
  "https://cdn.jsdelivr.net/npm/idb-keyval@3/dist/idb-keyval-iife.min.js"
);
// self.idbKeyval

function syncIndexDB(isLocalhost) {
  const ronnyStore = new self.idbKeyval.Store("ronnies.be", "events");
  return self.idbKeyval
    .keys(ronnyStore)
    .then((keys) => {
      return Math.max(0, Math.max(...keys));
    })
    .then((lastEventId) => {
      const root = isLocalhost
        ? "http://localhost:9090"
        : "https://nojaf-apim.azure-api.net/ronnies";
      const api = lastEventId
        ? `/events?lastEvent=${lastEventId}`
        : "/events";
      const url = root + api;
      const headers = isLocalhost
        ? {}
        : { "Ocp-Apim-Subscription-Key": "ee291b2be6464d6f94401c2c82c72775" };
      return fetch(url, {
        headers,
      });
    })
    .then((response) => response.json())
    .then((events) => {
      console.group("events in service worker");
      console.log(events);
      console.groupEnd();
      const newEventIds = Object.keys(events);
      const addEvents = newEventIds.map((id) => {
        const version = parseInt(id, 0);
        const evt = events[id];
        return self.idbKeyval.set(version, evt, ronnyStore);
      });
      return Promise.all(addEvents);
    });
}

const ronnyInMorse = ".-. --- -. -. -.--"
  .split("")
  .map((c) => (c === "." ? 100 : 250));

self.addEventListener("push", (evt) => {
  /** @typedef {Object} json
   * @property {String} creator
   * @property {String} id
   * @property {String} name
   * @property {String} type
   */
  const payload = evt.data && evt.data.json();

  const origin = evt.target.location.origin;
  // sync indexDb
  evt.waitUntil(
    syncIndexDB(origin === "http://localhost:8080" || evt.indexOf("gitpod") !== -1)
  );

  // show notification
  const title = "Nieuwe ronny plekke toegevoegd";
  const options = {
    body: `${payload.name} door ${payload.creator}`,
    icon: "/assets/android-chrome-192x192.png",
    badge: "/assets/android-chrome-192x192.png",
    data: payload,
    vibrate: ronnyInMorse,
  };

  return evt.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener("notificationclick", (evt) => {
  evt.notification.close();
  const payload = evt.notification.data;
  const url = `${evt.target.location.origin}/detail/${payload.id}`;
  return evt.waitUntil(clients.openWindow(url));
});
