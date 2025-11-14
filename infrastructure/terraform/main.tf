terraform {
  required_version = ">= 1.13.4"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.52"
    }
    azapi = {
      source  = "azure/azapi"
      version = "2.7.0"
    }
  }

  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

provider "azapi" {

}

# Resource Group
resource "azurerm_resource_group" "feat-rg" {
  name     = "${var.prefix}rg-uks-feat"
  location = var.location

  tags = {
    Environment = var.env
    Product     = var.product
  }
}