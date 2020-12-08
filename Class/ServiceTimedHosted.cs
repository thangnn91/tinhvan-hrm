using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace HRScripting.Services
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private Utils.ToolFolder folder;
        private Timer _timer;

        public TimedHostedService()
        {
            folder = new Utils.ToolFolder("appsettings.json");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int TimeBg;
            try { TimeBg = int.Parse(folder.ReadConfigToJson().System.TimeBackground.ToString()); } catch { TimeBg = 60; }
            folder.WriteLogService("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(TimeBg));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            folder.WriteLogService("Timed Background Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            folder.WriteLogService("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
