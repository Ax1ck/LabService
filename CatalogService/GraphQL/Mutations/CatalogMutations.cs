using CatalogService.Data;
using CatalogService.Models;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.GraphQL.Mutations
{
    public class CatalogMutations
    {
        public async Task<Product> AddProduct(
            [Service] CatalogDbContext context,
            string name,
            string description,
            decimal price,
            int categoryId)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProduct(
            [Service] CatalogDbContext context,
            int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return false;

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
    }
}