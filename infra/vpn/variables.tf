variable "project_id" {
  description = "value of project_id"
  type = string
}
variable "region" {
  description = "value of region"
  type = string
}
variable "peer_asn" {
  description = "value of peer_asn ex: 64515"
  type = number
}
variable "my_asn" {
    description = "value of my_asn ex: 64512"
    type = number
}
variable "share_secret" {
  description = "value of share_secret"
  type = string
}
variable "azure_vpn_ip_list" {
    description = "value of azure_vpn_ip_list"
    type = list(string)
}
variable "my_bgp_ip" {
    description = "value of my source bgp ip ex: [169.254.21.5, 169.254.21.9]"
    type = list(string)
}
variable "target_bgp_ip_in_azure" {
    description = "value of target bgp ip in azure ex: [169.254.21.6, 169.254.21.10]"
    type = list(string)
}