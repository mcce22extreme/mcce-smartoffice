resource "azurerm_mssql_server" "sqlserver" {
  name                         = "dbsrv-smartoffice-${random_string.name-prefix.result}"
  resource_group_name          = var.azure_resourcegroup
  location                     = var.azure_location
  version                      = "12.0"
  administrator_login          = var.smartoffice_dbadmin_username
  administrator_login_password = var.smartoffice_dbadmin_password
}

resource "azurerm_mssql_database" "database" {
  name           = "sqldb-smartoffice-${random_string.name-prefix.result}"
  server_id      = azurerm_mssql_server.sqlserver.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 2
  read_scale     = false
  sku_name       = "Basic"
  zone_redundant = false
}

resource "azurerm_mssql_firewall_rule" "azureservicerule" {
  name             = "sqlrule-smartoffice-${random_string.name-prefix.result}"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
