using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace HRScripting.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
    public class QueuedHostedService : BackgroundService
    {
        private ToolFolder folder;
        //private HRSCache cache;
        public QueuedHostedService(IBackgroundTaskQueue taskQueue)//, HRSCache _cache)
        {
            TaskQueue = taskQueue;
            folder = new ToolFolder();
            //cache = _cache;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected async override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            folder.WriteLogService("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    folder.WriteLogService($"Error occurred executing {nameof(workItem)}. Err: {ex.ToString()}.");
                }
            }

            folder.WriteLogService("Queued Hosted Service is stopping.");
        }
    }
}
