module Functions

open Firebase.Admin
open Firebase.Functions
open Firebase.Functions.V2

val user : Https.HttpsFunction
val sudo : Https.HttpsFunction
val users : Https.CallableFunction
val cleanUpUsers : Https.HttpsFunction
val locationCreated : FireStore.CloudFunction
