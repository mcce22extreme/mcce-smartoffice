terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
    }
    azapi = {
      source = "Azure/azapi"
    }
  }
  backend "azurerm" {
    key                  = "smartoffice.tfstate"
    resource_group_name  = "rg-smartoffice-common"
    storage_account_name = "smartofficetfstate"
    container_name       = "tfstate"
  }
}

provider "azapi" {
}

provider "azurerm" {
  features {
    app_configuration {
      purge_soft_delete_on_destroy = true
      recover_soft_deleted         = false
    }
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = false
    }
  }
}

data "azurerm_client_config" "current" {}

resource "random_string" "name-prefix" {
  length  = 4
  upper   = false
  lower   = true
  numeric = true
  special = false
}
