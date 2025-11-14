resource "azurerm_virtual_network" "feat_vnet" {
  name                = "${var.prefix}-vnet"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  address_space       = ["10.0.0.0/16"]


}

resource "azurerm_network_security_group" "feat-nsg" {
  name                = "${var.prefix}-nsg"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name

  security_rule {
    name                       = "Allow-Web-From-VNet"
    priority                   = 200
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_address_prefix      = "*"
    destination_address_prefix = "VirtualNetwork"
    destination_port_ranges    = ["80", "443"]
    source_port_range          = "*"
  }

  #potentially, for internal communication within the VNet:
  security_rule {
    name                       = "Allow-Internal-VNet"
    priority                   = 210
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "*"
    source_address_prefix      = "VirtualNetwork"
    destination_address_prefix = "VirtualNetwork"
    source_port_range          = "*"
    destination_port_range     = "*"
  }


}

resource "azapi_resource" "feat_main_subnet" {
  type      = "Microsoft.Network/virtualNetworks/subnets@2024-05-01"
  name      = "${var.prefix}-subnet"
  parent_id = azurerm_virtual_network.feat_vnet.id

  body = {
    properties = {
      addressPrefixes = ["10.0.1.0/24"]
      delegations = [
        {
          name = "asp-delegation"
          properties = {
            serviceName = "Microsoft.Web/serverFarms"
          }
        }
      ]
      serviceEndpoints = [
        {
          service   = "Microsoft.Sql"
          locations = [azurerm_resource_group.feat-rg.location]
        },
        {
          service   = "Microsoft.Storage"
          locations = [azurerm_resource_group.feat-rg.location]
        }
      ]
      # the association with the network security group
      networkSecurityGroup = {
        id = azurerm_network_security_group.feat-nsg.id
      }
    }
  }

  depends_on = [azurerm_network_security_group.feat-nsg]


}

resource "azapi_resource" "feat_ingestion_subnet" {
  type      = "Microsoft.Network/virtualNetworks/subnets@2024-05-01"
  name      = "${var.prefix}-ingestion-subnet"
  parent_id = azurerm_virtual_network.feat_vnet.id

  body = {
    properties = {
      addressPrefixes = ["10.0.2.0/23"]
      delegations = [
        {
          name = "ing-delegation"
          properties = {
            serviceName = "Microsoft.App/environments"
          }

        }
      ]

      # the association with the network security group
      networkSecurityGroup = {
        id = azurerm_network_security_group.feat-nsg.id
      }
    }
  }

  depends_on = [azurerm_network_security_group.feat-nsg]


}

resource "azurerm_private_dns_zone" "default" {
  name                = "${var.prefix}-pdz.database.windows.net"
  resource_group_name = azurerm_resource_group.feat-rg.name

  depends_on = [azapi_resource.feat_main_subnet, azapi_resource.feat_ingestion_subnet]


}

resource "azurerm_private_dns_zone_virtual_network_link" "default" {
  name                  = "${var.prefix}-pdz-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.default.name
  virtual_network_id    = azurerm_virtual_network.feat_vnet.id
  resource_group_name   = azurerm_resource_group.feat-rg.name

  depends_on = [azapi_resource.feat_main_subnet, azapi_resource.feat_ingestion_subnet]


}

#  VNet SQL Firewall Rule
resource "azurerm_mssql_virtual_network_rule" "mssql_vnet_rule" {
  name      = "${var.prefix}-mssql-vnet-rule"
  server_id = azurerm_mssql_server.feat_mssql_server.id
  subnet_id = azapi_resource.feat_main_subnet.id
}
resource "azurerm_mssql_virtual_network_rule" "mssql_ing_vnet_rule" {
  name      = "${var.prefix}-mssql-ing-vnet-rule"
  server_id = azurerm_mssql_server.feat_mssql_server.id
  subnet_id = azapi_resource.feat_ingestion_subnet.id
}

# API VNet Integration
resource "azurerm_app_service_virtual_network_swift_connection" "api_app_vn_conn" {
  app_service_id = azurerm_linux_web_app.feat-api.id
  subnet_id      = azapi_resource.feat_main_subnet.id

  depends_on = [azurerm_linux_web_app.feat-api, azapi_resource.feat_main_subnet]


}

# Website VNet Integration
resource "azurerm_app_service_virtual_network_swift_connection" "website_app_vn_conn" {
  app_service_id = azurerm_linux_web_app.feat-website.id
  subnet_id      = azapi_resource.feat_main_subnet.id

  depends_on = [azurerm_linux_web_app.feat-website, azapi_resource.feat_main_subnet]


}