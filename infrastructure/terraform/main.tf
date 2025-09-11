terraform {
  required_version = ">= 1.13.2"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# ---------------------------
# Random suffix for uniqueness
# ---------------------------
resource "random_string" "suffix" {
  length  = 4
  upper   = false
  numeric = true
  special = false
}

# ---------------------------
# Resource Group
# ---------------------------
resource "azurerm_resource_group" "this" {
  name     = "${var.env}-rg-feat"
  location = var.location
}

# ---------------------------
# Storage Account (optional)
# ---------------------------
resource "azurerm_storage_account" "this" {
  name                     = "${var.env}featstorage"
  resource_group_name      = azurerm_resource_group.this.name
  location                 = azurerm_resource_group.this.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

# ---------------------------
# App Service Plan
# ---------------------------
resource "azurerm_service_plan" "this" {
  name                = "${var.env}-appservice-plan-${random_string.suffix.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  os_type             = "Linux"
  sku_name            = "B1"
}

# ---------------------------
# Azure Container Registry
# ---------------------------
resource "azurerm_container_registry" "this" {
  name                = "featacr${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  sku                 = "Basic"
  admin_enabled       = true
}

# ---------------------------
# Linux Web App - API
# ---------------------------
resource "azurerm_linux_web_app" "api" {
  name                = "${var.env}-api-${random_string.suffix.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  service_plan_id     = azurerm_service_plan.this.id

  site_config {} # required block

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_CUSTOM_IMAGE_NAME"            = var.api_image
    "DOCKER_REGISTRY_SERVER_URL"          = azurerm_container_registry.this.login_server
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.this.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.this.admin_password
  }
}

# ---------------------------
# Linux Web App - Website
# ---------------------------
resource "azurerm_linux_web_app" "website" {
  name                = "${var.env}-website-${random_string.suffix.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  service_plan_id     = azurerm_service_plan.this.id

  site_config {} # required block

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_CUSTOM_IMAGE_NAME"            = var.website_image
    "DOCKER_REGISTRY_SERVER_URL"          = azurerm_container_registry.this.login_server
    "DOCKER_REGISTRY_SERVER_USERNAME"     = azurerm_container_registry.this.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = azurerm_container_registry.this.admin_password
  }
}

# ---------------------------
# Ingestion Container Group
# ---------------------------
resource "azurerm_container_group" "ingestion" {
  name                = "${var.env}-ingestion-${random_string.suffix.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  os_type             = "Linux"

  container {
    name   = "ingestion"
    image  = var.ingestion_image
    cpu    = "0.5"
    memory = "1.0"
    commands = [
      "/bin/sh",
      "-c",
      "echo 'Running ingestion service'"
    ]
  }

  ip_address_type = "None"
  restart_policy  = "Never"

  image_registry_credential {
    server   = azurerm_container_registry.this.login_server
    username = azurerm_container_registry.this.admin_username
    password = azurerm_container_registry.this.admin_password
  }

  tags = {
    environment = var.env
  }
}
