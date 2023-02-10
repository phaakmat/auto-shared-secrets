using Azure;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;

namespace Configuration
{
    /// <summary>
    /// Handles keyvault with rbac configured secret access. 
    /// As access is set per secret base this manager reads only secrets for which permissions have been granted within a given keyvault.
    /// </summary>
    public class HandlingRbacKeyVaultSecretManager : KeyVaultSecretManager
    {
        private readonly SecretClient _secretClient;

        public HandlingRbacKeyVaultSecretManager(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public override bool Load(SecretProperties secret)
        {
            try
            {
                _secretClient.GetSecret(secret.Name);
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 403)
            {
                return false;
            }
        }
    }
}