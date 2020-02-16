module Program

open Pulumi.FSharp
open Pulumi.Azure.Core
open Pulumi.Azure.Storage


let infra() =

    // Create an Azure Resource Group
    let resourceGroupArgs = ResourceGroupArgs(Name = input "rg-ronnies")
    let resourceGroup = ResourceGroup("rg-ronnies", args = resourceGroupArgs)

    // Create an Azure Storage Account
    let storageAccount =
        Account
            ("storageronnies",
             AccountArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input "storronnies",
                  AccountReplicationType = input "LRS", AccountTier = input "Standard"))

    // Export the connection string for the storage account
    dict [ ("connectionString", storageAccount.PrimaryConnectionString :> obj) ]

[<EntryPoint>]
let main _ =
    Deployment.run infra
