using System;
using System.Threading.Tasks;
using Grpc.Core;
using Worker;

namespace StatusMicroservice
{
    public class JobManagerService : JobManager.JobManagerBase
    {
        public override async Task TriggerJobs(TriggerJobsRequest request, IServerStreamWriter<TriggerJobsResponse> responseStream, ServerCallContext context)
        {
            for (var i = 0; i < request.JobsCount; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));

                await responseStream.WriteAsync(new TriggerJobsResponse
                {
                    JobSequence = i + 1,
                    JobMessage = "Job executed successfully"
                });
            }
        }

        public override async Task<SendJobsResponse> SendJobs(IAsyncStreamReader<SendJobsRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Job Id: {requestStream.Current.JobId}. Job description: {requestStream.Current.JobDescription}");
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            return new SendJobsResponse
            {
                Completed = true
            };
        }
    }
}
