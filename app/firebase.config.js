// Import the functions you need from the SDKs you need
import {initializeApp} from "firebase/app";
import {getAuth, connectAuthEmulator} from 'firebase/auth';
import {getFirestore, connectFirestoreEmulator} from 'firebase/firestore';
import {getStorage, connectStorageEmulator } from 'firebase/storage';
import { getFunctions, connectFunctionsEmulator } from 'firebase/functions';
import { getMessaging, isSupported } from 'firebase/messaging';

// Your web app's Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyDq-c1-HDDAQqNbFZgeWQ8VA8tAPSTwXxo",
    authDomain: "ronnies-210509.firebaseapp.com",
    projectId: "ronnies-210509",
    storageBucket: "ronnies-210509.appspot.com",
    messagingSenderId: "566310710121",
    appId: "1:566310710121:web:1bc67dddf5834127e7ebf8"
};

// Initialize Firebase app
export const app = initializeApp(firebaseConfig);

// Connect to the Firebase Authentication emulator
export const auth = getAuth(app);

const isLocalHost = window.location.hostname === 'localhost';

if (isLocalHost) {
    connectAuthEmulator(auth, 'http://localhost:9099');
}

// Connect to the Firestore emulator
export const firestore = getFirestore(app);
if (isLocalHost) {
    connectFirestoreEmulator(firestore, 'localhost', 6006);
}

export const storage = getStorage();
if (isLocalHost) {
    connectStorageEmulator(storage, "localhost", 7007);
}

export const functions = getFunctions(app, "europe-west1");
if (isLocalHost) {
    connectFunctionsEmulator(functions, "localhost", 5001);
}

export function messaging () {
    return isSupported().then(() => {
        return getMessaging(app);
    })
} 
