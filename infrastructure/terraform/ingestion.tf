# Azure Container App Environment
resource "azurerm_container_app_environment" "feat-ingestion-environment" {
  name                  = "${var.prefix}-cg-ingestion"
  resource_group_name   = azurerm_resource_group.feat-rg.name
  location              = azurerm_resource_group.feat-rg.location
  public_network_access = "Disabled"

  # VNet integration
  infrastructure_subnet_id = azapi_resource.feat_ingestion_subnet.id  

  # Logs
  logs_destination           = "log-analytics"
  log_analytics_workspace_id = azurerm_log_analytics_workspace.feat_logging.id

  tags = {
    Environment = var.env
    Product     = var.product
  }

  depends_on = [azapi_resource.feat_ingestion_subnet]
  
  lifecycle {
    ignore_changes = [
      # Ignore changes to the 'tags' attribute
      tags,
    ]
  }
}