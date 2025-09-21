output "api_url" {
  value = azurerm_linux_web_app.api.default_hostname
}

output "website_url" {
  value = azurerm_linux_web_app.website.default_hostname
}

output "ingestion_container_name" {
  value = azurerm_container_group.ingestion.name
}

// Output the Azure resource group and location details 
output "resource_group_name" {
  value = azurerm_resource_group.this.name
  description = "Name of the resource group"
}

output "resource_group_location" {
  description = "Location of the created resource group"
  value       = azurerm_resource_group.this.location
}

// Postgres related outputs 
output "azurerm_postgresql_flexible_server" {
  value = azurerm_postgresql_flexible_server.feat_psg_server.name
  description = "PostgreSQL Flexible Server name"
}

output "postgresql_flexible_server_database_name" {
  value = azurerm_postgresql_flexible_server_database.feat_pg_ingestion_db.name
  description = "Database name"
}

// Virtual network related outputs
output "azurerm_virtual_network" {
  value = azurerm_virtual_network.dfe_feat_vnet.name
  description = "Virtual network name"
}

output "azurerm_network_security_group" {
  value = azurerm_network_security_group.postgres-nsg.name
  description = "Network security group name"
}

output "azurerm_subnet" {
  value = azurerm_subnet.postgres_subnet.name
  description = "Subnet name"
}

output "azurerm_private_dns_zone" {
  value = azurerm_private_dns_zone.default.name
  description = "Private DNS Zone name"
}

output "azurerm_private_dns_zone_virtual_network_link" {
  value = azurerm_private_dns_zone_virtual_network_link.default.name
  description = "Private DNS Zone Virtual Network Link name"
}

// For dev only - do not use in production
output "postgresql_flexible_server_admin_login" {
  value = azurerm_postgresql_flexible_server.feat_psg_server.administrator_login
}

output "postgresql_flexible_server_admin_password" {
  sensitive = true
  value     = azurerm_postgresql_flexible_server.feat_psg_server.administrator_password
}

