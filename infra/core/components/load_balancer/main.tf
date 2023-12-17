resource "google_compute_global_address" "default" {
  name = "${var.name}-address"
}

resource "google_compute_global_address" "ipv6" {
  name       = "${var.name}-address-ipv6"
  ip_version = "IPV6"
}

resource "google_compute_managed_ssl_certificate" "default" {
  name = "${var.name}-cert"
  managed {
    domains = [var.domain]
  }
}

resource "google_compute_region_network_endpoint_group" "cloudrun_neg" {
  for_each              = var.cloud_run_services
  name                  = "${each.key}-neg"
  network_endpoint_type = "SERVERLESS"
  region                = var.region
  cloud_run {
    service = each.value
  }
}

resource "google_compute_region_network_endpoint_group" "cloud_function_neg" {
  for_each              = var.cloud_function_services
  name                  = "${each.key}-neg"
  network_endpoint_type = "SERVERLESS"
  region                = var.region
  cloud_function {
    function = each.value
  }
}

locals {
  backend_run_services_neg = {
    for k, v in var.cloud_run_services : k => google_compute_region_network_endpoint_group.cloudrun_neg[k].id
  }
  backend_function_services_neg = {
    for k, v in var.cloud_function_services : k => google_compute_region_network_endpoint_group.cloud_function_neg[k].id
  }
  backend_services_neg = merge(local.backend_run_services_neg, local.backend_function_services_neg)
}

// cloud armor
resource "google_compute_security_policy" "policy" {
  provider    = google-beta
  name        = "${var.name}-armor-policy"
  description = "throttle rule with enforce_on_key_configs"

  rule {
    action   = "throttle"
    priority = "2147483647"
    match {
      versioned_expr = "SRC_IPS_V1"
      config {
        src_ip_ranges = ["*"]
      }
    }
    description = "default rule"
    rate_limit_options {
      conform_action = "allow"
      exceed_action  = "deny(429)"

      enforce_on_key = ""

      rate_limit_threshold {
        count        = 10
        interval_sec = 60
      }

      enforce_on_key_configs {
        enforce_on_key_type = "IP"
      }
    }
  }
}

resource "google_compute_backend_service" "default" {
  for_each = local.backend_services_neg
  name     = "${each.key}-backend"

  protocol  = "HTTP"
  port_name = "http"

  security_policy = google_compute_security_policy.policy.id

  backend {
    group = each.value
  }
}

locals {
  backend_services = {
    for k, v in local.backend_services_neg : k => google_compute_backend_service.default[k].id
  }
}

resource "google_compute_backend_bucket" "static" {
  name        = "${var.name}-bucket"
  bucket_name = var.bucket

  enable_cdn = true
}

// url map
resource "google_compute_url_map" "urlmap" {
  name            = "${var.name}-urlmap"
  default_service = local.backend_services[var.default_service_name]

  host_rule {
    hosts        = [var.domain, google_compute_global_address.default.address]
    path_matcher = "default"
  }

  path_matcher {
    name            = "default"
    default_service = local.backend_services[var.default_service_name]

    dynamic "path_rule" {
      for_each = var.service_map_url
      content {
        paths   = [path_rule.value, "${path_rule.value}/*"]
        service = local.backend_services[path_rule.key]
      }
    }

    path_rule {
      paths   = ["/${var.bucket_prefix}/*"]
      service = google_compute_backend_bucket.static.id
    }
  }
}

// https handler
resource "google_compute_target_https_proxy" "default" {
  name = "${var.name}-https-proxy"

  url_map          = google_compute_url_map.urlmap.id
  ssl_certificates = [
    google_compute_managed_ssl_certificate.default.id
  ]
}

resource "google_compute_global_forwarding_rule" "default" {
  name = "${var.name}-lb"

  target     = google_compute_target_https_proxy.default.id
  port_range = "443"
  ip_address = google_compute_global_address.default.address
}

#resource "google_compute_global_forwarding_rule" "ipv6" {
#  name = "${var.name}-lb-ipv6"
#
#  target     = google_compute_target_https_proxy.default.id
#  port_range = "443"
#  ip_address = google_compute_global_address.ipv6.address
#}

// redirect http to https (optional 轉向 https 或 handle http 擇一)
#resource "google_compute_url_map" "https_redirect" {
#  name = "${var.name}-https-redirect"
#
#  default_url_redirect {
#    https_redirect         = true
#    redirect_response_code = "MOVED_PERMANENTLY_DEFAULT"
#    strip_query            = false
#  }
#}

#resource "google_compute_target_http_proxy" "https_redirect" {
#  name    = "${var.name}-http-proxy"
#  url_map = google_compute_url_map.https_redirect.id
#}
#
#resource "google_compute_global_forwarding_rule" "https_redirect" {
#  name = "${var.name}-lb-http"
#
#  target     = google_compute_target_http_proxy.https_redirect.id
#  port_range = "80"
#  ip_address = google_compute_global_address.default.address
#}

// http handler (optional)
#resource "google_compute_target_http_proxy" "default" {
#  name    = "${var.name}-http-proxy"
#  url_map = google_compute_url_map.urlmap.id
#}
#
#resource "google_compute_global_forwarding_rule" "http" {
#  name = "${var.name}-http-lb"
#
#  target     = google_compute_target_http_proxy.default.id
#  port_range = "80"
#  ip_address = google_compute_global_address.default.address
#}