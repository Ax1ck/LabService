using HotChocolate.Types;

namespace CatalogService.GraphQL.Types
{
    public class CategoryType : ObjectType<Category>
    {
        protected override void Configure(IObjectTypeDescriptor<Category> descriptor)
        {
            descriptor.Description("Represents a product category");
            descriptor.Field(c => c.Id).Description("Category ID");
            descriptor.Field(c => c.Name).Description("Category name");
            descriptor.Field(c => c.Products)
                .Description("Products in this category")
                .UseFiltering()
                .UseSorting();
        }
    }
}