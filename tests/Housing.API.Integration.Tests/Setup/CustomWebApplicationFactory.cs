using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Housing.API.Integration.Tests.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault
                  (d => d.ServiceType == typeof(DbContextOptions<FeedbackDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<FeedbackDbContext>
                 ((_, context) => context.UseInMemoryDatabase("Feedback"));

                // Build the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using var scope = serviceProvider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<FeedbackDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                    db.InitializeTestDatabase();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the databasewith test messages.Error: {ex.Message}");
                }
            });
        }
    }
}
