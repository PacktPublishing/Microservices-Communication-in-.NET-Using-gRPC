using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Prometheus;
using System.Threading.Tasks;

namespace IotApiGateway
{
    public class TracingInterceptor : Interceptor
    {
        private readonly ILogger<TracingInterceptor> logger;

        private static readonly Counter BlockingUnaryCallsCount = Metrics
            .CreateCounter("blocking_unary_calls_count", "Count of blocking unary calls.");
        private static readonly Counter AsyncUnaryCallsCount = Metrics
            .CreateCounter("async_unary_calls_count", "Count of async unary calls.");
        private static readonly Counter ClientStreamingCallsCount = Metrics
            .CreateCounter("client_streaming_calls_count", "Count of client streaming calls.");
        private static readonly Counter ServerStreamingCallsCount = Metrics
            .CreateCounter("server_streaming_calls_count", "Count of server streaming calls.");
        private static readonly Counter DuplexStreamingCallsCount = Metrics
            .CreateCounter("duplex_streaming_calls_count", "Count of bi-directional streaming calls.");
        private static readonly Counter FailedGrpcCallsCount = Metrics
            .CreateCounter("failed_grpc_calls_count", "Count of failed gRPC calls.");
        private static readonly Histogram GrpcCallDuration = Metrics
            .CreateHistogram("grpc_call_duration", "Durations of gRPC calls.");

        public TracingInterceptor(ILogger<TracingInterceptor> logger)
        {
            this.logger = logger;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);
            BlockingUnaryCallsCount.Inc();

            try
            {
                using (GrpcCallDuration.NewTimer())
                    return continuation(request, context);
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
            AsyncUnaryCallsCount.Inc();
            var call = continuation(request, context);
            using (GrpcCallDuration.NewTimer())
                return new AsyncUnaryCall<TResponse>(HandleCallResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);
            ClientStreamingCallsCount.Inc();
            var call = continuation(context);

            using (GrpcCallDuration.NewTimer())
                return new AsyncClientStreamingCall<TRequest, TResponse>(
                    call.RequestStream,
                    HandleCallResponse(call.ResponseAsync),
                    call.ResponseHeadersAsync,
                    call.GetStatus,
                    call.GetTrailers,
                    call.Dispose);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);
            ServerStreamingCallsCount.Inc();

            try
            {
                using (GrpcCallDuration.NewTimer())
                    return continuation(request, context);
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
            DuplexStreamingCallsCount.Inc();

            try
            {
                using (GrpcCallDuration.NewTimer())
                    return continuation(context);
            }
            catch (RpcException ex)
            {
                LogException(ex);
                throw;
            }
        }

        private async Task<TResponse> HandleCallResponse<TResponse>(Task<TResponse> responseTask)
        {
            try
            {
                var response = await responseTask;
                return response;
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
            logger.LogError(ex, "gRPC error occurred");
            FailedGrpcCallsCount.Inc();
        }
    }
}
