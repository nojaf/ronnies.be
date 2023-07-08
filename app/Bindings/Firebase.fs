namespace Firebase

open System
open Fable.Core
open Fable.Core.JS

#nowarn "1182"

module App =
    /// https://firebase.google.com/docs/reference/js/app.firebaseoptions.md#firebaseoptions_interface
    type FirebaseOptions =
        abstract apiKey : string
        abstract appId : string
        abstract authDomain : string
        abstract databaseURL : string
        abstract measurementId : string
        abstract messagingSenderId : string
        abstract projectId : string
        abstract storageBucket : string

    /// https://firebase.google.com/docs/reference/js/app.firebaseapp
    type FirebaseApp =
        abstract automaticDataCollectionEnabled : bool
        abstract name : string
        abstract options : FirebaseOptions

module Auth =
    /// https://firebase.google.com/docs/reference/js/auth.userinfo.md#userinfo_interface
    type UserInfo =
        abstract member uid : string
        abstract member displayName : string

    /// https://firebase.google.com/docs/reference/js/auth.user.md#user_interface
    type User =
        inherit UserInfo
        abstract member isAnonymous : bool

    /// https://firebase.google.com/docs/reference/js/auth.idtokenresult
    type IdTokenResult =
        abstract member claims : obj

    /// https://firebase.google.com/docs/reference/js/auth.auth
    type Auth =
        abstract member app : App.FirebaseApp
        abstract member currentUser : User option

    /// https://firebase.google.com/docs/reference/js/auth.usercredential
    type UserCredential =
        interface
        end

    /// https://firebase.google.com/docs/reference/js/auth.actioncodesettings.md#actioncodesettings_interface
    type ActionCodeSettings =
        {|
            handleCodeInApp : bool
            url : string
        |}

    type Exports =
        /// https://firebase.google.com/docs/reference/js/auth.md#signinwithemaillink
        [<Import("signInWithEmailLink", "firebase/auth")>]
        static member signInWithEmailLink (auth : Auth, email : string, ?emailLink : string) : Promise<UserCredential> =
            jsNative

        /// https://firebase.google.com/docs/reference/js/auth.md#createuserwithemailandpassword
        [<Import("createUserWithEmailAndPassword", "firebase/auth")>]
        static member createUserWithEmailAndPassword
            (
                auth : Auth,
                email : string,
                password : string
            )
            : Promise<UserCredential>
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/auth.md#issigninwithemaillink
        [<Import("isSignInWithEmailLink", "firebase/auth")>]
        static member isSignInWithEmailLink (auth : Auth, emailLink : string) : bool = jsNative

        /// https://firebase.google.com/docs/reference/js/auth.md#sendsigninlinktoemail
        [<Import("sendSignInLinkToEmail", "firebase/auth")>]
        static member sendSignInLinkToEmail
            (
                auth : Auth,
                email : string,
                actionCodeSettings : ActionCodeSettings
            )
            : Promise<unit>
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/auth.md#signout
        [<Import("signOut", "firebase/auth")>]
        static member signOut (auth : Auth) : Promise<unit> = jsNative

