﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Performance;

namespace ApiGateway
{
    public interface IGrpcPerformanceClient
    {
        Task<ResponseModel.PerformanceStatusModel> GetPerformanceStatus(string clientName);
        Task<IEnumerable<ResponseModel.PerformanceStatusModel>> GetPerformanceStatuses(IEnumerable<string> clientNames);
    }

    internal class GrpcPerformanceClient : IGrpcPerformanceClient, IDisposable
    {
        private readonly GrpcChannel channel;

        public GrpcPerformanceClient(string serverUrl)
        {
            channel = GrpcChannel.ForAddress(serverUrl);
        }

        public async Task<ResponseModel.PerformanceStatusModel> GetPerformanceStatus(string clientName)
        {
            var client = new Monitor.MonitorClient(channel);

            var response = await client.GetPerformanceAsync(new PerformanceStatusRequest
            {
                ClientName = clientName
            });

            return new ResponseModel.PerformanceStatusModel
            {
                CpuPercentageUsage = response.CpuPercentageUsage,
                MemoryUsage = response.MemoryUsage,
                ProcessesRunning = response.ProcessesRunning,
                ActiveConnections = response.ActiveConnections
            };
        }

        public void Dispose()
        {
            channel.Dispose();
        }

        public async Task<IEnumerable<ResponseModel.PerformanceStatusModel>> GetPerformanceStatuses(IEnumerable<string> clientNames)
        {
            
            var client = new Monitor.MonitorClient(channel);
            using var call = client.GetManyPerformanceStats();

            var responses = new List<ResponseModel.PerformanceStatusModel>();

            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    responses.Add(new ResponseModel.PerformanceStatusModel
                    {
                        CpuPercentageUsage = response.CpuPercentageUsage,
                        MemoryUsage = response.MemoryUsage,
                        ProcessesRunning = response.ProcessesRunning,
                        ActiveConnections = response.ActiveConnections
                    });
                }
            });

            foreach (var clientName in clientNames)
            {
                await call.RequestStream.WriteAsync(new PerformanceStatusRequest
                {
                    ClientName = clientName
                });
            }

            await call.RequestStream.CompleteAsync();
            await readTask;

            return responses;
        }
    }
}
