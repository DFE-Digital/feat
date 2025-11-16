<!-- BEGIN_TF_DOCS -->
## Requirements

| Name | Version |
|------|---------|
| <a name="requirement_terraform"></a> [terraform](#requirement\_terraform) | >= 1.13.4 |
| <a name="requirement_azapi"></a> [azapi](#requirement\_azapi) | 2.7.0 |
| <a name="requirement_azurerm"></a> [azurerm](#requirement\_azurerm) | ~> 4.52 |

## Providers

| Name | Version |
|------|---------|
| <a name="provider_azapi"></a> [azapi](#provider\_azapi) | 2.7.0 |
| <a name="provider_azurerm"></a> [azurerm](#provider\_azurerm) | ~> 4.52 |

## Modules

No modules.

## Resources

| Name | Type |
|------|------|
| [azapi_resource.feat_ingestion_subnet](https://registry.terraform.io/providers/azure/azapi/2.7.0/docs/resources/resource) | resource |
| [azapi_resource.feat_main_subnet](https://registry.terraform.io/providers/azure/azapi/2.7.0/docs/resources/resource) | resource |
| [azurerm_app_service_virtual_network_swift_connection.api_app_vn_conn](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/app_service_virtual_network_swift_connection) | resource |
| [azurerm_app_service_virtual_network_swift_connection.website_app_vn_conn](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/app_service_virtual_network_swift_connection) | resource |
| [azurerm_container_app_environment.feat-ingestion-environment](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/container_app_environment) | resource |
| [azurerm_container_registry.feat-registry](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/container_registry) | resource |
| [azurerm_linux_web_app.feat-api](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/linux_web_app) | resource |
| [azurerm_linux_web_app.feat-website](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/linux_web_app) | resource |
| [azurerm_log_analytics_workspace.feat_logging](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/log_analytics_workspace) | resource |
| [azurerm_managed_redis.feat_redis_enterprise](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/managed_redis) | resource |
| [azurerm_mssql_database.feat_mssql_db](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/mssql_database) | resource |
| [azurerm_mssql_server.feat_mssql_server](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/mssql_server) | resource |
| [azurerm_mssql_virtual_network_rule.mssql_vnet_rule](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/mssql_virtual_network_rule) | resource |
| [azurerm_network_security_group.feat-nsg](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/network_security_group) | resource |
| [azurerm_private_dns_zone.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/private_dns_zone) | resource |
| [azurerm_private_dns_zone_virtual_network_link.default](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/private_dns_zone_virtual_network_link) | resource |
| [azurerm_resource_group.feat-rg](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/resource_group) | resource |
| [azurerm_search_service.feat_search_service](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/search_service) | resource |
| [azurerm_service_plan.feat-web-asp](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/service_plan) | resource |
| [azurerm_storage_account.feat_storage_account](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/storage_account) | resource |
| [azurerm_virtual_network.feat_vnet](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/virtual_network) | resource |

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_api_image_name"></a> [api\_image\_name](#input\_api\_image\_name) | The repository name and tag for the API container. | `string` | n/a | yes |
| <a name="input_env"></a> [env](#input\_env) | Environment (dev, test, prod) | `string` | n/a | yes |
| <a name="input_location"></a> [location](#input\_location) | Azure region | `string` | `"uksouth"` | no |
| <a name="input_partition_count"></a> [partition\_count](#input\_partition\_count) | Partitions allow for scaling of document count as well as faster indexing by sharding your index over multiple search units. | `number` | `1` | no |
| <a name="input_prefix"></a> [prefix](#input\_prefix) | Prefix for resource names | `string` | `"s265"` | no |
| <a name="input_product"></a> [product](#input\_product) | Name of the project | `string` | `"Find Education and Training"` | no |
| <a name="input_replica_count"></a> [replica\_count](#input\_replica\_count) | Replicas distribute search workloads across the service. You need at least two replicas to support high availability of query workloads (not applicable to the free tier). | `number` | `1` | no |
| <a name="input_sku"></a> [sku](#input\_sku) | The pricing tier of the search service you want to create (for example, basic or standard). | `string` | `"standard"` | no |
| <a name="input_sql_admin_password"></a> [sql\_admin\_password](#input\_sql\_admin\_password) | The administrator password of the SQL logical server. | `string` | `null` | no |
| <a name="input_sql_admin_username"></a> [sql\_admin\_username](#input\_sql\_admin\_username) | The administrator username of the SQL logical server. | `string` | `"azureadmin"` | no |
| <a name="input_website_image_name"></a> [website\_image\_name](#input\_website\_image\_name) | The repository name and tag for the website container. | `string` | n/a | yes |

## Outputs

| Name | Description |
|------|-------------|
| <a name="output_api_url"></a> [api\_url](#output\_api\_url) | n/a |
| <a name="output_azurerm_network_security_group"></a> [azurerm\_network\_security\_group](#output\_azurerm\_network\_security\_group) | Network security group name |
| <a name="output_azurerm_private_dns_zone"></a> [azurerm\_private\_dns\_zone](#output\_azurerm\_private\_dns\_zone) | Private DNS Zone name |
| <a name="output_azurerm_private_dns_zone_virtual_network_link"></a> [azurerm\_private\_dns\_zone\_virtual\_network\_link](#output\_azurerm\_private\_dns\_zone\_virtual\_network\_link) | Private DNS Zone Virtual Network Link name |
| <a name="output_azurerm_subnet"></a> [azurerm\_subnet](#output\_azurerm\_subnet) | Subnet name |
| <a name="output_azurerm_virtual_network"></a> [azurerm\_virtual\_network](#output\_azurerm\_virtual\_network) | Virtual network name |
| <a name="output_cache_hostname"></a> [cache\_hostname](#output\_cache\_hostname) | The DNS hostname for the cache |
| <a name="output_cache_port"></a> [cache\_port](#output\_cache\_port) | The port the managed redis cache is running on |
| <a name="output_cache_primary_access_key"></a> [cache\_primary\_access\_key](#output\_cache\_primary\_access\_key) | The Managed Redis primary access key. |
| <a name="output_mssql_database_name"></a> [mssql\_database\_name](#output\_mssql\_database\_name) | The name of the SQL Database |
| <a name="output_mssql_server_fqdn"></a> [mssql\_server\_fqdn](#output\_mssql\_server\_fqdn) | The fully qualified domain name of the SQL Server |
| <a name="output_mssql_server_name"></a> [mssql\_server\_name](#output\_mssql\_server\_name) | The name of the SQL Server |
| <a name="output_search_service_name"></a> [search\_service\_name](#output\_search\_service\_name) | The name of the Azure Search Service |
| <a name="output_website_url"></a> [website\_url](#output\_website\_url) | n/a |
<!-- END_TF_DOCS -->