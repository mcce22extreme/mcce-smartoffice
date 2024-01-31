variable "azure_location" {
  type    = string
  default = "westeurope"
}

variable "azure_resourcegroup" {
  type    = string
  default = "rg-smartoffice-001"
}

variable "smartoffice_app_dns_prefix" {
  type    = string
  default = "smartoffice-mcce22extreme"
}

variable "smartoffice_mqtt_dns_prefix" {
  type    = string
  default = "mqtt-mcce22extreme"
}

variable "smartoffice_mqtt_port" {
  type    = string
  default = "1883"
}

variable "smartoffice_mqtt_username" {
  type = string
}

variable "smartoffice_mqtt_password" {
  type      = string
  sensitive = true
}

variable "smartoffice_dbadmin_username" {
  type = string
}

variable "smartoffice_dbadmin_password" {
  type      = string
  sensitive = true
}

variable "smartoffice_authurl" {
  type    = string
  default = "https://keycloak.gruber.info:8943/realms/smartoffice"
}

variable "smartoffice_authclientid" {
  type    = string
  default = "smartoffice"
}
