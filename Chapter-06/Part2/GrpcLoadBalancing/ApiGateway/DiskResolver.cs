using Grpc.Net.Client.Balancer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class DiskResolver : Resolver
    {
        private readonly Uri _address;
        private Action<ResolverResult> _listener;

        public DiskResolver(Uri address)
        {
            _address = address;
        }

        public override Task RefreshAsync(CancellationToken cancellationToken)
        {
            var addresses = new List<DnsEndPoint>();

            foreach (var line in File.ReadLines(_address.LocalPath))
            {
                var addresComponents = line.Split(' ');
                addresses.Add(new DnsEndPoint(addresComponents[0], int.Parse(addresComponents[1])));
            }

            _listener(ResolverResult.ForResult(addresses, serviceConfig: null));

            return Task.CompletedTask;
        }

        public override void Start(Action<ResolverResult> listener)
        {
            _listener = listener;
        }      
    }

    public class DiskResolverFactory : ResolverFactory
    {
        public override string Name => "disk";

        public override Resolver Create(ResolverOptions options)
        {
            return new DiskResolver(options.Address);
        }
    }
}
