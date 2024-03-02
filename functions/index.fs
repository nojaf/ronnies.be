module Functions

open System
open Fable.Core
open Fable.Core.JsInterop
open Firebase.Admin
open Firebase.Functions
open Firebase.Functions.V2
open Firebase.Functions.V2.Https
open Ronnies.Shared

let isEmulator : bool = emitJsExpr () "process.env.FUNCTIONS_EMULATOR === \"true\""

let allowedCors =
    if isEmulator then
        "http://localhost:4000"
    else
        "https://ronnies.be"

let SECRET_HEADER : string = emitJsExpr () "process.env.SECRET_HEADER"

let SECRET_HEADER_VALUE : string = emitJsExpr () "process.env.SECRET_HEADER_VALUE"

let firebaseConfig : App.AppOptions =
    {|
        apiKey = "AIzaSyDq-c1-HDDAQqNbFZgeWQ8VA8tAPSTwXxo"
        authDomain = "ronnies-210509.firebaseapp.com"
        projectId = "ronnies-210509"
        storageBucket = "ronnies-210509.appspot.com"
        messagingSenderId = "566310710121"
        appId = "1:566310710121:web:1bc67dddf5834127e7ebf8"
    |}

let app = App.Exports.initializeApp firebaseConfig
let auth = Auth.Exports.getAuth app
let firestore = FireStore.Exports.getFirestore app
let messaging = Messaging.Exports.getMessaging app

let getSecretHeader (request : Https.Request<_>) : string =
    emitJsExpr (request, SECRET_HEADER) "$0.headers && $0.headers[$1]"

let secretRequest
    verb
    (request : Https.Request<_>)
    (response : Https.Response)
    (f : Https.Request<_> -> Https.Response -> JS.Promise<Https.Response>)
    : JS.Promise<Https.Response>
    =
    promise {
        if request.method <> verb then
            return response.status(400).send ("Bad request")
        elif (getSecretHeader request) <> SECRET_HEADER_VALUE then
            return response.status(400).send ("Bad request")
        else
            return! f request response
    }

type User =
    abstract displayName : string
    abstract email : string

type CustomClaims = {| ``member`` : bool ; admin : bool |}

let isAdmin (auth : Https.RequestAuth<CustomClaims> option) =
    match auth with
    | None -> false
    | Some auth ->
        match auth.token with
        | None -> false
        | Some token -> token.admin

let tryFindUserByEmail email =
    auth.getUserByEmail email
    |> Promise.map (fun user -> Some user)
    |> Promise.catch (fun _ -> None)

let user =
    Https.Exports.onCall<User, CustomClaims, Auth.UserRecord<CustomClaims>> (
        {|
            region = "europe-west1"
            allowedCors = None
        |},
        fun request ->
            promise {
                if not (isAdmin request.auth) then
                    return raise (new Https.HttpsError ("Invalid permissions"))

                let user = request.data

                if
                    (String.IsNullOrWhiteSpace user.displayName
                     || String.IsNullOrWhiteSpace user.email
                     || not (user.email.Contains ("@")))
                then
                    Logger.logger.warn ($"Invalid user or email: ${user.displayName} ${user.email}")
                    return raise (new Https.HttpsError ("Invalid user or email"))
                else
                    let! existingUser = tryFindUserByEmail user.email

                    let! user =
                        match existingUser with
                        | Some existingUser -> auth.updateUser (existingUser.uid, {| displayName = user.displayName |})
                        | None ->
                            auth.createUser (
                                {|
                                    email = user.email
                                    displayName = user.displayName
                                    password = "ronalds"
                                |}
                            )

                    do!
                        auth.setCustomUserClaims (
                            user.uid,
                            JS.Constructors.Object.assign (obj (), user.customClaims, {| ``member`` = true |})
                        )

                    return user
            }
    )

let sudo =
    Https.Exports.onRequest (
        {|
            region = "europe-west1"
            allowedCors = None
        |},
        fun request response ->
            secretRequest
                "POST"
                request
                response
                (fun request response ->
                    promise {
                        try
                            let email : string = request.body?email
                            let! user = auth.getUserByEmail email

                            do!
                                auth.setCustomUserClaims<CustomClaims> (
                                    user.uid,
                                    {| ``member`` = true ; admin = true |}
                                )

                            return response.status(200).send (user)
                        with ex ->
                            Logger.logger.error ("Error while elevating user", ex, {| structuredData = true |})
                            return response.sendStatus (500)
                    }
                )

    )

let (|HasMemberClaim|_|) (requestAuth : Https.RequestAuth<CustomClaims> option) =
    match requestAuth with
    | None -> None
    | Some requestAuth ->
        match requestAuth.token with
        | None -> None
        | Some token -> if token.``member`` then Some requestAuth.uid else None

