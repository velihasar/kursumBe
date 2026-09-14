using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // 1. .env dosyalarını yükle (Sistem değişkenlerine ekler)
                    var envName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                    DotNetEnv.Env.Load($".env.{envName}");
                    DotNetEnv.Env.Load();

                    // 2. .env'den gelen yeni değişkenleri konfigürasyona dahil et
                    config.AddEnvironmentVariables();

                    // 3. Mevcut tüm ayarları oku, {VAR} formatındakileri gerçek değerlerle değiştir
                    var tempConfig = config.Build();
                    var updatedSettings = new Dictionary<string, string>();

                    foreach (var entry in tempConfig.AsEnumerable())
                    {
                        var value = entry.Value;
                        if (!string.IsNullOrEmpty(value) && value.Contains("{") && value.Contains("}"))
                        {
                            var startIndex = value.IndexOf('{');
                            var endIndex = value.IndexOf('}');
                            var envKey = value.Substring(startIndex + 1, endIndex - startIndex - 1);
                            var envValue = System.Environment.GetEnvironmentVariable(envKey);

                            if (!string.IsNullOrEmpty(envValue))
                            {
                                var newValue = value.Replace("{" + envKey + "}", envValue);
                                updatedSettings[entry.Key] = newValue;
                            }
                        }
                    }

                    // 4. Güncellenmiş değerleri en üst katman olarak konfigürasyona geri ekle
                    if (updatedSettings.Any())
                    {
                        config.AddInMemoryCollection(updatedSettings);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
    }
}