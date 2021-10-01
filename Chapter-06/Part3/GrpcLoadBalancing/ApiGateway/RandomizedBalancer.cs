using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ApiGateway
{
    public class RandomizedBalancer : SubchannelsLoadBalancer
    {
        public RandomizedBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory)
           : base(controller, loggerFactory)
        {
        }

        protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
        {
            return new RandomizedPicker(readySubchannels);
        }

        private class RandomizedPicker : SubchannelPicker
        {
            private readonly IReadOnlyList<Subchannel> _subchannels;
            private readonly Random _randomNumberGenerator;

            public RandomizedPicker(IReadOnlyList<Subchannel> subchannels)
            {
                _subchannels = subchannels;
                _randomNumberGenerator = new Random();
            }

            public override PickResult Pick(PickContext context)
            {
                return PickResult.ForSubchannel(_subchannels[_randomNumberGenerator.Next(0, _subchannels.Count)]);
            }
        }
    }

    public class RandomizedBalancerFactory : LoadBalancerFactory
    {
        public override string Name => "randomized";

        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new RandomizedBalancer(options.Controller, options.LoggerFactory);
        }
    }
}