module rec FireStore =
    open App

    /// https://firebase.google.com/docs/reference/js/firestore_.firestore
    type FireStore =
        abstract app : App.FirebaseApp
        abstract ``type`` : string

    /// https://firebase.google.com/docs/reference/js/firestore_.query.md#query_class
    type Query<'T> =
        abstract member converter : obj
        abstract member firestore : FireStore
        abstract member ``type`` : string

    /// https://firebase.google.com/docs/reference/js/firestore_.collectionreference
    type CollectionReference<'T> =
        inherit Query<'T>

    /// https://firebase.google.com/docs/reference/js/firestore_.snapshotmetadata
    type SnapshotMetadata =
        abstract member fromCache : bool
        abstract member hasPendingWrites : bool

    /// https://firebase.google.com/docs/reference/js/firestore_.documentreference
    type DocumentReference<'T> =
        abstract member id : string
        abstract member parent : CollectionReference<'T>
        abstract member path : string

    /// https://firebase.google.com/docs/reference/js/firestore_.snapshotoptions.md#snapshotoptions_interface
    type SnapshotOptions =
        abstract member serverTimestamps : ServerTimestamps

    /// https://firebase.google.com/docs/reference/js/firestore_.documentsnapshot
    type DocumentSnapshot<'T> =
        abstract member id : string
        abstract member metadata : SnapshotMetadata
        abstract member ref : DocumentReference<'T>
        abstract member data : ?options : SnapshotOptions -> 'T
        abstract member exists : unit -> bool
        abstract member get : fieldPath : string -> 'T

    /// https://firebase.google.com/docs/reference/js/firestore_.querydocumentsnapshot.md#querydocumentsnapshot_class
    type QueryDocumentSnapshot<'T> =
        inherit DocumentSnapshot<'T>
        abstract member data : ?options : obj -> 'T

    /// https://firebase.google.com/docs/reference/js/firestore_.querysnapshot
    type QuerySnapshot<'T> =
        abstract member docs : QueryDocumentSnapshot<'T> array
        abstract member empty : bool
        abstract member metadata : obj
        abstract member query : Query<'T>
        abstract member size : int

    /// https://firebase.google.com/docs/reference/js/firestore_.queryconstraint
    type QueryConstraint =
        interface
        end

    /// https://firebase.google.com/docs/reference/js/firestore_#wherefilterop
    [<StringEnum>]
    [<RequireQualifiedAccess>]
    type WhereFilterOp =
        | [<CompiledName("<")>] Greater
        | [<CompiledName("<=")>] GreaterOrEqual
        | [<CompiledName("==")>] Equal
        | [<CompiledName("!=")>] NotEqual
        | [<CompiledName(">=")>] LessOrEqual
        | [<CompiledName(">")>] Less
        | [<CompiledName("array-contains")>] ArrayContains
        | [<CompiledName("in")>] In
        | [<CompiledName("array-contains-any")>] ArrayContainsAny
        | [<CompiledName("not-in")>] NotIn

    /// https://firebase.google.com/docs/reference/node/firebase.firestore#firestoreerrorcode
    [<StringEnum>]
    [<RequireQualifiedAccess>]
    type FirestoreErrorCode =
        | [<CompiledName("cancelled")>] Cancelled
        | [<CompiledName("unknown")>] Unknown
        | [<CompiledName("invalid-argument")>] InvalidArgument
        | [<CompiledName("deadline-exceeded")>] DeadlineExceeded
        | [<CompiledName("not-found")>] NotFound
        | [<CompiledName("already-exists")>] AlreadyExists
        | [<CompiledName("permission-denied")>] PermissionDenied
        | [<CompiledName("resource-exhausted")>] ResourceExhausted
        | [<CompiledName("failed-precondition")>] FailedPrecondition
        | [<CompiledName("aborted")>] Aborted
        | [<CompiledName("out-of-range")>] OutOfRange
        | [<CompiledName("unimplemented")>] Unimplemented
        | [<CompiledName("internal")>] Internal
        | [<CompiledName("unavailable")>] Unavailable
        | [<CompiledName("data-loss")>] DateLoss
        | [<CompiledName("unauthenticated")>] Unauthenticated

    /// https://firebase.google.com/docs/reference/node/firebase.firestore.FirestoreError
    type FirestoreError =
        abstract member code : FirestoreErrorCode
        abstract member message : string
        abstract member stack : string

    /// https://firebase.google.com/docs/reference/js/firestore_.snapshotoptions.md#snapshotoptionsservertimestamps
    [<StringEnum>]
    [<RequireQualifiedAccess>]
    type ServerTimestamps =
        | [<CompiledName "estimate">] Estimate
        | [<CompiledName "previous">] Previous
        | [<CompiledName "none">] None

    type Exports =
        /// https://firebase.google.com/docs/reference/js/firestore_.md#getfirestore
        [<Import("getFirestore", "firebase/firestore")>]
        static member getFirestore (?app : FirebaseApp) : FireStore = jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_.md#collection
        [<Import("collection", "firebase/firestore")>]
        static member collection<'T>
            (
                app : FireStore,
                path : string,
                [<ParamArray>] pathSegments : string array
            )
            : CollectionReference<'T>
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_.md#doc
        [<Import("doc", "firebase/firestore")>]
        static member doc<'T>
            (
                fireStore : FireStore,
                path : string,
                [<ParamArray>] pathSegments : string array
            )
            : DocumentReference<'T>
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_.md#query
        [<Import("query", "firebase/firestore")>]
        static member query<'T>
            (
                query : Query<'T>,
                [<ParamArray>] queryConstraints : QueryConstraint array
            )
            : Query<'T>
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_.md#where
        [<Import("where", "firebase/firestore")>]
        static member where
            (
                fieldPath : string,
                opStr : WhereFilterOp,
                value : U3<string, int, bool>
            )
            : QueryConstraint
            =
            jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_lite.md#setdoc
        [<Import("setDoc ", "firebase/firestore")>]
        static member setDoc<'T> (reference : DocumentReference<'T>, data : 'T) : Promise<unit> = jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_lite.md#adddoc
        [<Import("addDoc", "firebase/firestore")>]
        static member addDoc<'T> (reference : CollectionReference<'T>, data : 'T) : Promise<DocumentReference<'T>> =
            jsNative

        /// https://firebase.google.com/docs/reference/js/firestore_lite.md#updatedoc
        [<Import("updateDoc", "firebase/firestore")>]
        static member updateDoc<'T, 'V> (reference : DocumentReference<'T>, data : 'V) : Promise<unit> = jsNative

