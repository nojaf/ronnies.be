module Program

open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
open Pulumi.FSharp
open Pulumi.Azure.Core
open Pulumi.Azure.Storage


let infra() =
    let stackName = Pulumi.Deployment.Instance.StackName

    // Create an Azure Resource Group
    let resourceGroupArgs = ResourceGroupArgs(Name = input (sprintf "rg-ronnies-%s" stackName))
    let resourceGroup = ResourceGroup(sprintf "rg-ronnies-%s" stackName, args = resourceGroupArgs)

    // Create an Azure Storage Account
    let storageAccount =
        Account
            ("storageronnies",
             AccountArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "storronnies%s" stackName),
                  AccountReplicationType = input "LRS", AccountTier = input "Standard"))

    let applicationsInsight =
        Insights
            ("ai-ronnies",
             InsightsArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "ai-ronnies-%s" stackName),
                  ApplicationType = input "web"))


    let appServicePlan =
        Plan
            ("azfun-ronnies",
             PlanArgs
                 (ResourceGroupName = io resourceGroup.Name, Kind = input "FunctionApp",
                  Sku = input (PlanSkuArgs(Tier = input "Dynamic", Size = input "Y1")),
                  Name = input (sprintf "azfun-ronnies-plan-%s" stackName)))

    let app =
        FunctionApp
            ("azfun-ronnies-plan",
             FunctionAppArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "azfun-ronnies-%s" stackName),
                  AppServicePlanId = io appServicePlan.Id,
                  StorageConnectionString = io storageAccount.PrimaryConnectionString,
                  AppSettings =
                      inputMap
                          [ "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                            "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
                            "StorageAccountKey", io storageAccount.PrimaryAccessKey
                            "StorageAccountName", io storageAccount.Name ],
                  SiteConfig =
                      input
                          (FunctionAppSiteConfigArgs
                              (Http2Enabled = input true,
                               Cors =
                                   input
                                       (FunctionAppSiteConfigCorsArgs
                                           (AllowedOrigins =
                                               inputList
                                                   [ input "https://ronnies.be"
                                                     input "http://localhost:3000" ])))), HttpsOnly = input true,
                  Version = input "~3"))

    // Export the connection string for the storage account
    dict
        [ ("connectionString", storageAccount.PrimaryConnectionString :> obj)
          ("endpointHostName", app.DefaultHostname :> obj) ]

[<EntryPoint>]
let main _ =
    Deployment.run infra
