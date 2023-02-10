resource "azurerm_key_vault_secret" "config" {
  name = "Clients--${var.name}--Config"
  value = jsonencode({
    "ClientId"          = var.client_id
    "AllowedScopes"     = var.allowed_scopes
    "AllowedGrantTyeps" = var.allowed_grant_types
    "RedirectUris"      = var.redirect_uris
  })
  key_vault_id = var.key_vault_id
}

resource "azurerm_key_vault_certificate" "secret" {
  name         = "Clients--${var.name}--Secret"
  key_vault_id = var.key_vault_id

  certificate_policy {
    issuer_parameters {
      name = "Self"
    }

    key_properties {
      exportable = true
      key_size   = 2048
      key_type   = "RSA"
      reuse_key  = true
    }

    lifetime_action {
      action {
        action_type = "AutoRenew"
      }

      trigger {
        days_before_expiry = 2
      }
    }

    secret_properties {
      content_type = "application/x-pkcs12"
    }

    x509_certificate_properties {
      key_usage = [
        "cRLSign",
        "dataEncipherment",
        "digitalSignature",
        "keyAgreement",
        "keyCertSign",
        "keyEncipherment",
      ]

      subject            = "CN=test-signing"
      validity_in_months = 1
    }
  }
}
