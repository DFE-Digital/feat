variable "env" {
  description = "Environment (dev, test)"
  type        = string
}

variable "prefix" {
    description = "Prefix for resource names"
    type        = string
    default     = "s213"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "uksouth"
}

variable "api_image" {
  description = "Docker image for API"
  type        = string
}

variable "website_image" {
  description = "Docker image for Website"
  type        = string
}

//--------------------------
// PostgreSQL Configuration

variable "subscription_id" {
  description = "The Azure subscription ID."
  type        = string
  #default     = "00000000-0000-0000-0000-000000000000"   
}

variable "postgresql_admin_login" {
  description = "The administrator login for the PostgreSQL flexible server."
  type        = string  
}

variable "postgresql_admin_password" {
  description = "The administrator password for the PostgreSQL flexible server."
  type        = string  
}

variable "postgresql_flexible_server_sku_name" {
  description = "The SKU name for the Postgres flexible server."
  type        = string
  default     = "B_Standard_B1ms" // Set as appropriate   
}

