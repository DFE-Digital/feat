resource "azurerm_mssql_server" "feat_mssql_server" {
  name                         = "${var.prefix}-sqlsrv"
  resource_group_name          = azurerm_resource_group.feat-rg.name
  location                     = azurerm_resource_group.feat-rg.location
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  version                      = "12.0"

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

resource "azurerm_mssql_database" "feat_mssql_db" {
  name      = "${var.prefix}-coursedb"
  server_id = azurerm_mssql_server.feat_mssql_server.id
  sku_name  = "GP_S_Gen5_1"

  auto_pause_delay_in_minutes = 120
  max_size_gb                 = 8
  min_capacity                = 0.5

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

  depends_on = [azurerm_mssql_server.feat_mssql_server]
}

