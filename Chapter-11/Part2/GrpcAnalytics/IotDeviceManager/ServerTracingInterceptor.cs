using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace IotDeviceManager
{
    public class ServerTracingInterceptor : Interceptor
    {
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return base.UnaryServerHandler(request, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return base.ClientStreamingServerHandler(requestStream, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
