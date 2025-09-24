terraform {
  required_version = ">= 1.13.3"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.45" # was "~>4.0" tried explicitly "4.45.1" latest published 11hr ago.
    }
  }
}

provider "azurerm" {
  subscription_id = var.subscription_id
  features {}
}

# Resource Group
resource "azurerm_resource_group" "feat-rg" {
  name     = "${var.prefix}-uks-rg"
  location = var.location
}

# App Service Plan
resource "azurerm_service_plan" "feat-asp" {
  name                = "${var.prefix}-uks-archylife-asp"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  os_type             = "Linux"
  sku_name            = "B1"
}

# Azure Container Registry
resource "azurerm_container_registry" "feat-registry" {
  name                = "dfefeat${var.env}uksacr"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

# Linux Web App - API
resource "azurerm_linux_web_app" "api" {
  name                = "${var.prefix}-uks-app-api"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  service_plan_id     = azurerm_service_plan.feat-asp.id
  
  
  site_config {}

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_CUSTOM_IMAGE_NAME"            = var.api_image
    "DOCKER_REGISTRY_SERVER_URL"          = azurerm_container_registry.feat-registry.login_server
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.feat-registry.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.feat-registry.admin_password 
    # - "DATABASE_CONNECTION_STRING" = "Server=tcp:${azurerm_mssql_server.feat_mssql_server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.feat_mssql_database.name};Persist Security Info=False;User ID=${var.sql_admin_username};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

# Linux Web App - Website
resource "azurerm_linux_web_app" "website" {
  name                = "${var.prefix}-uks-app-web"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  service_plan_id     = azurerm_service_plan.feat-asp.id  
  
  site_config {}

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_CUSTOM_IMAGE_NAME"            = var.website_image
    "DOCKER_REGISTRY_SERVER_URL"          = azurerm_container_registry.feat-registry.login_server
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.feat-registry.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.feat-registry.admin_password      
  }
}
