using Grpc.Core;
using Grpc.Core.Interceptors;
using System;

namespace IotApiGateway
{
    public class TracingInterceptor : Interceptor
    {
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return base.BlockingUnaryCall(request, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return base.AsyncUnaryCall(request, context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return base.AsyncClientStreamingCall(context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return base.AsyncServerStreamingCall(request, context, continuation);
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
                return base.AsyncDuplexStreamingCall(context, continuation);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
