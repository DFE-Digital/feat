env             = "test"
prefix          = "s213t01"
location        = "UK South"

api_image       = "dfefeattestuksacr.azurecr.io/feat-api:latest"
website_image   = "dfefeattestuksacr.azurecr.io/feat-web:latest"
ingestion_image = "mcr.microsoft.com/dotnet/runtime:9.0"

# Postgres Database configuration
postgresql_admin_login    = ""
postgresql_admin_password = ""
postgresql_flexible_server_sku_name = "B_Standard_B1ms" # e.g. "Standard_B1ms" the most basic sku

subscription_id = "00000000-0000-0000-0000-000000000000" # Set subscription ID
