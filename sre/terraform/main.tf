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
