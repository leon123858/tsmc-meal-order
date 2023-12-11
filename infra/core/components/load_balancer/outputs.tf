output "load_balancer_ip" {
  value = google_compute_global_address.default.address
}
output "load_balancer_ipv6" {
  value = google_compute_global_address.ipv6.address
}