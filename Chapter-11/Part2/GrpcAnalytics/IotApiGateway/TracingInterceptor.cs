using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;

namespace IotApiGateway
{
    public class TracingInterceptor : Interceptor
    {
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return continuation(request, context);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var call = continuation(request, context);
            return new AsyncUnaryCall<TResponse>(HandleCallResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var call = continuation(context);
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
            try
            {
                return continuation(request, context);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return continuation(context);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
