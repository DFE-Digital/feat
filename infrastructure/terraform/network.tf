
resource "azurerm_virtual_network" "dfe_feat_vnet" {
  name                = "${var.prefix}-vnet"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_network_security_group" "feat-nsg" {
  name                = "${var.prefix}-feat-nsg"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name

  security_rule {
    name                       = "defaultSecurityrule"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*" # All source ports
    destination_port_range     = "*" # "5432" 
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

resource "azurerm_subnet" "feat_main_subnet" {
  name                 = "${var.prefix}-feat-main-subnet"
  virtual_network_name = azurerm_virtual_network.dfe_feat_vnet.name
  resource_group_name  = azurerm_resource_group.feat-rg.name
  address_prefixes     = ["10.0.1.0/24"]

  service_endpoints    = ["Microsoft.Sql", "Microsoft.Storage"]

  delegation {
    name = "fs"
    service_delegation {
      name = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/join/action",
      ]
    }
  }
}

resource "azurerm_subnet_network_security_group_association" "default" {
  subnet_id                 = azurerm_subnet.feat_main_subnet.id
  network_security_group_id = azurerm_network_security_group.feat-nsg.id
}

resource "azurerm_private_dns_zone" "default" {
  name                = "${var.prefix}-pdz.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.feat-rg.name

  depends_on = [azurerm_subnet_network_security_group_association.default]
}

resource "azurerm_private_dns_zone_virtual_network_link" "default" {
  name                  = "${var.prefix}-pdzvnetlink.com"
  private_dns_zone_name = azurerm_private_dns_zone.default.name
  virtual_network_id    = azurerm_virtual_network.dfe_feat_vnet.id
  resource_group_name   = azurerm_resource_group.feat-rg.name

  depends_on = [azurerm_subnet.feat_main_subnet]
}
# --------
# Create SQL Firewall Rule for VNet
resource "azurerm_mssql_virtual_network_rule" "mssql_vnet_rule" {
  name      = "${var.prefix}-mssql-vnet-rule"
  server_id = azurerm_mssql_server.feat_mssql_server.id
  subnet_id = azurerm_subnet.feat_main_subnet.id
}

# Create VNet Integration for API app
resource "azurerm_app_service_virtual_network_swift_connection" "api_app_vn_conn" {
  app_service_id = azurerm_linux_web_app.api.id
  subnet_id      = azurerm_subnet.feat_main_subnet.id
}

# Create VNet Integration for website app
resource "azurerm_app_service_virtual_network_swift_connection" "website_app_vn_conn" {
  app_service_id = azurerm_linux_web_app.website.id
  subnet_id      = azurerm_subnet.feat_main_subnet.id
}

# End of network.tf

