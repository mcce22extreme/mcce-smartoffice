resource "azurerm_service_plan" "example" {
  name                = "asp-smartoffice-${random_string.name-prefix.result}"
  resource_group_name = var.azure_resourcegroup
  location            = var.azure_location
  os_type             = "Linux"
  sku_name            = "B1"
}

# resource "azurerm_app_service_plan" "appserviceplan" {
#   name                = "asp-smartoffice-${random_string.name-prefix.result}"
#   location            = var.azure_location
#   resource_group_name = var.azure_resourcegroup
#   kind                = "Linux"
#   reserved            = true
#   sku {
#     tier = "Basic"
#     size = "B1"
#   }
# }

# resource "azurerm_app_service" "appservice" {
#   name                = "as-smartoffice-${random_string.name-prefix.result}"
#   location            = azurerm_resource_group.example.location
#   resource_group_name = azurerm_resource_group.example.name
#   app_service_plan_id = azurerm_app_service_plan.appserviceplan.id
#   identity {
#     type = "SystemAssigned"
#   }
#   app_settings = {
#     DOCKER_REGISTRY_SERVER_URL          = azurerm_container_registry.acr.0.login_server
#     DOCKER_REGISTRY_SERVER_USERNAME     = azurerm_container_registry.acr.0.admin_username
#     DOCKER_REGISTRY_SERVER_PASSWORD     = azurerm_container_registry.acr.0.admin_password
#     WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
#     WEBSITES_PORT                       = local.environmentvars["backend_port"]
#   }
# }

# resource "azurerm_role_assignment" "appservice" {
#   principal_id                     = azurerm_app_service.backend.identity.0.principal_id
#   role_definition_name             = "AcrPull"
#   scope                            = azurerm_container_registry.registry.0.id
#   skip_service_principal_aad_check = true
# }
