const { initializeApp } = require("firebase-admin/app");

const { onRequest } = require("firebase-functions/v2/https");
const logger = require("firebase-functions/logger");
const { getAuth } = require("firebase-admin/auth");

const isEmulator = process.env.FUNCTIONS_EMULATOR === "true";
const allowedCors = isEmulator ? "http://localhost:4000" : "https://ronnies.be";

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyDq-c1-HDDAQqNbFZgeWQ8VA8tAPSTwXxo",
  authDomain: "ronnies-210509.firebaseapp.com",
  projectId: "ronnies-210509",
  storageBucket: "ronnies-210509.appspot.com",
  messagingSenderId: "566310710121",
  appId: "1:566310710121:web:1bc67dddf5834127e7ebf8",
};
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);

exports.user = onRequest(
  { region: "europe-west1" },
  async (request, response) => {
    try {
      if (request.method !== "POST") {
        return response.status(400).send("Bad request");
      }

      if (
        request.headers &&
        request.headers["x-kye"] !== "Um9ubmllcyB6aWpuIGtpc3pha3Mh"
      ) {
        return response.status(400).send("Bad request");
      }

      const { displayName, email } = request.body;
      if (!displayName || !email || email.indexOf("@") === -1) {
        logger.warn(`Invalid user or email: ${user} ${email}`);
        return response.sendStatus(400);
      }

      const result = await auth.createUser({
        email: email,
        displayName: displayName,
        password: "ronalds",
      });

      await auth.setCustomUserClaims(result.uid, {
        member: true,
      });

      return response.status(200).send(result);
    } catch (error) {
      logger.error("Error while creating user", error, {
        structuredData: true,
      });
      return response.sendStatus(500);
    }
  }
);

exports.sudo = onRequest(
  { region: "europe-west1" },
  async (request, response) => {
    try {
      if (request.method !== "POST") {
        return response.status(400).send("Bad request");
      }

      if (
        request.headers &&
        request.headers["x-kye"] !== "Um9ubmllcyB6aWpuIGtpc3pha3Mh"
      ) {
        return response.status(400).send("Bad request");
      }

      const { email } = request.body;
      const user = await auth.getUserByEmail(email);

      await auth.setCustomUserClaims(user.uid, {
        admin: true,
        member: true,
      });

      return response.status(200).send(user);
    } catch (error) {
      logger.error("Error while creating user", error, {
        structuredData: true,
      });
      return response.sendStatus(500);
    }
  }
);

exports.users = onRequest(
  { region: "europe-west1", cors: allowedCors },
  async (request, response) => {
    try {
      if (request.method !== "GET") {
        return response.status(400).send("Bad request");
      }

      const currentId = request.path && request.path.replace("/", "");
      const listUsersResult = await auth.listUsers(1000);
      const users = listUsersResult.users
        .filter((userRecord) => userRecord.uid !== currentId && userRecord.customClaims.member)
        .map((userRecord) => ({
          displayName: userRecord.displayName,
          uid: userRecord.uid,
        }));
      return response.status(200).send(users);
    } catch (error) {
      logger.error("Error while creating user", error, {
        structuredData: true,
      });
      return response.sendStatus(500);
    }
  }
);