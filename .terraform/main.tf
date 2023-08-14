terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.48.0"
    }
	  dockerhub = {
      source  = "BarnabyShearer/dockerhub"
      version = ">= 0.0.15"
    }
  }
}
provider "azurerm" {
  features {}
}

variable "DOCKER_USERNAME" {
  type = string
}

variable "DOCKER_PASSWORD" {
  type = string
}

provider "dockerhub" {
  # Note: This cannot be a Personal Access Token
  username = "${var.DOCKER_USERNAME}"
  password = "${var.DOCKER_PASSWORD}"
}

resource "dockerhub_repository" "recordhub-identity" {
  namespace = "skava600"
  name        = "recordhub-identity"
}

resource "dockerhub_repository" "recordhub-catalog" {
  namespace = "skava600"
  name        = "recordhub-catalog"
}

resource "dockerhub_repository" "recordhub-basket" {
  namespace = "skava600"
  name        = "recordhub-basket"
}

resource "dockerhub_repository" "recordhub-ordering" {
  namespace = "skava600"
  name        = "recordhub-ordering"
}

resource "dockerhub_repository" "recordhub-chat" {
  namespace = "skava600"
  name        = "recordhub-chat"
}

resource "dockerhub_repository" "recordhub-mail" {
  namespace = "skava600"
  name        = "recordhub-mail"
}
resource "azurerm_resource_group" "rg" {
  name     = "recordHubResourceGroup"
  location = "northeurope"
}

resource "azurerm_kubernetes_cluster" "cluster" {
  name                = "recordhubscluster"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "recordhubscluster"

  default_node_pool {
    name       = "default"
    node_count = "2"
    vm_size    = "standard_d2_v2"
  }

  identity {
    type = "SystemAssigned"
  }
}