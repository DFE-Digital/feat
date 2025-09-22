
resource "azurerm_postgresql_flexible_server" "feat_psg_server" {
  name                = "${var.prefix}-pg-flx-srv"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  version = "17"

  delegated_subnet_id = azurerm_subnet.postgres_subnet.id
  private_dns_zone_id = azurerm_private_dns_zone.default.id

  administrator_login    = var.postgresql_admin_login
  administrator_password = var.postgresql_admin_password

  # PostgreSQL flexible server configuration; currently using the most basic sku db settings 
  # see https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/postgresql_flexible_server for more details
  zone                  = "1"
  storage_mb            = 32768
  sku_name = var.postgresql_flexible_server_sku_name # flexible-server sku name, e.g. "Standard_B1ms"
  backup_retention_days = 7

  public_network_access_enabled = false

  depends_on = [azurerm_private_dns_zone_virtual_network_link.default]

  # prevent the possibility of accidental data loss
  lifecycle {
    prevent_destroy = false # set to true for none-dev environments,
  }
  
  tags = {
    environments = var.env 
  }
}

resource "azurerm_postgresql_flexible_server_database" "feat_pg_ingestion_db" {
  name      = "${var.prefix}-pg-ingestion-db"
  server_id = azurerm_postgresql_flexible_server.feat_psg_server.id
  collation = "en_US.utf8"
  charset   = "UTF8"

  # prevent the possibility of accidental data loss
  # terraform won't delete the DB if set to true.
  lifecycle {
    prevent_destroy = false # set to true for none-dev environments,
  }
}

# Select PostGIS extension
resource "azurerm_postgresql_flexible_server_configuration" "azure_extensions" {
  name      = "azure.extensions"
  server_id = azurerm_postgresql_flexible_server.feat_psg_server.id
  value     = "postgis"
}

# Enable PostGIS drivers
resource "azurerm_postgresql_flexible_server_configuration" "postgis_gdal_drivers" {
  name      = "postgis.gdal_enabled_drivers"
  server_id = azurerm_postgresql_flexible_server.feat_psg_server.id
  value     = "ENABLE_ALL"
}
