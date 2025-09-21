env             = "test"
location        = "UK South"

api_image       = "dfefeattestuksacr.azurecr.io/feat-api:latest"
website_image   = "dfefeattestuksacr.azurecr.io/feat-web:latest"
ingestion_image = "mcr.microsoft.com/dotnet/runtime:9.0"

# Postgres Database configuration
postgresql_admin_login    = "useradmin"
postgresql_admin_password = "useradminpa55word!"
postgresql_flexible_server_sku_name = "B_Standard_B1ms" # e.g. "Standard_B1ms" the most basic sku
