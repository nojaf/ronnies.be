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
    let stackName = Pulumi.Deployment.Instance.StackName

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
                      inputMap [
                          "AzureWebJobsSecretStorageType", input "Files"
                          "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                          "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
                          "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl
                          "StorageAccountKey", io storageAccount.PrimaryAccessKey
                          "StorageAccountName", io storageAccount.Name
                          "Auth0Management_ClientId", input (System.Environment.GetEnvironmentVariable("Auth0Management_ClientId"))
                          "Auth0Management_ClientSecret", input (System.Environment.GetEnvironmentVariable("Auth0Management_ClientSecret"))
                          "Auth0Management_Audience", input (System.Environment.GetEnvironmentVariable("Auth0Management_Audience"))
                          "Auth0Management_APIRoot", input (System.Environment.GetEnvironmentVariable("Auth0Management_APIRoot"))
                          "Vapid_Subject", input (System.Environment.GetEnvironmentVariable("Vapid_Subject"))
                          "Vapid_PublicKey", input (System.Environment.GetEnvironmentVariable("Vapid_PublicKey"))
                          "Vapid_PrivateKey", input (System.Environment.GetEnvironmentVariable("Vapid_PrivateKey"))
                      ],
                  SiteConfig =
                      input
                          (FunctionAppSiteConfigArgs
                              (Http2Enabled = input true,
                               Cors =
                                   input
                                       (FunctionAppSiteConfigCorsArgs
                                           (AllowedOrigins =
                                               inputList [
                                                   input "https://ronnies.be"
                                                   input "http://localhost:8080"
                                               ])))),
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

    let _logger =
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
                      inputMap [
                          "apim", io apimServiceName
                          "productId", io product.ProductId
                          "primaryKey", input (System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY"))
                      ],
                  DeploymentMode = input "Incremental"))

    let _getEventsOperation =
        ApiOperation
            ("get-events",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "api/get-events",
                  Method = input "GET",
                  DisplayName = input "Get events",
                  OperationId = input "get-events",
                  Request =
                      input
                          (ApiOperationRequestArgs
                              (QueryParameters =
                                  inputList
                                      [ input
                                          (ApiOperationRequestQueryParameterArgs
                                              (Name = input "lastEvent",
                                               Required = input false,
                                               Type = input "integer",
                                               Description = input "Last event number the client already has")) ]))))

    let addEventsOperation =
        ApiOperation
            ("add-events",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "api/add-events",
                  Method = input "POST",
                  DisplayName = input "Add events",
                  OperationId = input "add-events"))

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

    let _addEventsPolicy =
        ApiOperationPolicy
            ("add-events-policy",
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io addEventsOperation.OperationId,
                  XmlContent = input authenticatedPolicyContent))

    let addSubscriptionOperation =
        ApiOperation
            ("add-subscription",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "api/add-subscription",
                  Method = input "POST",
                  DisplayName = input "Add subscription",
                  OperationId = input "add-subscription"))

    let _addSubscriptionPolicy =
        ApiOperationPolicy
            ("add-subscription-policy",
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io addSubscriptionOperation.OperationId,
                  XmlContent = input authenticatedPolicyContent))

    let removeSubscriptionOperation =
        ApiOperation
            ("remove-subscription",
             ApiOperationArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  UrlTemplate = input "api/remove-subscription",
                  Method = input "POST",
                  DisplayName = input "Remove subscription",
                  OperationId = input "remove-subscription"))

    let _removeSubscriptionPolicy =
        ApiOperationPolicy
            ("remove-subscription-policy",
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io removeSubscriptionOperation.OperationId,
                  XmlContent = input authenticatedPolicyContent))

    dict []

[<EntryPoint>]
let main _ = Deployment.run infra
