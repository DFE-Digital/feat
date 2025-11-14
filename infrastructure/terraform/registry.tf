# Azure Container Registry
resource "azurerm_container_registry" "feat-registry" {
  name                = "${var.prefix}acr"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = "Basic"
  admin_enabled       = true
}
