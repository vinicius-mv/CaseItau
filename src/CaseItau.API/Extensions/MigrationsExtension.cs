using CaseItau.Infra;
using Microsoft.EntityFrameworkCore;

namespace CaseItau.API.Extensions;

public static class MigrationsExtension
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
