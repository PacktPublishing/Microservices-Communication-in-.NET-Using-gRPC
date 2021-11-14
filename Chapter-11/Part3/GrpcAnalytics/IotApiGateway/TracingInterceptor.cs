using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace IotApiGateway
{
    public class TracingInterceptor : Interceptor
    {
        private readonly ILogger<TracingInterceptor> logger;

        public TracingInterceptor(ILogger<TracingInterceptor> logger)
        {
            this.logger = logger;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            try
            {
                return base.BlockingUnaryCall(request, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            try
            {
                return base.AsyncUnaryCall(request, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            try
            {
                return base.AsyncClientStreamingCall(context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            try
            {
                return base.AsyncServerStreamingCall(request, context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            try
            {
                return base.AsyncDuplexStreamingCall(context, continuation);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        private void LogCall<TRequest, TResponse>(Method<TRequest, TResponse> method) where TRequest : class where TResponse : class
        {
            logger.LogDebug($"Call type: {method.Type}. Method name: {method.FullName}. Service name: {method.ServiceName}.");
        }

        private void LogException(RpcException ex)
        {
            logger.LogError(ex, "gRPC error occured");
        }
    }
}
