module "login_client" {
  source = "../../terraform-module/"

  environment  = local.environment
  name         = local.name
  key_vault_id = local.key_vault_id
  client_id    = local.name
  redirect_uris = [
    "https://localhost/auth/signin-oidc/",
    "https://localhost/auth/signin-oidc/"
  ]
  allowed_grant_types = ["hybrid"]
  allowed_scopes      = ["openid", "profile"]
}

locals {
  environment  = "playground"
  name         = "ClientNumberTwo"
  key_vault_id = "/subscriptions/075254db-75bd-4d17-b2a8-9fa8cdff9f65/resourceGroups/rg-max/providers/Microsoft.KeyVault/vaults/pascal-test-kv"
}
