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
    }
}
