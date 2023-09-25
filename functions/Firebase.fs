namespace Firebase.Admin

open Fable.Core

#nowarn "1182"

module App =
    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.app.appoptions
    type AppOptions =
        {|
            apiKey : string
            authDomain : string
            projectId : string
            storageBucket : string
            messagingSenderId : string
            appId : string
        |}

    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.app.app
    type App =
        abstract name : string

    type Exports =
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.app.md#initializeapp
        [<Import("initializeApp", "firebase-admin/app")>]
        static member initializeApp (options : AppOptions) : App = jsNative

module Auth =
    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.userrecord.md#userrecord_class
    type UserRecord<'Claims> =
        abstract uid : string
        abstract displayName : string
        abstract email : string
        abstract customClaims : 'Claims

    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.listusersresult.md#listusersresult_interface
    type ListUserResult<'Claims> =
        abstract pageToken : string
        abstract users : UserRecord<'Claims> array

    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.auth.md#auth_class
    type Auth =
        abstract app : App.App

        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthcreateuser
        abstract createUser<'Claims> :
            {|
                email : string
                displayName : string
                password : string
            |} ->
                JS.Promise<UserRecord<'Claims>>

        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthsetcustomuserclaims
        abstract setCustomUserClaims<'Claims> : string * customUserClaims : 'Claims -> JS.Promise<unit>
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthgetuserbyemail
        abstract getUserByEmail<'Claims> : string -> JS.Promise<UserRecord<'Claims>>
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthlistusers
        abstract listUsers<'Claims> : int -> JS.Promise<ListUserResult<'Claims>>
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthdeleteuser
        abstract deleteUser : string -> JS.Promise<unit>
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.baseauth.md#baseauthgetuser
        abstract getUser<'Claims> : string -> JS.Promise<UserRecord<'Claims>>

    type Exports =
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.auth.md#getauth
        [<Import("getAuth", "firebase-admin/auth")>]
        static member getAuth (app : App.App) : Auth = jsNative

module rec FireStore =
    /// https://googleapis.dev/nodejs/firestore/latest/DocumentReference.html
    type DocumentReference<'T> =
        abstract member id : string
        abstract member parent : CollectionReference<'T>
        abstract member path : string

    /// https://googleapis.dev/nodejs/firestore/latest/DocumentSnapshot.html
    type DocumentSnapshot<'T> =
        abstract member id : string
        abstract member ref : DocumentReference<'T>
        abstract member data : unit -> 'T
        abstract member exists : bool
        abstract member get : fieldPath : string -> 'T

    /// https://firebase.google.com/docs/reference/js/firestore_.querydocumentsnapshot.md#querydocumentsnapshot_class
    type QueryDocumentSnapshot<'T> =
        inherit DocumentSnapshot<'T>
        abstract member data : unit -> 'T
        abstract member data : options : obj -> 'T

    /// https://googleapis.dev/nodejs/firestore/latest/QuerySnapshot.html
    type QuerySnapshot<'T> =
        abstract member docs : QueryDocumentSnapshot<'T> array
        abstract member empty : bool
        abstract member metadata : obj
        abstract member query : Query<'T>
        abstract member size : int

    /// https://googleapis.dev/nodejs/firestore/latest/Query.html
    type Query<'T> =
        abstract member converter : obj
        abstract member firestore : FireStore
        abstract member ``type`` : string
        /// https://googleapis.dev/nodejs/firestore/latest/Query.html#get
        abstract member get : unit -> JS.Promise<QuerySnapshot<'T>>

    /// https://googleapis.dev/nodejs/firestore/latest/CollectionReference.html
    type CollectionReference<'T> =
        inherit Query<'T>

    type FireStore =
        abstract collection<'T> : string -> CollectionReference<'T>

    type Exports =
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.firestore#getfirestore
        [<Import("getFirestore", "firebase-admin/firestore")>]
        static member getFirestore (app : App.App) : FireStore = jsNative

module Messaging =
    type TokenMessage = {| token : string ; data : obj |}

    /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.messaging.messaging.md#messaging_class
    type Messaging =
        abstract send : TokenMessage -> JS.Promise<unit>

    type Exports =
        /// https://firebase.google.com/docs/reference/admin/node/firebase-admin.messaging.md#getmessaging
        [<Import("getMessaging", "firebase-admin/messaging")>]
        static member getMessaging (app : App.App) : Messaging = jsNative

namespace Firebase.Functions

open Fable.Core

#nowarn "1182"

module V2 =

    module Https =
        /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.https.httpsoptions.md#httpshttpsoptions_interface
        type HttpsOptions =
            {|
                region : string
                allowedCors : string option
            |}

        type RequestAuth<'Claims> =
            abstract uid : string
            abstract token : 'Claims option

        type Request<'Claims> =
            abstract baseUrl : string
            abstract method : string
            abstract headers : obj
            abstract body : obj
            abstract auth : RequestAuth<'Claims> option

        type Response =
            abstract status : int -> Response
            abstract send : obj -> Response
            abstract sendStatus : int -> Response

        type HttpsFunction =
            interface
            end

        type CallableFunction =
            interface
            end

        type CallableRequest<'TData, 'Claims> =
            abstract auth : RequestAuth<'Claims> option
            abstract data : 'TData

        [<Import("HttpsError", "firebase-functions/v2/https")>]
        type HttpsError(value : string) =
            inherit System.Exception(value)

        type Exports =
            /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.https.md#httpsonrequest
            [<Import("onRequest", "firebase-functions/v2/https")>]
            static member onRequest<'Claims>
                (
                    options : HttpsOptions,
                    handler : System.Func<Request<'Claims>, Response, JS.Promise<Response>>
                )
                : HttpsFunction
                =
                jsNative

            /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.https.md#httpsoncall
            [<Import("onCall", "firebase-functions/v2/https")>]
            static member onCall<'TData, 'Claims, 'TResponse>
                (
                    options : HttpsOptions,
                    handler : CallableRequest<'TData, 'Claims> -> JS.Promise<'TResponse>
                )
                : CallableFunction
                =
                jsNative

module Logger =
    open System
    open Fable.Core.JsInterop

    /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.logger
    type ILogger =
        /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.logger.md#loggerwarn
        abstract warn : [<ParamArray>] parameters : obj array -> unit
        /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.logger.md#loggererror
        abstract error : [<ParamArray>] parameters : obj array -> unit
        /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.logger.md#loggerinfo
        abstract info : [<ParamArray>] parameters : obj array -> unit

    let logger : ILogger = import "*" "firebase-functions/logger"

module FireStore =
    type CloudFunction =
        interface
        end

    type CloudEvent<'T> =
        abstract data : 'T

    type CreatedHandler<'TData, 'TResult> =
        CloudEvent<Firebase.Admin.FireStore.QueryDocumentSnapshot<'TData>> -> JS.Promise<'TResult>

    type Exports =
        /// https://firebase.google.com/docs/reference/functions/2nd-gen/node/firebase-functions.firestore.md#firestoreondocumentcreated
        [<Import("onDocumentCreated", "firebase-functions/v2/firestore")>]
        static member onDocumentCreated<'TData, 'TResult>
            (
                documentPath : string,
                handler : CreatedHandler<'TData, 'TResult>
            )
            : CloudFunction
            =
            jsNative
