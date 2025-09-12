variable "env" {
  description = "Environment (dev, test)"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "West Europe"
}

variable "api_image" {
  description = "Docker image for API"
  type        = string
}

variable "website_image" {
  description = "Docker image for Website"
  type        = string
}

variable "ingestion_image" {
  description = "Docker image for Ingestion Service"
  type        = string
}
