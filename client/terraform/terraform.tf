terraform {
  backend "azurerm" {
    resource_group_name  = "rg-tfstate-01"
    container_name       = "tfstate"
    key                  = "tfstate.auto-shared-secrets"
    storage_account_name = "fundatfstateplayground"
  }
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
    }
  }
}

provider "azurerm" {
  features {}
}
