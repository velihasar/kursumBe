using Business.Handlers.FeeDues.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Her ayın başında aktif kurs kayıtları için aidat tahakkuklarını (FeeDue) otomatik oluşturan arka plan servisi.
    /// </summary>
    public class MonthlyFeeDueBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonthlyFeeDueBackgroundService> _logger;

        public MonthlyFeeDueBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<MonthlyFeeDueBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[MonthlyFeeDueBackgroundService] Otomatik Aidat Tahakkuk Servisi başlatıldı.");

            // Uygulama ayağa kalktıktan 1 dakika sonra ilk kontrolü yap
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessMonthlyFeeDuesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[MonthlyFeeDueBackgroundService] Aidat tahakkuku oluşturulurken hata meydana geldi.");
                }

                // 24 saatte bir kontrol et
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessMonthlyFeeDuesAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            string currentPeriod = $"{now.Year}-{now.Month:D2}";

            _logger.LogInformation($"[MonthlyFeeDueBackgroundService] {currentPeriod} dönemi için otomatik aidat kontrolü başlatılıyor...");

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            // Tüm kurumlar için mevcut ayın aidatlarını otomatik oluştur
            var result = await mediator.Send(new GenerateMonthlyFeeDuesCommand
            {
                Period = currentPeriod,
                TenantId = null // Tüm kurumlar
            }, cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation($"[MonthlyFeeDueBackgroundService] {result.Message}");
            }
            else
            {
                _logger.LogWarning($"[MonthlyFeeDueBackgroundService] Tahakkuk oluşturulamadı: {result.Message}");
            }
        }
    }
}
