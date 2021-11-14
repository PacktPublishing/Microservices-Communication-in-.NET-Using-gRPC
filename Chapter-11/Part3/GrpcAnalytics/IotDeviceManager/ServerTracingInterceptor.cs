using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IotDeviceManager
{
    public class ServerTracingInterceptor : Interceptor
    {
        private readonly ILogger<ServerTracingInterceptor> logger;

        public ServerTracingInterceptor(ILogger<ServerTracingInterceptor> logger)
        {
            this.logger = logger;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogCall(context);

            try
            {
                return base.UnaryServerHandler(request, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall(context);

            try
            {
                return base.ClientStreamingServerHandler(requestStream, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall(context);

            try
            {
                return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            LogCall(context);

            try
            {
                return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        private void LogCall(ServerCallContext context)
        {
            logger.LogDebug($"gRPC call request: {context.GetHttpContext().Request.Path}");
        }

        private void LogException(RpcException ex)
        {
            logger.LogError(ex, "gRPC error occured");
        }
    }
}
