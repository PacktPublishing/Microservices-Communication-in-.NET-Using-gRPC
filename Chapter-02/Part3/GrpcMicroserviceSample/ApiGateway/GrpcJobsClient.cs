using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Worker;

namespace ApiGateway
{
    public interface IGrpcJobsClient
    {
        Task SendJobs(IEnumerable<JobModel> jobs);
        Task TriggerJobs(int jobCount);
    }

    internal class GrpcJobsClient : IGrpcJobsClient, IDisposable
    {
        private readonly GrpcChannel channel;
        private readonly JobManager.JobManagerClient client;

        public GrpcJobsClient(string serverUrl)
        {
            channel = GrpcChannel.ForAddress(serverUrl);
            client = new JobManager.JobManagerClient(channel);
        }

        public async Task SendJobs(IEnumerable<JobModel> jobs)
        {
            using var call = client.SendJobs();

            foreach (var job in jobs)
            {
                await call.RequestStream.WriteAsync(new SendJobsRequest
                {
                    JobId = job.JobId,
                    JobDescription = job.JobDescription
                });
            }
            await call.RequestStream.CompleteAsync();

            await call;
        }

        public async Task TriggerJobs(int jobCount)
        {
            using var call = client.TriggerJobs(new TriggerJobsRequest { JobsCount = jobCount });

            while (await call.ResponseStream.MoveNext())
            {
                Console.WriteLine($"Job sequence: {call.ResponseStream.Current.JobSequence}. Job description: {call.ResponseStream.Current.JobMessage}");
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
