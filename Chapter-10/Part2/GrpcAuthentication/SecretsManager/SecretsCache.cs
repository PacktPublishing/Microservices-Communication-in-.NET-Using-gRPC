using Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretsManager
{
    public class SecretsCache
    {
        private readonly List<SecretData> secrets;

        public SecretsCache()
        {
            secrets = new List<SecretData>();
            secrets.Add(new SecretData
            {
                Id = 1,
                Title = "Undercover Operative",
                Description = "We have an undercover operative in Northern Alaska",
                Level = SecretLevel.Restricted
            });
            secrets.Add(new SecretData
            {
                Id = 2,
                Title = "Ship Position",
                Description = "The current ship's coordinates are 54.55, 4.9",
                Level = SecretLevel.Secret
            });
            secrets.Add(new SecretData
            {
                Id = 3,
                Title = "Bioweapon",
                Description = "A bioweapon has been in development since 2009",
                Level = SecretLevel.TopSecret
            });
        }

        public SecretData GetSecret(int id)
        {
            return secrets.FirstOrDefault(s => s.Id == id);
        }
    }
}
