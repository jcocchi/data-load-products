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

1. Add a database named `cosmicworks` and a container named `products-scale` with `categoryId` as the partition key.

## Run the Application

You can run the application by entering `Ctrl + F5` in Visual Studio or entering `dotnet run` in the command line.

> Note: The application defaults to writing 200,000 products in batches of 200 items. These settings are configurable by changing line 20 of `Program.cs`.
>
> ```c#
> await CreateDocuments(container, documentsToCreate: 200000, batchSize: 200, sleep: 100); //Line 20: Change the values here
> ```
