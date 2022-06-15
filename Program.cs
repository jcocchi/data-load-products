using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace BulkLoadCosmos
{
    
    class Program
    {
        static async Task Main(string[] args)
        {
            CosmosClient client = GetCosmosClient();

            var container = client.GetDatabase("cosmicworks").GetContainer("products-scale");

            await CreateDocuments(container, documentsToCreate: 200000, batchSize: 200, sleep: 100);
        }

        private static async Task CreateDocuments(Container container, int documentsToCreate, int batchSize, int sleep, int numWritten = 0)
        {
            Console.WriteLine($"Welcome to the Cosmos DB Bulk Loader. \n\nWriting {documentsToCreate} records...");

            while (numWritten < documentsToCreate)
            {
                numWritten = await CreateDocumentsBatch(container, batchSize, numWritten);

                Thread.Sleep(sleep);
            }

            Console.WriteLine($"Finished writing {numWritten} records.");
        }

        private static async Task<int> CreateDocumentsBatch(Container container, int batchSize, int numWritten)
        {
            Console.WriteLine($"Writing {batchSize} more records... total written so far {numWritten}");
            var cost = 0.0;
            var errors = 0;

            List<Task> concurrentTasks = new();
            foreach (var product in ProductGenerator.GenerateRandomProducts(batchSize))
            {
                concurrentTasks.Add(container.CreateItemAsync(product, new PartitionKey(product.categoryId)).ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cost += t.Result.RequestCharge;
                    }
                    else
                    {
                        Console.WriteLine($"Error creating document: {t.Exception.Message}");
                        errors++;
                    }
                }));
            }

            await Task.WhenAll(concurrentTasks);

            numWritten += batchSize - errors;
            Console.WriteLine($"Documents written this batch:{batchSize - errors}   Cost: {cost}   Errors: {errors} \n");
            return numWritten;
        }

        private static CosmosClient GetCosmosClient()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            CosmosClient client = new CosmosClient(config["uri"], config["key"]);
            return client;
        }
    }
}
