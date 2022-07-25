output "aks_name" {
  value = azurerm_kubernetes_cluster.sre.name
}

output "aks_api_url" {
  value = azurerm_kubernetes_cluster.sre.fqdn
}
