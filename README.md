# BlazorMapsCreator
A simple GUI PoC for Azure Maps Creator built with Blazor and Fluent UI

## How to run locally?

### Prerequisites
1. [Make an Azure Maps account](https://docs.microsoft.com/en-us/azure/azure-maps/quick-demo-map-app#create-an-azure-maps-account).
2. [Obtain a primary subscription key](https://docs.microsoft.com/en-us/azure/azure-maps/quick-demo-map-app.md#get-the-primary-key-for-your-account), also known as the primary key or the subscription key.
3. [Create a Creator resource](https://docs.microsoft.com/en-us/azure/azure-maps/how-to-manage-creator.md).
4. Download the [Sample Drawing package](https://github.com/Azure-Samples/am-creator-indoor-data-examples/blob/master/Sample%20-%20Contoso%20Drawing%20Package.zip).
5. You will need to be able to compile with .NET 6 Preview 5 (at the moment). Please ensure this version is installed on your machine. If a newer version is released, follow the steps to upgrade in the announcement blog post.

### Set up your environment
- For the app to work, you will need configure your azure maps. Under `BlazorMapoCreator\BlazorMapsCreator` in your cloned version of this repo, create a `appsettings.Development.json` file. This will be automatically ignored by the `.gitignore` file, so you can safely store your authentication information in it. **Please be sure not to host any of your keys on your GitHub repository**. The app expects the configuration to be found under an `AzureMaps` entry, so your file should contain at least the following : 

```
{
    "AzureMaps": {
      "SubscriptionKey": "<Your subscription key>",
      "Geography": "<eu or us>"
    }
}
