resource "azapi_resource" "feat_redis_enterprise" {
  type      = "Microsoft.Cache/redisEnterprise@2024-09-01-preview"
  name                         = "${var.prefix}-redis"
  location                     = azurerm_resource_group.feat-rg.location
  parent_id = azurerm_resource_group.feat-rg.id

  body = {
    properties = {
      highAvailability = "Disabled"
    }
    sku = {
      name = "Balanced_B0"
    }
  }

  schema_validation_enabled = false
}

resource "azapi_resource" "feat_redis_cache" {
  type      = "Microsoft.Cache/redisEnterprise/databases@2024-09-01-preview"
  name      = "default"
  location  = azurerm_resource_group.feat-rg.location
  parent_id = azurerm_resource_group.feat-rg.id

  body = {
    properties = {
      clientProtocol   = "Encrypted"
      evictionPolicy   = "VolatileTTL"
      clusteringPolicy = "OSSCluster"
      deferUpgrade     = "NotDeferred"
      persistence = {
        aofEnabled = false
        rdbEnabled = false
      }
      accessKeysAuthentication = "Enabled"
    }
  }

  depends_on = [
    azapi_resource.feat_redis_enterprise
  ]

  schema_validation_enabled = false
}
