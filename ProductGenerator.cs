using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace BulkLoadCosmos
{
    internal class ProductGenerator
    {

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
                .RuleFor(t => t.categoryName, (f, m) => Categories.Find(x => x.categoryId == m.categoryId).categoryName)
                .RuleFor(t => t.sku, f => f.Random.AlphaNumeric(6))
                .RuleFor(p => p.name, f => f.Commerce.ProductName())
                .RuleFor(t => t.description, f => f.Commerce.ProductDescription())
                .RuleFor(t => t.price, f => f.Finance.Amount())
                .RuleFor(t => t.tags, f => tagFaker.Generate(f.Random.Int(15, 50)));

            var products = productFaker.Generate(numberOfDocumentsPerBatch, null);

            return products;
        }

        public static List<Category> Categories => new List<Category>
            {
                new Category{ categoryId="1", categoryName= "Bikes, Touring Bikes"},
                new Category{ categoryId="2", categoryName= "Bikes, Road Bikes"},
                new Category{ categoryId="3", categoryName= "Components, Saddles"},
                new Category{ categoryId="4", categoryName= "Components, Pedals"},
                new Category{ categoryId="5", categoryName= "Components, Bottom Brackets"},
                new Category{ categoryId="6", categoryName= "Components, Headsets"},
                new Category{ categoryId="7", categoryName= "Components, Road Frames"},
                new Category{ categoryId="8", categoryName= "Accessories, Bottles and Cages"},
                new Category{ categoryId="9", categoryName= "Accessories, Tires and Tubes"},
                new Category{ categoryId="10", categoryName= "Accessories, Baskets"},
                new Category{ categoryId="11", categoryName= "Accessories, Helmets"},
                new Category{ categoryId="12", categoryName= "Clothing, Vests"},
                new Category{ categoryId="13", categoryName= "Clothing, Tights"},
                new Category{ categoryId="14", categoryName= "Clothing, Gloves"},
                new Category{ categoryId="15", categoryName= "Clothing, Shoes"},
                new Category{ categoryId="16", categoryName= "Clothing, Jackets"},
            };

        

    }
}