module Storage =
    open Browser.Types

    /// https://firebase.google.com/docs/reference/js/storage.firebasestorage
    type FirebaseStorage =
        abstract app : App.FirebaseApp
        abstract maxOperationRetryTime : int
        abstract maxUploadRetryTime : int

    /// https://firebase.google.com/docs/reference/js/storage.storagereference.md#storagereference_interface
    type StorageReference =
        abstract bucket : string
        abstract fullPath : string
        abstract name : string
        abstract parent : StorageReference option
        abstract root : StorageReference
        abstract storage : FirebaseStorage
        abstract toString : unit -> string

    /// https://firebase.google.com/docs/reference/js/storage.fullmetadata.md#fullmetadata_interface
    type FullMetadata =
        abstract bucket : string
        abstract downloadTokens : string array option
        abstract fullPath : string
        abstract ref : StorageReference option
        abstract size : int
        abstract timeCreated : string
        abstract updated : string

    /// https://firebase.google.com/docs/reference/js/storage.uploadresult.md#uploadresult_interface
    type UploadResult =
        abstract metadata : FullMetadata
        abstract ref : StorageReference

    type Exports =
        /// https://firebase.google.com/docs/reference/js/storage.md#ref
        [<Import("ref", "firebase/storage")>]
        static member ref (storage : FirebaseStorage, ?path : string) : StorageReference = jsNative

        /// https://firebase.google.com/docs/reference/js/storage.md#uploadbytes
        [<Import("uploadBytes", "firebase/storage")>]
        static member uploadBytes (ref : StorageReference, data : Blob) : Promise<UploadResult> = jsNative

/// https://github.com/andipaetzold/react-firehooks
module Hooks =
    open FireStore

    type ValueHookResult<'T, 'Error> = 'T option * bool * 'Error option

    type Exports =
        /// https://github.com/andipaetzold/react-firehooks#useQuery
        [<Import("useQuery", "react-firehooks/firestore")>]
        static member useQuery<'T> (query : Query<'T>, ?options : obj) : QuerySnapshot<'T> * bool * FirestoreError =
            jsNative

        /// https://github.com/andipaetzold/react-firehooks#useDocumentData
        [<Import("useDocumentData", "react-firehooks/firestore")>]
        static member useDocumentData<'T>
            (
                documentReference : DocumentReference<'T>,
                ?option : obj
            )
            : 'T * bool * FirestoreError
            =
            jsNative

        /// https://github.com/andipaetzold/react-firehooks#useAuthState
        [<Import("useAuthState", "react-firehooks/auth")>]
        static member useAuthState (auth : Auth.Auth) : Auth.User option * bool * FirestoreError option = jsNative

        /// https://github.com/andipaetzold/react-firehooks#useauthidtoken
        [<Import("useAuthIdTokenResult", "react-firehooks/auth")>]
        static member useAuthIdTokenResult
            (auth : Auth.Auth)
            : Auth.IdTokenResult option * bool * FirestoreError option
            =
            jsNative
