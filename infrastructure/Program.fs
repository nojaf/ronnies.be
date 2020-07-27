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
                      inputMap [ "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                                 "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
                                 "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl
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

    let azure =
        let credentials =
            Microsoft.Azure.Management.ResourceManager.Fluent.Authentication.AzureCredentialsFactory()
                     .FromServicePrincipal(System.Environment.GetEnvironmentVariable("ARM_CLIENT_ID"),
                                           System.Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET"),
                                           System.Environment.GetEnvironmentVariable("ARM_TENANT_ID"),
                                           AzureEnvironment.AzureGlobalCloud)

        Microsoft.Azure.Management.Fluent.Azure.Authenticate(credentials)
                 .WithSubscription(System.Environment.GetEnvironmentVariable("ARM_SUBSCRIPTION_ID"))

    let addFunctionKeyHeaderPolicy functionName =
        output {
            let! appId = app.Id
            let! functionApp = azure.AppServices.FunctionApps.GetByIdAsync(appId)

            let functionKey =
                functionApp.ListFunctionKeys(functionName).["default"]

            return sprintf """<set-header name="x-functions-key" exists-action="override">
    <value>%s</value>
</set-header>"""    functionKey
        }

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

    let apiPolicyContent = """<policies>
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
    </inbound>
    <outbound>
        <base />
    </outbound>
</policies>
"""

    let _apiPolicy =
        ApiPolicy
            ("ronnies-policy",
             ApiPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  XmlContent = input apiPolicyContent))

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


    let getEventsOperation =
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

    let getEventsPolicyContent =
        output {
            let! functionKeyHeader = addFunctionKeyHeaderPolicy "get-events"

            return sprintf """
<policies>
    <inbound>
        <base />
        %s
    </inbound>
    <outbound>
        <base />
    </outbound>
</policies>
"""                 functionKeyHeader
        }

    let _getEventsOperationPolicy =
        ApiOperationPolicy
            ("get-events-policy",
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io getEventsOperation.OperationId,
                  XmlContent = io getEventsPolicyContent))

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

    let addEventsPolicyContent =
        output {
            let! functionKeyHeader = addFunctionKeyHeaderPolicy "add-events"

            return sprintf """
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
                        %s
                    </inbound>
                    <outbound>
                        <base />
                    </outbound>
                </policies>
                """ functionKeyHeader
        }

    let _addEventsPolicy =
        ApiOperationPolicy
            ("add-events-policy",
             ApiOperationPolicyArgs
                 (ResourceGroupName = io apimRgName,
                  ApiManagementName = io apimServiceName,
                  ApiName = io api.Name,
                  OperationId = io addEventsOperation.OperationId,
                  XmlContent = io addEventsPolicyContent))

    dict []

[<EntryPoint>]
let main _ = Deployment.run infra
