# Azure Cosmos DB Bulk Loader

This project generates fictional product data using [Bogus](https://github.com/bchavez/Bogus) and loads it into Azure Cosmos DB.

## Setup

1. Create an appsettings.json file with the following content.

```json
{
  "uri": "<add the uri here>",
  "key": "<add the key here>"
}
```

1. Create an Azure Cosmos DB SQL API account following [these instructions](https://docs.microsoft.com/azure/cosmos-db/sql/how-to-create-account).

1. Add a database named `cosmicworks` and a container named `products-scale`.

## Run the Application

You can run the application by entering `Ctrl + F5` in Visual Studio or entering `dotnet run` in the command line.

> Note: The application defaults to writing 200,000 products in batches of 100 items. These settings are configurable by changing line 20 (or 24 to change default values) `Program.cs`.
>
> ```c#
> private static async Task CreateDocuments(Container container, int documentsToCreate = 500, int batchSize = 100, int sleep = 100, int numWritten = 0) ; //Line 24: Change default values here
>
> await CreateDocuments(container, documentsToCreate: 20000, batchSize: 200, sleep: 100); //Line 20: Change the values here
> ```



        
   
