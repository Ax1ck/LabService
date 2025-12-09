using CatalogService.Data;
using HotChocolate;
using HotChocolate.Data;
using System.Linq;

namespace CatalogService.GraphQL.Queries
{
    public class CatalogQuery
    {
        [UseDbContext(typeof(CatalogDbContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Category> GetCategories([ScopedService] CatalogDbContext context)
            => context.Categories;

        [UseDbContext(typeof(CatalogDbContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> GetProducts([ScopedService] CatalogDbContext context)
            => context.Products;

        [UseDbContext(typeof(CatalogDbContext))]
        public Product? GetProductById([ScopedService] CatalogDbContext context, int id)
            => context.Products.FirstOrDefault(p => p.Id == id);
    }
}