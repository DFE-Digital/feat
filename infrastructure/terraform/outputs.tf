output "api_url" {
  value = azurerm_linux_web_app.api.default_hostname
}

output "website_url" {
  value = azurerm_linux_web_app.website.default_hostname
}


// Output the Azure resource group and location details 
output "resource_group_name" {
  value = azurerm_resource_group.feat-rg.name
  description = "Name of the resource group"
}

output "resource_group_location" {
  description = "Location of the created resource group"
  value       = azurerm_resource_group.feat-rg.location
}


// Virtual network related outputs
output "azurerm_virtual_network" {
  value = azurerm_virtual_network.dfe_feat_vnet.name
  description = "Virtual network name"
}

output "azurerm_network_security_group" {
  value = azurerm_network_security_group.feat-nsg.name
  description = "Network security group name"
}

output "azurerm_subnet" {
  value = azurerm_subnet.feat_main_subnet.name
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


# MsSql related outputs
output "mssql_server_name" {
  value       = azurerm_mssql_server.feat_mssql_server.name
  description = "The name of the SQL Server."
}
output "mssql_database_name" {
  value       = azurerm_mssql_database.mssql_db.name
  description = "The name of the SQL Database."
}
output "mssql_server_fqdn" {
  value       = azurerm_mssql_server.feat_mssql_server.fully_qualified_domain_name
  description = "The fully qualified domain name of the SQL Server."
}


# Azure Search Service related outputs
output "search_service_name" {
  value       = azurerm_search_service.feat_search_service.name
  description = "The name of the Azure Search Service."
}
