# Workaround for a bug where azurerm_container_app_environment doesn't register the Microsoft.App provider
resource "azurerm_resource_provider_registration" "app" {
  name = "Microsoft.App"
}

# Azure Container App Environment
resource "azurerm_container_app_environment" "feat-ingestion-environment" {
  name                  = "${var.prefix}-cg-ingestion"
  resource_group_name   = azurerm_resource_group.feat-rg.name
  location              = azurerm_resource_group.feat-rg.location
  public_network_access = "Disabled"

  # VNet integration
  infrastructure_subnet_id = azapi_resource.feat_main_subnet.id

  # Logs
  logs_destination           = "log-analytics"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.feat_logging.id

  workload_profile {
    name                  = "${var.prefix}-wp-ingestion"
    workload_profile_type = "Consumption"
    minimum_count         = 0
    maximum_count         = 1
  }

  tags = {
    Environment = var.env
    Product     = var.product
  }

  depends_on = [
    azurerm_resource_provider_registration.app
  ]
}