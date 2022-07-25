terraform {
  required_providers {
    azuredevops = {
      source  = "microsoft/azuredevops"
      version = ">=0.1.0"
    }
  }
}

provider "azuredevops" {
  org_service_url = "https://dev.azure.com/pikatpavel/"
}

data "azuredevops_project" "economic" {
  name = "e-conomic"
}

data "azurerm_subscription" "current" {
}

resource "azuredevops_serviceendpoint_azurecr" "sre" {
  project_id                = data.azuredevops_project.economic.id
  service_endpoint_name     = "e-conomic AzureCR"
  resource_group            = var.acr_rg_name
  azurecr_spn_tenantid      = data.azurerm_subscription.current.tenant_id
  azurecr_name              = var.acr_name
  azurecr_subscription_id   = data.azurerm_subscription.current.subscription_id
  azurecr_subscription_name = data.azurerm_subscription.current.display_name
}
