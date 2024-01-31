# Role assignement for app configuration access
resource "azurerm_role_assignment" "appconfig-role" {
  scope                = azurerm_app_configuration.appconfig.id
  role_definition_name = "App Configuration Data Reader"
  principal_id         = azurerm_user_assigned_identity.identity.principal_id
  depends_on           = [azurerm_user_assigned_identity.identity, azurerm_app_configuration.appconfig]
}

resource "azurerm_role_assignment" "appconfig-dataowner" {
  scope                = azurerm_app_configuration.appconfig.id
  role_definition_name = "App Configuration Data Owner"
  principal_id         = data.azurerm_client_config.current.object_id
  depends_on           = [azurerm_app_configuration.appconfig]
}

# Role assignements for key vault access
resource "azurerm_role_assignment" "kv-admin-role" {
  scope                = azurerm_key_vault.kv.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = data.azurerm_client_config.current.object_id
  depends_on           = [azurerm_key_vault.kv]
}

resource "azurerm_role_assignment" "kv-identity-role" {
  scope                = azurerm_key_vault.kv.id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = azurerm_user_assigned_identity.identity.principal_id
  depends_on           = [azurerm_user_assigned_identity.identity, azurerm_key_vault.kv]
}

# Role assignements for acr <-> aks communication
resource "azurerm_role_assignment" "acr-role" {
  principal_id                     = azurerm_user_assigned_identity.identity.principal_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.acr.id
  skip_service_principal_aad_check = true
  depends_on                       = [azurerm_user_assigned_identity.identity, azurerm_container_registry.acr]
}
