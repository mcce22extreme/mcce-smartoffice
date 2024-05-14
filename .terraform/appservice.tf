resource "azurerm_service_plan" "appserviceplan" {
  name                = "asp-smartoffice-${random_string.name-prefix.result}"
  resource_group_name = var.azure_resourcegroup
  location            = var.azure_location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "smartoffice-api" {
  name                = var.smartoffice_app_dns_prefix
  resource_group_name = var.azure_resourcegroup
  location            = var.azure_location
  service_plan_id     = azurerm_service_plan.appserviceplan.id

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on                               = false
    container_registry_use_managed_identity = true
    application_stack {
      docker_image_name   = "mcce-smart-office-api:latest"
      docker_registry_url = "https://${azurerm_container_registry.acr.login_server}"
    }
  }

  storage_account {
    name         = "userimages"
    type         = "AzureFiles"
    account_name = azurerm_storage_account.storage.name
    share_name   = azurerm_storage_share.userimage-share.name
    access_key   = azurerm_storage_account.storage.primary_access_key
    mount_path   = "/mnt/smartoffice/userimages"
  }

  app_settings = {
    "SMARTOFFICE_AUTHCONFIG__AUTHURL"  = var.smartoffice_authurl
    "SMARTOFFICE_AUTHCONFIG__CLIENTID" = var.smartoffice_authclientid
    "SMARTOFFICE_BASEADDRESS"          = var.smartoffice_baseaddress
    "SMARTOFFICE_CONNECTIONSTRING"     = "Server=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.database.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sqlserver.administrator_login};Password=${azurerm_mssql_server.sqlserver.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    "SMARTOFFICE_FRONTENDURL"          = "https://${var.smartoffice_app_dns_prefix}.azurewebsites.net"
    "SMARTOFFICE_MQTTCONFIG__HOSTNAME" = azurerm_container_group.mosquittoaci.fqdn
    "SMARTOFFICE_MQTTCONFIG__PORT"     = 1883
    "SMARTOFFICE_STORAGEPATH"          = "/mnt/smartoffice/userimages"
  }
}

resource "azurerm_role_assignment" "appservice-acr-pull" {
  principal_id                     = azurerm_linux_web_app.smartoffice-api.identity.0.principal_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.acr.id
  skip_service_principal_aad_check = true
}
