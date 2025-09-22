
resource "azurerm_virtual_network" "dfe_feat_vnet" {
  name                = "${var.prefix}-vnet"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_network_security_group" "postgres-nsg" {
  name                = "${var.prefix}-postgres-nsg"
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

resource "azurerm_subnet" "postgres_subnet" {
  name                 = "${var.prefix}-postgres-subnet"
  virtual_network_name = azurerm_virtual_network.dfe_feat_vnet.name
  resource_group_name  = azurerm_resource_group.feat-rg.name
  address_prefixes     = ["10.0.2.0/24"]

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
  subnet_id                 = azurerm_subnet.postgres_subnet.id
  network_security_group_id = azurerm_network_security_group.postgres-nsg.id
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
}