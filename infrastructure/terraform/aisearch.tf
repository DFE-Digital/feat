resource "azurerm_search_service" "feat_search_service" {
  name                = "${var.prefix}-search"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = var.ai_search_sku
  replica_count       = var.replica_count
  partition_count     = var.partition_count
  semantic_search_sku = var.ai_search_semantic_sku

  tags = {
    Environment = var.env
    Product     = var.product
  }

  lifecycle {
    ignore_changes = [
      # Ignore changes to the 'tags' attribute
      tags,
    ]
    prevent_destroy = true
  }
}