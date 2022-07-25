terraform {
  backend "azurerm" {
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.15.0"
    }
  }

  required_version = ">= 1.2.0"
}

provider "azurerm" {
  features {}
}

locals {
  base_name = "sre-assignment"
}

resource "azurerm_resource_group" "sre" {
  name     = "${local.base_name}-rg"
  location = "West Europe"
  tags     = {
    Project = "e-conomic sre assignment"
  }
}

module "acr" {
  source    = "./modules/acr"
  base_name = local.base_name
  location  = azurerm_resource_group.sre.location
  rg_name   = azurerm_resource_group.sre.name
}

module "aks" {
  source    = "./modules/aks"
  base_name = local.base_name
  location  = azurerm_resource_group.sre.location
  rg_name   = azurerm_resource_group.sre.name
  acr_id    = module.acr.acr_id
}

module "ado" {
  source      = "./modules/ado"
  acr_name    = module.acr.acr_name
  acr_rg_name = azurerm_resource_group.sre.name
  aks_api_url = module.aks.aks_api_url
  aks_name    = module.aks.aks_name
  aks_rg_name = azurerm_resource_group.sre.name
}

