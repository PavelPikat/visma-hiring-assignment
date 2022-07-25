output "acr_admin_password" {
  value     = module.acr.acr_admin_password
  sensitive = true
}

output "acr_name" {
  value = module.acr.acr_name
}

output "ado_acr_service_connection_name" {
  value = module.ado.ado_acr_service_connection_name
}
