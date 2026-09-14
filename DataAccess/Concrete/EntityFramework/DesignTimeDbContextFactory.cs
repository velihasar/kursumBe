using System;
using System.IO;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.EntityFramework
{
    /// <summary>
    /// Design-time (Migration komutları) sırasında DbContext'in nasıl ayağa kalkacağını tanımlar.
    /// .env dosyasındaki şifreleri okuyarak manuel Update-Database hatalarını çözer.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
    {
        public ProjectDbContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            
            // WebAPI klasörünü bulmaya çalış (Hangi klasörde olursak olalım)
            var webApiDirectory = currentDirectory;
            if (webApiDirectory.EndsWith("DataAccess")) 
                webApiDirectory = Path.Combine(currentDirectory, "..", "WebAPI");
            else if (!webApiDirectory.EndsWith("WebAPI") && Directory.Exists(Path.Combine(currentDirectory, "WebAPI")))
                webApiDirectory = Path.Combine(currentDirectory, "WebAPI");

            // Ortama göre .env dosyasını yükle
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            DotNetEnv.Env.Load(Path.Combine(webApiDirectory, $".env.{envName}"));
            DotNetEnv.Env.Load(Path.Combine(webApiDirectory, ".env"));

            // Configuration'ı oku ve Environment değişkenlerini dahil et
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(webApiDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<ProjectDbContext>();
            var connectionStringTemplate = configuration.GetConnectionString("DArchPgContext");
            
            // Yer tutucuyu elle değiştir (Çünkü tasarım anında Program.cs çalışmaz)
            var connectionString = connectionStringTemplate;
            if (connectionString.Contains("{DARCH_PG_CONN}"))
            {
                var envValue = Environment.GetEnvironmentVariable("DARCH_PG_CONN");
                if (!string.IsNullOrEmpty(envValue))
                {
                    connectionString = connectionString.Replace("{DARCH_PG_CONN}", envValue);
                }
            }

            builder.UseNpgsql(connectionString);

            return new ProjectDbContext(builder.Options, configuration);
        }
    }
}
