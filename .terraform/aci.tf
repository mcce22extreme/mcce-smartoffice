resource "azurerm_container_group" "mosquittoaci" {
  name                = "aci-mosquitto-${random_string.name-prefix.result}"
  location            = var.azure_location
  resource_group_name = var.azure_resourcegroup
  ip_address_type     = "Public"
  dns_name_label      = var.smartoffice_mqtt_dns_prefix
  os_type             = "Linux"
  container {
    name   = "mosquitto"
    image  = "eclipse-mosquitto:2.0.18"
    cpu    = "0.5"
    memory = "1.5"
    ports {
      port     = 1883
      protocol = "TCP"
    }
    volume {
      name                 = "config"
      mount_path           = "/mosquitto/config"
      read_only            = true
      share_name           = azurerm_storage_share.mosquitto-share.name
      storage_account_name = azurerm_storage_account.storage.name
      storage_account_key  = azurerm_storage_account.storage.primary_access_key
    }
  }
}

resource "local_file" "mosquitto-config" {
  content  = <<-EOT
    allow_anonymous true
    listener 1883
    protocol mqtt
  EOT
  filename = "mosquitto.conf"
}

resource "azurerm_storage_share_file" "mosquitto-config-file" {
  name             = "mosquitto.conf"
  storage_share_id = azurerm_storage_share.mosquitto-share.id
  source           = local_file.mosquitto-config.filename
}
