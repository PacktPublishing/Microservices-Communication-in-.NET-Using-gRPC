using Grpc.Core;
using Secrets;
using System;
using System.Threading.Tasks;
using UserFacingApp.Models;

namespace UserFacingApp
{
    public class GrpcClientWrapper
    {
        private readonly SecretStore.SecretStoreClient client;
        public GrpcClientWrapper(SecretStore.SecretStoreClient client)
        {
            this.client = client;
        }

        public async Task<SecretDetails> GetSecret(int id, string accessToken)
        {
            var metadata = new Metadata
            {
                { "Authorization", $"Bearer {accessToken}" }
            };
            var request = new GetSecretRequest
            {
                Id = id
            };

            var response = await client.GetSecretAsync(request, metadata);

            if (string.IsNullOrEmpty(response.ErrorMessage))
                return new SecretDetails
                {
                    Id = response.Data.Id,
                    Title = response.Data.Title,
                    Description = response.Data.Description,
                    SecretLevel = response.Data.Level.ToString()
                };

            throw new Exception(response.ErrorMessage);
        }
    }
}
