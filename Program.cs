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
        public static Dictionary<int, string> _categories = new Dictionary<int, string>()
        {
            { 1, "Bikes, Touring Bikes" },
            { 2, "Bikes, Road Bikes" },
            { 3, "Components, Saddles" },
            { 4, "Components, Pedals" },
            { 5, "Components, Bottom Brackets" },
            { 6, "Components, Headsets" },
            { 7, "Components, Road Frames" },
            { 8, "Accessories, Bottles and Cages" },
            { 9, "Accessories, Tires and Tubes" },
            { 10, "Accessories, Baskets" },
            { 11, "Accessories, Helmets" },
            { 12, "Clothing, Vests" },
            { 13, "Clothing, Tights" },
            { 14, "Clothing, Gloves" },
            { 15, "Clothing, Shoes" },
            { 16, "Clothing, Jackets" },
        };
        
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new CosmosClientOptions() { AllowBulkExecution = true, ConnectionMode = ConnectionMode.Direct };
            var client = new CosmosClient(config.GetConnectionString("CosmosConnection"), options);
            var container = client.GetDatabase("cosmicworks").GetContainer("products-scale");

            var faker = new Faker("en")
            {
                Random = new Randomizer(42) // Seed value
            };

            var numDocsToWrite = 200000;
            var batchSize = 100;
            var sleep = 100;

            var numWritten = 0;

            Console.WriteLine($"Welcome to the Cosmos DB Bulk Loader. \n\nWriting {numDocsToWrite} records...");
            while (numWritten <= numDocsToWrite)
            {
                Console.WriteLine($"Writing {batchSize} more records... total written so far {numWritten}");
                var cost = 0.0;
                var errors = 0;

                List<Task> concurrentTasks = new List<Task>();
                var products = GenerateRandomProducts(batchSize);
                foreach (var product in products)
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
                Console.WriteLine($"Documents written this batch:{batchSize-errors}   Cost: {cost}   Errors: {errors} \n");

                Thread.Sleep(sleep);
            }

            Console.WriteLine($"Finished writing {numWritten} records.");
        }

        internal static List<Product> GenerateRandomProducts(int numberOfDocumentsPerBatch)
        {
            var tagFaker = new Faker<Tag>()
                .StrictMode(true)
                .RuleFor(t => t.id, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.name, f => f.Random.Word());

            var productFaker = new Faker<Product>()
                .StrictMode(true)
                .RuleFor(t => t.id, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.categoryId, f => f.Random.Int(1, 16).ToString())
                .RuleFor(t => t.categoryName, (f, m) => _categories[int.Parse(m.categoryId)])
                .RuleFor(t => t.sku, f => f.Random.AlphaNumeric(6))
                .RuleFor(p => p.name, f => f.Commerce.ProductName())
                .RuleFor(t => t.description, f => f.Commerce.ProductDescription())
                .RuleFor(t => t.price, f => f.Finance.Amount())
                .RuleFor(t => t.tags, f => tagFaker.Generate(f.Random.Int(15, 50)));

            var products = productFaker.Generate(numberOfDocumentsPerBatch, null);

            return products;
        }
    }
}
