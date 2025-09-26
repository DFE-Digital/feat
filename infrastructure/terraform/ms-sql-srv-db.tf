
resource "azurerm_mssql_server" "feat_mssql_server" {
  name                         = "${var.prefix}-mssql-srv"
  resource_group_name          = azurerm_resource_group.feat-rg.name
  location                     = azurerm_resource_group.feat-rg.location
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  version                      = "12.0"
  
  tags = {
    environments = var.env 
  }

  lifecycle {
    prevent_destroy = false # set to true for none-dev environments,
  }
}

resource "azurerm_mssql_database" "mssql_db" {
  name      = "${var.prefix}-ingestion-mssql-db"
  server_id = azurerm_mssql_server.feat_mssql_server.id
  collation = "SQL_Latin1_General_CP1_CI_AS"
  license_type = "LicenseIncluded"
  sku_name  = "S0"
  
  auto_pause_delay_in_minutes  = 120 # Auto-pauses after 120 minutes of inactivity
  max_size_gb                  = 2   # Max size of 2 GB
  min_capacity                 = 0.5 # Minimum capacity of 0.5 vCores

  tags = {
    environments = var.env 
  }

  lifecycle {
    prevent_destroy = false # set to true for none-dev environments,
  }
}
