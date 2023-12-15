// VPC Network with HA VPN Gateway and Router
resource "google_compute_network" "network" {
  name                    = "network-vpn-ha"
  routing_mode            = "GLOBAL"
  auto_create_subnetworks = false
}

resource "google_compute_subnetwork" "network_subnet1" {
  name          = "ha-vpn-subnet-1"
  ip_cidr_range = "10.8.0.0/28"
  region        = var.region
  network       = google_compute_network.network.id
}

resource "google_compute_ha_vpn_gateway" "ha_gateway" {
  region  = var.region
  name    = "ha-vpn"
  network = google_compute_network.network.id
}

resource "google_compute_router" "router1" {
  name    = "ha-vpn-router1"
  network = google_compute_network.network.name
  bgp {
    asn = var.my_asn
  }
}

// External VPN Gateway
resource "google_compute_external_vpn_gateway" "external_gateway" {
  name            = "external-gateway"
  redundancy_type = "TWO_IPS_REDUNDANCY"
  description     = "An externally managed VPN gateway for azure"
  interface {
    id         = 0
    ip_address = var.azure_vpn_ip_list[0]
  }
  interface {
    id         = 1
    ip_address = var.azure_vpn_ip_list[1]
  }
}

// create VPN tunnels
resource "google_compute_vpn_tunnel" "tunnel1" {
  name                            = "ha-vpn-tunnel1"
  region                          = var.region
  vpn_gateway                     = google_compute_ha_vpn_gateway.ha_gateway.id
  peer_external_gateway           = google_compute_external_vpn_gateway.external_gateway.id
  peer_external_gateway_interface = 0
  shared_secret                   = var.share_secret
  router                          = google_compute_router.router1.id
  vpn_gateway_interface           = 0
}

resource "google_compute_vpn_tunnel" "tunnel2" {
  name                            = "ha-vpn-tunnel2"
  region                          = var.region
  vpn_gateway                     = google_compute_ha_vpn_gateway.ha_gateway.id
  peer_external_gateway           = google_compute_external_vpn_gateway.external_gateway.id
  peer_external_gateway_interface = 1
  shared_secret                   = var.share_secret
  router                          = google_compute_router.router1.id
  vpn_gateway_interface           = 1
}

// BGP Peer
resource "google_compute_router_interface" "router1_interface1" {
  name       = "router1-interface1"
  router     = google_compute_router.router1.name
  region     = var.region
  ip_range   = "${var.my_bgp_ip[0]}/30"
  vpn_tunnel = google_compute_vpn_tunnel.tunnel1.name
}

resource "google_compute_router_peer" "router1_peer1" {
  name                      = "router1-peer1"
  router                    = google_compute_router.router1.name
  region                    = var.region
  peer_ip_address           = var.target_bgp_ip_in_azure[0]
  peer_asn                  = var.peer_asn
  advertised_route_priority = 100
  interface                 = google_compute_router_interface.router1_interface1.name
}

resource "google_compute_router_interface" "router1_interface2" {
  name       = "router1-interface2"
  router     = google_compute_router.router1.name
  region     = var.region
  ip_range   = "${var.my_bgp_ip[1]}/30"
  vpn_tunnel = google_compute_vpn_tunnel.tunnel2.name
}

resource "google_compute_router_peer" "router1_peer2" {
  name                      = "router1-peer2"
  router                    = google_compute_router.router1.name
  region                    = var.region
  peer_ip_address           = var.target_bgp_ip_in_azure[1]
  peer_asn                  = var.peer_asn
  advertised_route_priority = 100
  interface                 = google_compute_router_interface.router1_interface2.name
}

// serverless vpc connector
module "serverless-connector" {
  source         = "terraform-google-modules/network/google//modules/vpc-serverless-connector-beta"
  version        = "~> 8.0"
  project_id     = var.project_id
  vpc_connectors = [
    {
      name          = "serverless"
      region        = var.region
      subnet_name   = google_compute_subnetwork.network_subnet1.name
      machine_type  = "f1-micro"
      min_instances = 2
      max_instances = 3
    }
    # Uncomment to specify an ip_cidr_range
    #   , {
    #     name          = "central-serverless2"
    #     region        = "us-central1"
    #     network       = module.test-vpc-module.network_name
    #     ip_cidr_range = "10.10.11.0/28"
    #     subnet_name   = null
    #     machine_type  = "e2-standard-4"
    #     min_instances = 2
    #   max_instances = 7 }
  ]
}