using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Secrets;
using System.Threading.Tasks;

namespace SecretsManager
{
    [Authorize]
    public class SecretsManagerService : SecretStore.SecretStoreBase
    {
        private readonly SecretsCache secretsCache;
        
        public SecretsManagerService(SecretsCache secretsCache)
        {
            this.secretsCache = secretsCache;
        }

        [Authorize(Roles = "User")]
        public override Task<GetSecretResponse> GetSecret(GetSecretRequest request, ServerCallContext context)
        {
            var secret = secretsCache.GetSecret(request.Id);

            if (secret is not null)
                return Task.FromResult(new GetSecretResponse
                {
                    Data = secret
                });

            return Task.FromResult(new GetSecretResponse
            {
                ErrorMessage = $"No secret found for id {request.Id}."
            });        
        }

        [AllowAnonymous]
        public override Task<SecretsCount> GetSecretsCount(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SecretsCount
            {
                Count = secretsCache.GetCount()
            });
        }

        [Authorize(Roles = "Admin")]
        public override Task<Empty> InsertSecret(SecretData request, ServerCallContext context)
        {
            secretsCache.InsertSecret(request);
            return Task.FromResult(new Empty());
        }
    }
}
