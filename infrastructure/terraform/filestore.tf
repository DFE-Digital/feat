resource "azurerm_storage_account" "feat_storage_account" {
  name                          = "${var.prefix}storage"
  resource_group_name           = azurerm_resource_group.feat-rg.name
  location                      = azurerm_resource_group.feat-rg.location
  account_tier                  = "Standard"
  account_replication_type      = "LRS"
  public_network_access_enabled = true

  network_rules {
    default_action             = "Deny"
    virtual_network_subnet_ids = [azapi_resource.feat_main_subnet.id]
  }

  tags = {
    Environment = var.env
    Product     = var.product
  }
}