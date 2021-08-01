using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalrApplication.Hubs
{
    public class JobsHub : Hub
    {
        public async Task SendSingleJob(string jobDescription)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", $"Job executed successfully. Description: {jobDescription}");
        }

        public async Task StreamJobs(IAsyncEnumerable<int> stream)
        {
            var jobsCount = 0;
            await foreach (var item in stream)
            {
                Console.WriteLine($"Job {item} executed succesfully");
                jobsCount++;
            }

            await Clients.Caller.SendAsync("ReceiveMessage", $"{jobsCount} jobs executed successfully.");
        }

        public async IAsyncEnumerable<string> TriggerJobs(
        int jobsCount,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
        {
            for (var i = 0; i < jobsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return $"Job {i} executed succesfully";

                await Task.Delay(2000, cancellationToken);
            }
        }
    }
}
