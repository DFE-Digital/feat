terraform {
  required_version = ">= 1.13.3"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.45"
    }
  }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "feat-rg" {
  name     = "${var.prefix}-rg"
  location = var.location

  tags = {
    Environment = var.env
    Product     = var.product
  }
}

# App Service Plan
resource "azurerm_service_plan" "feat-asp" {
  name                = "${var.prefix}-asp"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  os_type             = "Linux"
  sku_name            = "B1"
}

# Azure Container Registry
resource "azurerm_container_registry" "feat-registry" {
  name                = "${var.prefix}acr"
  resource_group_name = azurerm_resource_group.feat-rg.name
  location            = azurerm_resource_group.feat-rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

# Linux Web App - API
resource "azurerm_linux_web_app" "feat-api" {
  name                = "${var.prefix}-app-api"
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
  }
}

# Linux Web App - Website
resource "azurerm_linux_web_app" "feat-website" {
  name                = "${var.prefix}-app-web"
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
