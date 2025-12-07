using UserCore.Data;
using Microsoft.EntityFrameworkCore;

namespace UserCore.Extensions;

public static class MigrationExtensions
{
    public static async void ApplyMigration(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        await using DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        //await dataContext.Database.EnsureDeletedAsync();
        await dataContext.Database.MigrateAsync();
    }
}