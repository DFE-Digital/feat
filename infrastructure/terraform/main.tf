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

# App Service Plan (Web)
resource "azurerm_service_plan" "feat-web-asp" {
  name                = "${var.prefix}-web-asp"
  location            = azurerm_resource_group.feat-rg.location
  resource_group_name = azurerm_resource_group.feat-rg.name
  os_type             = "Linux"
  sku_name            = "B1"
}

# App Service Plan (Ingestion)
resource "azurerm_service_plan" "feat-ing-asp" {
  name                = "${var.prefix}-ing-asp"
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
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = "false"
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