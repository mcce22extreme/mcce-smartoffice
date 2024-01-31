resource "azurerm_app_configuration" "appconfig" {
  name                     = "cfg-smartoffice-${random_string.name-prefix.result}"
  resource_group_name      = var.azure_resourcegroup
  location                 = var.azure_location
  sku                      = "free"
  purge_protection_enabled = false
}

resource "azurerm_app_configuration_key" "appconfig-authurl" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "authconfig:authurl"
  value                  = var.smartoffice_authurl
}

resource "azurerm_app_configuration_key" "appconfig-authclientid" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "authconfig:clientid"
  value                  = var.smartoffice_authclientid
}

resource "azurerm_app_configuration_key" "appconfig-connectionstring" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner, azurerm_key_vault_secret.dbconfig-connectionstring]
  key                    = "dbconfig:connectionstring"
  type                   = "vault"
  vault_key_reference    = azurerm_key_vault_secret.dbconfig-connectionstring.versionless_id
}

resource "azurerm_app_configuration_key" "appconfig-databasetype" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "dbconfig:databasetype"
  value                  = "SqlServer"
}

# resource "azurerm_app_configuration_key" "appconfig-frontendUrl" {
#   configuration_store_id = azurerm_app_configuration.appconfig.id
#   depends_on             = [azurerm_role_assignment.appconfig-dataowner, azurerm_kubernetes_cluster.aks]
#   key                    = "frontendurl"
#   value                  = "https://${azurerm_kubernetes_cluster.aks.dns_prefix}.${var.azure_location}.cloudapp.azure.com"
# }

resource "azurerm_app_configuration_key" "appconfig-mqtt-hostname" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "mqttconfig:hostname"
  value                  = "${var.smartoffice_mqtt_dns_prefix}.${var.azure_location}.cloudapp.azure.com"
}

resource "azurerm_app_configuration_key" "appconfig-mqtt-port" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "mqttconfig:port"
  value                  = var.smartoffice_mqtt_port
}

resource "azurerm_app_configuration_key" "appconfig-mqtt-username" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner]
  key                    = "mqttconfig:username"
  value                  = var.smartoffice_mqtt_username
}

resource "azurerm_app_configuration_key" "appconfig-mqtt-password" {
  configuration_store_id = azurerm_app_configuration.appconfig.id
  depends_on             = [azurerm_role_assignment.appconfig-dataowner, azurerm_key_vault_secret.mqttconfig-password]
  key                    = "mqttconfig:password"
  type                   = "vault"
  vault_key_reference    = azurerm_key_vault_secret.mqttconfig-password.versionless_id
}
