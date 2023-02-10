variable "environment" {
  type        = string
  description = "The environment of the application, e.g. 'playground' or 'dev', used as namespace name suffix."

  validation {
    condition     = contains(["transit", "playground", "tools", "dev", "acc", "prod"], var.environment)
    error_message = "Environment should be transit, playground, tools, dev, acc or prod."
  }
}

variable "key_vault_id" {
  type = string
}

variable "client_id" {
  type = string
}

variable "name" {
  type = string
}

variable "allowed_grant_types" {
  type = list(string)
}

variable "allowed_scopes" {
  type = list(string)
}

variable "redirect_uris" {
  type = list(string)
}

