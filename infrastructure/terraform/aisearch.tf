resource "azurerm_search_service" "feat_search_service" {
  name                = "${var.prefix}-search"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = var.sku
  replica_count       = var.replica_count
  partition_count     = var.partition_count


}