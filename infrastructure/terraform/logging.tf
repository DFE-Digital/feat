resource "azurerm_log_analytics_workspace" "feat_logging" {
  name                = "${var.prefix}logging"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = {
    Environment = var.env
    Product     = var.product
  }

  lifecycle {
    ignore_changes = [
      # Ignore changes to the 'tags' attribute
      tags,
    ]
  }
}
