# App Service Plan (Web)
resource "azurerm_service_plan" "feat-web-asp" {
  name                = "${var.prefix}-web-asp"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  os_type             = "Linux"
  sku_name            = "B1"

  depends_on = [azurerm_mssql_database.feat_mssql_db, azurerm_managed_redis.feat_redis_enterprise]
}

# Linux Web App - API
resource "azurerm_linux_web_app" "feat-api" {
  name                = "${var.prefix}-app-api"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  service_plan_id     = azurerm_service_plan.feat-web-asp.id


  site_config {
    application_stack {
      docker_image_name        = var.api_image_name
      docker_registry_url      = "https://${azurerm_container_registry.feat-registry.login_server}"
      docker_registry_username = azurerm_container_registry.feat-registry.admin_username
      docker_registry_password = azurerm_container_registry.feat-registry.admin_password
    }
  }

  app_settings = {
    WEBSITES_ENABLE_APP_SERVICE_STORAGE   = "false"
    Azure__OpenAiEndpoint                 = "https://pp-api.education.gov.uk"
    Azure__AiSearchUrl                    = "https://${azurerm_search_service.feat_search_service.name}.search.windows.net"
    Azure__AiSearchIndex                  = "development"
    Azure__AiSearchIndexScoringProfile    = ""
    Azure__AiSearchIndexScoringParameters = ""
  }

  connection_string {
    name  = "SQL"
    type  = "SQLServer"
    value = "Server=tcp:${azurerm_mssql_server.feat_mssql_server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.feat_mssql_db.name};Persist Security Info=False;User ID=feat-admin;Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

  connection_string {
    name  = "Redis"
    type  = "RedisCache"
    value = "s265d01-redis.uksouth.redis.azure.net:10000"
  }

  https_only = true
  depends_on = [azurerm_service_plan.feat-web-asp]
}

# Linux Web App - Website
resource "azurerm_linux_web_app" "feat-website" {
  name                = "${var.prefix}-app-web"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  service_plan_id     = azurerm_service_plan.feat-web-asp.id

  site_config {
    application_stack {
      docker_image_name        = var.website_image_name
      docker_registry_url      = "https://${azurerm_container_registry.feat-registry.login_server}"
      docker_registry_username = azurerm_container_registry.feat-registry.admin_username
      docker_registry_password = azurerm_container_registry.feat-registry.admin_password
    }
  }

  app_settings = {
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = "false"
  }

  https_only = true
  depends_on = [azurerm_service_plan.feat-web-asp]
}
