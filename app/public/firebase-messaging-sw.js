import {initializeApp} from "https://www.gstatic.com/firebasejs/10.0.0/firebase-app.js";
import { getMessaging, onBackgroundMessage } from 'https://www.gstatic.com/firebasejs/10.0.0/firebase-messaging-sw.js';

const firebaseConfig = {
    apiKey: "AIzaSyDq-c1-HDDAQqNbFZgeWQ8VA8tAPSTwXxo",
    authDomain: "ronnies-210509.firebaseapp.com",
    projectId: "ronnies-210509",
    storageBucket: "ronnies-210509.appspot.com",
    messagingSenderId: "566310710121",
    appId: "1:566310710121:web:1bc67dddf5834127e7ebf8"
};

const app = initializeApp(firebaseConfig);

// Retrieve firebase messaging
const messaging = getMessaging(app);

const ronnyInMorse = ".-. --- -. -. -.--"
  .split("")
  .map((c) => (c === "." ? 100 : 250));

function onMessage(payload) {
  console.log('Received background message ', payload);
  const { locationId, locationName, userName } = payload.data;

  const notificationTitle = "Nieuwe ronny plekke toegevoegd";
  const notificationOptions = {
    body: `${locationName} door ${userName}`,
    icon: "/images/ronny.png",
    badge: "/images/ronny.png",
    data: payload.data,
    vibrate: ronnyInMorse,
  };

  return self.registration.showNotification(notificationTitle, notificationOptions);
}

onBackgroundMessage(messaging, onMessage);

self.addEventListener("notificationclick", (evt) => {
  evt.notification.close();
  const payload = evt.notification.data;
  const locationId = payload && payload.locationId;
  if (locationId) {
    const url = `${evt.target.location.origin}/detail/${locationId}`;
    return evt.waitUntil(clients.openWindow(url));
  }
});
