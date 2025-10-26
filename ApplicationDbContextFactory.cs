// StarEvents/ApplicationDbContextFactory.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StarEvents.Data;
using System.IO;

// This factory class tells the 'Update-Database' and 'Add-Migration' commands
// how to create an instance of ApplicationDbContext at design time.
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 1. Build configuration to access the connection string
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json") // Ensure this file is present
            .Build();

        // 2. Retrieve the connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 3. Configure DbContextOptions
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(connectionString);

        // 4. Return the DbContext instance
        return new ApplicationDbContext(builder.Options);
    }
}