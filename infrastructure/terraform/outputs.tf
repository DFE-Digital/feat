output "api_url" {
  value = azurerm_linux_web_app.api.default_hostname
}

output "website_url" {
  value = azurerm_linux_web_app.website.default_hostname
}

output "ingestion_container_name" {
  value = azurerm_container_group.ingestion.name
}

output "acr_login_server" {
  value = azurerm_container_registry.this.login_server
}
