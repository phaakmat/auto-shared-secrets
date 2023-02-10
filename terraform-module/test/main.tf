module "login_client" {
  source = "../"

  environment  = local.environment
  name         = local.name
  key_vault_id = local.key_vault_id
  client_id    = local.name
  redirect_uris = [
    "http://localhost"
  ]
  allowed_grant_types = ["hybrid"]
  allowed_scopes      = ["openid", "profile"]
}

locals {
  environment  = "playground"
  name         = "GeneratedKeyTest"
  key_vault_id = "/subscriptions/075254db-75bd-4d17-b2a8-9fa8cdff9f65/resourceGroups/rg-max/providers/Microsoft.KeyVault/vaults/pascal-test-kv"
}
