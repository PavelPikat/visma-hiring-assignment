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

resource "azurerm_role_assignment" "acr_role" {
  scope                = var.acr_id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_kubernetes_cluster.sre.kubelet_identity[0].object_id
}
