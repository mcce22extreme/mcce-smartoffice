resource "azurerm_user_assigned_identity" "identity" {
  name                = "id-smartoffice-${random_string.name-prefix.result}"
  location            = var.azure_location
  resource_group_name = var.azure_resourcegroup
}
