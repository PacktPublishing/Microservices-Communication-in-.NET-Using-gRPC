using System;
using System.Threading.Tasks;
using DataProcessor;
using Grpc.Core;

namespace GrpcServerCommon
{
    public class IngestorService : Ingestor.IngestorBase
    {
        public override Task<DataResponse> ProcessData(DataRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Object id: {request.Id}");
            Console.WriteLine($"Object name: {request.Name}");
            Console.WriteLine($"Object description: {request.Description}");

            return Task.FromResult(new DataResponse
            {
                Success = true
            });
        }
    }
}
