variable "env" {
  description = "Environment (dev, test, prod)"
  type        = string
}

variable "product" {
  description = "Name of the project"
  type        = string
  default     = "Find Education and Training"
}

variable "prefix" {
  description = "Prefix for resource names"
  type        = string
  default     = "s265"
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "uksouth"
}

variable "api_image_name" {
  type        = string
  description = "The repository name and tag for the API container."
}

variable "website_image_name" {
  type        = string
  description = "The repository name and tag for the website container."
}

variable "sql_admin_username" {
  type        = string
  description = "The administrator username of the SQL logical server."
  default     = "azureadmin"
}

variable "sql_admin_password" {
  type        = string
  description = "The administrator password of the SQL logical server."
  sensitive   = true
  default     = null
}

variable "openai_key" {
  type        = string
  description = "The OpenAI Key used for text embeddings."
  sensitive   = true
  default     = null
}

# Azure Search Service Configuration variables
variable "ai_search_sku" {
  description = "The SKU which should be used for this Search Service. Possible values include basic, free, standard, standard2, standard3, storage_optimized_l1 and storage_optimized_l2"
  default     = "standard"
  type        = string
  validation {
    condition     = contains(["free", "basic", "standard", "standard2", "standard3", "storage_optimized_l1", "storage_optimized_l2"], var.ai_search_sku)
    error_message = "The sku must be one of the following values: free, basic, standard, standard2, standard3, storage_optimized_l1, storage_optimized_l2."
  }
}

variable "ai_search_semantic_sku"
{
  description = "Specifies the Semantic Search SKU which should be used for this Search Service. Possible values include free and standard"
  default     = "standard"
  type        = string
  validation {
    condition     = contains(["free", "standard"], var.ai_search_semantic_sku)
    error_message = "The sku must be one of the following values: free, standard."
  }
}

variable "replica_count" {
  type        = number
  description = "Replicas distribute search workloads across the service. You need at least two replicas to support high availability of query workloads (not applicable to the free tier)."
  default     = 1
  validation {
    condition     = var.replica_count >= 1 && var.replica_count <= 12
    error_message = "The replica_count must be between 1 and 12."
  }
}

variable "partition_count" {
  type        = number
  description = "Partitions allow for scaling of document count as well as faster indexing by sharding your index over multiple search units."
  default     = 1
  validation {
    condition     = contains([1, 2, 3, 4, 6, 12], var.partition_count)
    error_message = "The partition_count must be one of the following values: 1, 2, 3, 4, 6, 12."
  }
}