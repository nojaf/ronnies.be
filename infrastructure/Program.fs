module Program

open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
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

    let applicationsInsight =
        Insights
            ("ai-ronnies",
             InsightsArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input "ai-ronnies", ApplicationType = input "web"))


    let appServicePlan =
        Plan
            ("azfun-ronnies",
             PlanArgs
                 (ResourceGroupName = io resourceGroup.Name, Kind = input "FunctionApp",
                  Sku = input (PlanSkuArgs(Tier = input "Dynamic", Size = input "Y1")),
                  Name = input "azfun-ronnies-plan"))

    let app =
        FunctionApp
            ("azfun-ronnies-plan",
             FunctionAppArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input "azfun-ronnies",
                  AppServicePlanId = io appServicePlan.Id,
                  StorageConnectionString = io storageAccount.PrimaryConnectionString,
                  AppSettings =
                      inputMap
                          [ "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                            "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey ],
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
