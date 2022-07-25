resource "azurerm_kubernetes_cluster" "sre" {
  name                = "${var.base_name}-aks-1"
  location            = var.location
  resource_group_name = var.rg_name
  dns_prefix          = var.base_name

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_B2s"
  }

  identity {
    type = "SystemAssigned"
  }
}
