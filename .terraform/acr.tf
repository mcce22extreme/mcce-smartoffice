resource "azurerm_container_registry" "acr" {
  name                = "acrsmartoffice${random_string.name-prefix.result}"
  resource_group_name = var.azure_resourcegroup
  location            = var.azure_location
  sku                 = "Basic"
}
