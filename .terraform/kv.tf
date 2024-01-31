resource "azurerm_key_vault" "kv" {
  name                        = "kv-smartoffice-${random_string.name-prefix.result}"
  resource_group_name         = var.azure_resourcegroup
  location                    = var.azure_location
  enabled_for_disk_encryption = false
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false
  enable_rbac_authorization   = true
  sku_name                    = "standard"
}

resource "azurerm_key_vault_secret" "mqttconfig-password" {
  name         = "mqttpassword"
  value        = var.smartoffice_mqtt_password
  key_vault_id = azurerm_key_vault.kv.id
  depends_on   = [azurerm_role_assignment.kv-admin-role]
}

resource "azurerm_key_vault_secret" "dbconfig-connectionstring" {
  name         = "connectionstring"
  value        = "Server=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.database.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sqlserver.administrator_login};Password=${azurerm_mssql_server.sqlserver.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  key_vault_id = azurerm_key_vault.kv.id
  depends_on   = [azurerm_mssql_server.sqlserver, azurerm_role_assignment.kv-admin-role]
}