let users =
    Https.Exports.onCall (
        {|
            region = "europe-west1"
            allowedCors = Some allowedCors
        |},
        fun (request : CallableRequest<UsersData, CustomClaims>) ->
            promise {
                match request.auth with
                | HasMemberClaim currentId ->
                    let! listUsersResult = auth.listUsers<CustomClaims> 1000

                    let users =
                        listUsersResult.users
                        |> Array.choose (fun userRecord ->
                            if userRecord.uid = currentId && not request.data.includeCurrentUser then
                                None
                            elif userRecord.uid = currentId then
                                Some
                                    {|
                                        displayName = userRecord.displayName
                                        uid = userRecord.uid
                                    |}
                            else
                                userRecord.customClaims
                                |> Option.bind (fun customClaims ->
                                    if not customClaims.``member`` then
                                        None
                                    else
                                        Some
                                            {|
                                                displayName = userRecord.displayName
                                                uid = userRecord.uid
                                            |}
                                )

                        )

                    return users

                | _ -> return raise<_> (new Https.HttpsError ("Unauthorized access"))

            }
    )

let cleanUpUsers =
    Https.Exports.onRequest (
        {|
            region = "europe-west1"
            allowedCors = None
        |},
        fun request response ->
            secretRequest
                "POST"
                request
                response
                (fun _request response ->
                    promise {
                        let! listUserResult = auth.listUsers 1000

                        let nonMembers =
                            listUserResult.users
                            |> Array.filter (fun user -> Option.isNone (user.customClaims))

                        for nonMember in nonMembers do
                            do! auth.deleteUser (nonMember.uid)
                            Logger.logger.info $"Deleted {nonMember.uid} {nonMember.email}"

                        return response.sendStatus 200
                    }
                )
    )

type FCMTokenData = {| tokens : string array |}

let broadCastNotification data =
    promise {
        let fcmCollection = firestore.collection<FCMTokenData> ("fcmTokens")
        let! tokenSnapshots = fcmCollection.get ()

        for tokenSnapshot in tokenSnapshots.docs do
            let tokenData = tokenSnapshot.data ()

            for token in tokenData.tokens do
                try
                    do! messaging.send {| token = token ; data = data |}
                with ex ->
                    Firebase.Functions.Logger.logger.error ex
    }

// There are more properties present here, I'm only using the data I'm interested in
type RonnyLocation =
    abstract name : string
    abstract userId : string

let locationCreated =
    FireStore.Exports.onDocumentCreated<RonnyLocation, unit> (
        "locations/{locationId}",
        fun event ->
            promise {
                let location = event.data.data ()

                if
                    not (isNullOrUndefined location)
                    && not (String.IsNullOrWhiteSpace location.name)
                then
                    let! user = auth.getUser (location.userId)

                    let locationData =
                        {|
                            locationId = event.data.id
                            userName = user.displayName
                            locationName = location.name
                        |}

                    do! broadCastNotification locationData

            }
    )

emitJsStatement
    ()
    """
if (process.env.FUNCTIONS_EMULATOR === "true") {
  auth.getUserByEmail("florian.verdonck@outlook.com").catch((err) => {
    auth
      .createUser({
        displayName: "nojaf",
        email: "florian.verdonck@outlook.com",
        password: "ronalds",
      })
      .then((user) => {
        console.log("admin seeded");
        auth.setCustomUserClaims(user.uid, {
          admin: true,
          member: true,
        });
      });
  });
}
"""

let testNotification =
    Https.Exports.onRequest (
        {|
            region = "europe-west1"
            allowedCors = None
        |},
        fun request response ->
            secretRequest
                "POST"
                request
                response
                (fun _request response ->
                    promise {
                        try
                            let! user = auth.getUserByEmail "florian.verdonck@outlook.com"

                            let data =
                                {|
                                    locationId = "Emwzg2aIXFjESlL0RB66"
                                    userName = "Sie Nojaf"
                                    locationName = "Test notification"
                                |}

                            let fcmCollection = firestore.collection<FCMTokenData> ("fcmTokens")
                            let fcmDocumentReference = fcmCollection.doc user.uid
                            let! fcmDocumentSnapshot = fcmDocumentReference.get ()
                            let tokenData = fcmDocumentSnapshot.data ()

                            for token in tokenData.tokens do
                                try
                                    do! messaging.send {| token = token ; data = data |}
                                with ex ->
                                    Firebase.Functions.Logger.logger.error ex

                            return response.status(200).send (tokenData)
                        with ex ->
                            Logger.logger.error ("Error while elevating user", ex, {| structuredData = true |})
                            return response.sendStatus (500)
                    }
                )

    )
