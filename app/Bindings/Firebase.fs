namespace Firebase

open System
open Fable.Core
open Fable.Core.JS

#nowarn "1182"

module App =

    type FirebaseApp =
        interface
        end

module Auth =
    /// https://firebase.google.com/docs/reference/js/auth.userinfo.md#userinfo_interface
    type UserInfo =
        abstract member uid : string
        abstract member displayName : string

    /// https://firebase.google.com/docs/reference/js/auth.user.md#user_interface
    type User =
        inherit UserInfo
        abstract member isAnonymous : bool

    type IdTokenResult =
        abstract member claims : obj

    type Auth =
        abstract member app : App.FirebaseApp
        abstract member currentUser : User option

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

    type FireStore =
        interface
        end

    type Query<'T> =
        abstract member converter : obj
        abstract member firestore : FireStore
        abstract member ``type`` : string

    type CollectionReference<'T> =
        inherit Query<'T>

    type SnapshotMetadata =
        abstract member fromCache : bool
        abstract member hasPendingWrites : bool

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

    type QueryConstraint =
        interface
        end

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
        [<Import("getFirestore", "firebase/firestore")>]
        static member getFirestore (?app : FirebaseApp) : FireStore = jsNative

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

        [<Import("query", "firebase/firestore")>]
        static member query<'T>
            (
                query : Query<'T>,
                [<ParamArray>] queryConstraints : QueryConstraint array
            )
            : Query<'T>
            =
            jsNative

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

        [<Import("setDoc ", "firebase/firestore")>]
        static member setDoc<'T> (reference : DocumentReference<'T>, data : 'T) : Promise<unit> = jsNative

        [<Import("addDoc", "firebase/firestore")>]
        static member addDoc<'T> (reference : CollectionReference<'T>, data : 'T) : Promise<DocumentReference<'T>> =
            jsNative

        [<Import("updateDoc", "firebase/firestore")>]
        static member updateDoc<'T, 'V> (reference : DocumentReference<'T>, data : 'V) : Promise<unit> = jsNative

/// https://github.com/andipaetzold/react-firehooks
module Hooks =
    open FireStore

    type ValueHookResult<'T, 'Error> = 'T option * bool * 'Error option

    type Exports =
        /// https://github.com/andipaetzold/react-firehooks/blob/main/src/firestore/useCollection.ts
        [<Import("useCollection", "react-firehooks/firestore")>]
        static member useCollection<'T>
            (
                query : Query<'T>,
                ?options : obj
            )
            : QuerySnapshot<'T> * bool * FirestoreError
            =
            jsNative

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
