using System;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class ElasticSearchLogger : LoggerServiceBase
    {
        public ElasticSearchLogger()
        {
            try
            {
                var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
                var elasticConfig = configuration.GetSection("ElasticSearchConfig").Get<ElasticSearchOptions>();

                // Eğer konfigürasyon yoksa veya placeholder (yer tutucu) halindeyse işlemi sessizce geç
                if (elasticConfig == null || 
                    string.IsNullOrEmpty(elasticConfig.ConnectionString) || 
                    elasticConfig.ConnectionString.StartsWith("{"))
                {
                    Logger = new LoggerConfiguration().CreateLogger(); // Boş logger
                    return;
                }

                Logger = new LoggerConfiguration()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticConfig.ConnectionString))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = "siparis-sistemi-log-{0:yyyy.MM.dd}",
                        ModifyConnectionSettings = x => x.BasicAuthentication(elasticConfig.UserName, elasticConfig.Password),
                        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog // Hata durumunda uygulamayı bozma
                    })
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                // Kritik: Hata durumunda uygulamayı çökertme, sadece Debug çıktısına yaz
                System.Diagnostics.Debug.WriteLine($"ElasticSearchLogger Connection Error: {ex.Message}");
                Logger = new LoggerConfiguration().CreateLogger(); // Yedek boş logger
            }
        }
    }

    public class ElasticSearchOptions
    {
        public string ConnectionString { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
