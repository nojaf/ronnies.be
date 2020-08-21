module Program

open Microsoft.Azure.Management.ResourceManager.Fluent
open Pulumi.Azure.ApiManagement.Inputs
open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
open Pulumi.Azure.Core
open Pulumi.FSharp
open Pulumi.Azure.Storage
open Pulumi.Azure.ApiManagement
open Pulumi.FSharp.Output
open Pulumi
open System.IO

let infra () =
    let stackName = Deployment.Instance.StackName

    let resourceGroupName =
        output {
            let name = sprintf "rg-ronnies-%s" stackName
            let! rg = GetResourceGroup.InvokeAsync(GetResourceGroupArgs(Name = name))
            return rg.Name
        }

    // Create an Azure Storage Account
    let storageAccount =
        Account
            ("storageronnies",
             AccountArgs
                 (ResourceGroupName = io resourceGroupName,
                  Name = input (sprintf "storronnies%s" stackName),
                  AccountReplicationType = input "LRS",
                  AccountTier = input "Standard"))

    let applicationsInsight =
        Insights
            ("ai-ronnies",
             InsightsArgs
                 (ResourceGroupName = io resourceGroupName,
                  Name = input (sprintf "ai-ronnies-%s" stackName),
                  ApplicationType = input "web"))


    let appServicePlan =
        Plan
            ("azfun-ronnies",
             PlanArgs
                 (ResourceGroupName = io resourceGroupName,
                  Kind = input "FunctionApp",
                  Sku = input (PlanSkuArgs(Tier = input "Dynamic", Size = input "Y1")),
                  Name = input (sprintf "azfun-ronnies-plan-%s" stackName)))

    let zipContainer =
        Container
            ("zips",
             ContainerArgs
                 (Name = input "zips",
                  StorageAccountName = io storageAccount.Name,
                  ContainerAccessType = input "private"))

    let artifactsFolder =
        Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts")

    let path = Path.Combine(artifactsFolder, "server")
    let archive : AssetOrArchive = FileArchive(path) :> AssetOrArchive

    let blob =
        Blob
            ("server-zip",
             BlobArgs
                 (StorageAccountName = io storageAccount.Name,
                  StorageContainerName = io zipContainer.Name,
                  Type = input "Block",
                  Source = input archive))

    let codeBlobUrl =
        SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount)

    let app =
        FunctionApp
            ("azfun-ronnies-plan",
             FunctionAppArgs
                 (ResourceGroupName = io resourceGroupName,
                  Name = input (sprintf "azfun-ronnies-%s" stackName),
                  AppServicePlanId = io appServicePlan.Id,
                  StorageAccountName = io storageAccount.Name,
                  StorageAccountAccessKey = io storageAccount.PrimaryAccessKey,
                  AppSettings =
                      inputMap [ "AzureWebJobsSecretStorageType", input "Files"
                                 "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                                 "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
                                 "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl
                                 "StorageAccountKey", io storageAccount.PrimaryAccessKey
                                 "StorageAccountName", io storageAccount.Name
                                 "Auth0Management_ClientId",
                                 input (System.Environment.GetEnvironmentVariable("Auth0Management_ClientId"))
                                 "Auth0Management_ClientSecret",
                                 input (System.Environment.GetEnvironmentVariable("Auth0Management_ClientSecret"))
                                 "Auth0Management_Audience",
                                 input (System.Environment.GetEnvironmentVariable("Auth0Management_Audience"))
                                 "Auth0Management_APIRoot",
                                 input (System.Environment.GetEnvironmentVariable("Auth0Management_APIRoot"))
                                 "Vapid_Subject", input (System.Environment.GetEnvironmentVariable("Vapid_Subject"))
                                 "Vapid_PublicKey", input (System.Environment.GetEnvironmentVariable("Vapid_PublicKey"))
                                 "Vapid_PrivateKey",
                                 input (System.Environment.GetEnvironmentVariable("Vapid_PrivateKey"))
                                 "BACKEND", input (System.Environment.GetEnvironmentVariable("BACKEND"))
                                 "SUBSCRIPTION_KEY",
                                 input (System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY")) ],
                  SiteConfig =
                      input
                          (FunctionAppSiteConfigArgs
                              (Http2Enabled = input true,
                               Cors =
                                   input
                                       (FunctionAppSiteConfigCorsArgs
                                           (AllowedOrigins =
                                               inputList [ input "https://ronnies.be"
                                                           input "http://localhost:8080" ])))),
                  HttpsOnly = input true,
                  Version = input "~3"))

    let apimRgName =
        output {
            let nojafAPIMResourceGroupName = sprintf "rg-nojaf-apim-%s" stackName
            let! rg = GetResourceGroup.InvokeAsync(GetResourceGroupArgs(Name = nojafAPIMResourceGroupName))
            return rg.Name
        }

    let apimServiceName =
        output {
            let nojafAPIMResourceGroupName = sprintf "rg-nojaf-apim-%s" stackName
            let! rg = GetResourceGroup.InvokeAsync(GetResourceGroupArgs(Name = nojafAPIMResourceGroupName))
            let nojafAPIMName = "nojaf-apim"
            let! apim = GetService.InvokeAsync(GetServiceArgs(Name = nojafAPIMName, ResourceGroupName = rg.Name))
            return apim.Name
        }

    let logger =
        Logger
            ("ronny-logger",
             LoggerArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApplicationInsights =
                      input
                          (LoggerApplicationInsightsArgs(InstrumentationKey = io applicationsInsight.InstrumentationKey))))

    let api =
        Api
            ("ronnies",
             ApiArgs
                 (Name = input "ronnies",
                  ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  Revision = input "1",
                  DisplayName = input "ronnies.be",
                  Path = input "ronnies",
                  ServiceUrl = io (app.DefaultHostname.Apply(sprintf "https://%s")),
                  Protocols = inputList [ input "https" ],
                  SubscriptionRequired = input true))

    let _diagnostics =
        Diagnostic
            ("ronnyDiagnostics",
             DiagnosticArgs
                 (Identifier = input "applicationinsights",
                  ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiManagementLoggerId = io logger.Id,
                  Enabled = input true))

    let apiPolicyContent () =
        output {
            let! appId = app.Id

            let azure =
                let credentials =
                    Microsoft.Azure.Management.ResourceManager.Fluent.Authentication.AzureCredentialsFactory()
                             .FromServicePrincipal(System.Environment.GetEnvironmentVariable("ARM_CLIENT_ID"),
                                                   System.Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET"),
                                                   System.Environment.GetEnvironmentVariable("ARM_TENANT_ID"),
                                                   AzureEnvironment.AzureGlobalCloud)

                Microsoft.Azure.Management.Fluent.Azure.Authenticate(credentials)
                         .WithSubscription(System.Environment.GetEnvironmentVariable("ARM_SUBSCRIPTION_ID"))

            let! functionApp = azure.AppServices.FunctionApps.GetByIdAsync(appId)

            let! masterKey = functionApp.GetMasterKeyAsync()
            // let masterKey = "master"

            let policy =
                sprintf """<policies>
                <inbound>
                    <base />
                    <cors allow-credentials="true">
                        <allowed-origins>
                            <origin>http://localhost:8080/</origin>
                            <origin>https://ronnies.be/</origin>
                        </allowed-origins>
                        <allowed-methods preflight-result-max-age="300">
                            <method>GET</method>
                            <method>POST</method>
                            <method>DELETE</method>
                        </allowed-methods>
                        <allowed-headers>
                            <header>*</header>
                        </allowed-headers>
                    </cors>
                    <set-header name="x-functions-key" exists-action="override">
                        <value>%s</value>
                    </set-header>
                </inbound>
                <outbound>
                    <base />
                </outbound>
            </policies>
            """  masterKey

            return policy
        }

    let _apiPolicy =
        ApiPolicy
            ("ronnies-policy",
             ApiPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  XmlContent = io (apiPolicyContent ())))

    let product =
        Product
            ("ronnies",
             ProductArgs
                 (ProductId = input "ronnies",
                  ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  DisplayName = input "Ronnies",
                  SubscriptionRequired = input true,
                  SubscriptionsLimit = input 1,
                  ApprovalRequired = input true,
                  Published = input true))

    let _productApi =
        ProductApi
            ("ronnies",
             ProductApiArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  ProductId = io product.ProductId))

    // Pulumi makes the user id required for subscriptions.
    // In case of the consumption tier, users are not allowed.
    // So ARM template serves as a workaround for this limitation.
    let subscriptionArm = """
    {
        "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
        "contentVersion": "1.0.0.0",
        "parameters":{
            "apim": {
                "type":"string"
            },
            "productId":{
                "type":"string"
            },
            "primaryKey":{
                "type":"string"
            }
        },
        "resources": [{
            "apiVersion": "2019-12-01",
            "name": "[concat(parameters('apim') , '/rudi-sub')]",
            "type": "Microsoft.ApiManagement/service/subscriptions",
            "properties": {
                "scope": "[concat('/products/', parameters('productId'))]",
                "primaryKey": "[parameters('primaryKey')]",
                "displayName": "Rudi subscription"
            }
        }]
    }
"""

    let _subscriptionDeployment =
        TemplateDeployment
            ("rudi-sub",
             TemplateDeploymentArgs
                 (ResourceGroupName = io apimRgName,
                  TemplateBody = input subscriptionArm,
                  Parameters =
                      inputMap [ "apim", io apimServiceName
                                 "productId", io product.ProductId
                                 "primaryKey", input (System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY")) ],
                  DeploymentMode = input "Incremental"))

    let _getEventsOperation =
        ApiOperation
            ("get-events",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "events",
                  Method = input "GET",
                  DisplayName = input "Get events",
                  OperationId = input "get-events",
                  Request =
                      input
                          (ApiOperationRequestArgs
                              (QueryParameters =
                                  inputList [ input
                                                  (ApiOperationRequestQueryParameterArgs
                                                      (Name = input "lastEvent",
                                                       Required = input false,
                                                       Type = input "integer",
                                                       Description = input "Last event number the client already has")) ]))))

    let authenticatedPolicyContent = """
<policies>
    <inbound>
        <base />
        <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="@((string)context.LastError.Message)" require-scheme="Bearer" require-signed-tokens="true">
            <openid-config url="https://nojaf.eu.auth0.com/.well-known/openid-configuration" />
            <audiences>
                <audience>https://ronnies.be</audience>
            </audiences>
            <issuers>
                <issuer>https://nojaf.eu.auth0.com/</issuer>
            </issuers>
            <required-claims>
                <claim name="https://ronnies.be/roles" match="any">
                    <value>admin</value>
                    <value>editor</value>
                </claim>
            </required-claims>
        </validate-jwt>
    </inbound>
    <outbound>
        <base />
    </outbound>
</policies>"""

    let authenticatedPolicy functionName (operation : ApiOperation) =
        ApiOperationPolicy
            (sprintf "%s-policy" functionName,
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io operation.OperationId,
                  XmlContent = input authenticatedPolicyContent))

    let authenticatedOperation functionName method urlTemplate displayName =
        let operation =
            ApiOperation
                (functionName,
                 ApiOperationArgs
                     (ResourceGroupName = io apimRgName,
                      ApiManagementName = io apimServiceName,
                      ApiName = io api.Name,
                      UrlTemplate = input urlTemplate,
                      Method = input method,
                      DisplayName = input displayName,
                      OperationId = input functionName))

        authenticatedPolicy functionName operation

    let _addEventsOperation =
        authenticatedOperation "add-events" "POST" "events" "Add events"

    let _addSubscriptionOperation =
        authenticatedOperation "add-subscription" "POST" "subscriptions" "Add subscription"

    let _removeSubscriptionOperation =
        authenticatedOperation "remove-subscription" "DELETE" "subscriptions" "Remove subscription"

    let _getAllUsersOperation =
        authenticatedOperation "get-users" "GET" "users" "Get all user information"

    let _getUserByIdOperation =
        let functionName = "get-user"

        let operation =
            ApiOperation
                (functionName,
                 ApiOperationArgs
                     (ResourceGroupName = io apimRgName,
                      ApiManagementName = io apimServiceName,
                      ApiName = io api.Name,
                      UrlTemplate = input "users/{id}",
                      Method = input "GET",
                      DisplayName = input "Get user information",
                      OperationId = input functionName,
                      TemplateParameters =
                          inputList [ input
                                          (ApiOperationTemplateParameterArgs
                                              (Name = input "id",
                                               Required = input true,
                                               Type = input "string",
                                               Description = input "Auth0 user_id")) ]))

        authenticatedPolicy functionName operation

    let _getPingOperation =
        ApiOperation
            ("ping",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "ping",
                  Method = input "GET",
                  DisplayName = input "Ping",
                  OperationId = input "ping"))

    dict []

[<EntryPoint>]
let main _ = Deployment.run infra
