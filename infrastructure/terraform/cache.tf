resource "azurerm_managed_redis" "feat_redis_enterprise" {
  name                      = "${var.prefix}-redis"
  location                  = azurerm_resource_group.feat-rg.location
  resource_group_name       = azurerm_resource_group.feat-rg.name
  sku_name                  = "Balanced_B0"
  high_availability_enabled = false

  default_database {
    access_keys_authentication_enabled = true
  }


}
