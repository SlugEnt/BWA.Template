using Global;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.BWA.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Get Sensitive Appsettings.json file location
            string? sensitiveAppSettings = ApplicationGlobals.GetSensitiveAppSettingFullPath();

            // Add our custom AppSettings.JSON files
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile(sensitiveAppSettings, true, true).Build();

            Console.WriteLine("AppDbContext Design Time Tools:  Sensitive File: " + sensitiveAppSettings);
            if (!File.Exists(sensitiveAppSettings))
            {
                throw new ApplicationException("Sensitive Setting File does not exist.");
            }

            DbContextOptionsBuilder<AppDbContext> builder          = new();
            string?                               connectionString = configuration.GetConnectionString(AppDbContext.DatabaseReferenceName());
            if (connectionString == null)
            {
                throw new ApplicationException("Connection String Not Found!");
            }

            Console.WriteLine("Connection String Was Found!" + connectionString!.Substring(0, 15) + "...");

            builder.UseSqlServer(connectionString);
            return new AppDbContext(builder.Options);
        }
    }
}