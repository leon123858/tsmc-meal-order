resource "google_secret_manager_secret" "secrets" {
  count     = length(var.service_secrets)
  secret_id = "mail-secret-${var.service_secrets[count.index]}"
  replication {
    user_managed {
      replicas {
        location = var.region
      }
    }
  }
}

resource "google_secret_manager_secret_version" "secret_version0" {
  count       = length(var.service_secrets)
  secret      = google_secret_manager_secret.secrets[count.index].id
  secret_data = "initial-secret-value"
}

locals {
  tmpSecretMap = {for idx, item in google_secret_manager_secret.secrets : var.service_secrets[idx] => item.secret_id}
}

resource "google_cloud_run_v2_service" "default" {
  name     = var.service_name
  location = var.region
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    service_account = var.service_account

    dynamic "volumes" {
      for_each = var.cloudsql_instance == "" ?{} : { cloudsql : var.cloudsql_instance }
      content {
        name               = volumes.key
        cloud_sql_instance {
          instances = [volumes.value]
        }
      }
    }

    containers {
      image = "adrianrudnik/maintenance-server:latest"
      env {
        name  = "SERVER_PORT"
        value = ":8080"
      }
      dynamic "env" {
        for_each = var.service_env
        content {
          name  = env.key
          value = env.value
        }
      }
      dynamic "env" {
        for_each = local.tmpSecretMap
        content {
          name = env.key
          value_source {
            secret_key_ref {
              secret  = env.value
              version = "latest"
            }
          }
        }
      }

      dynamic "volume_mounts" {
        for_each = var.cloudsql_instance == "" ?{} : { cloudsql : "/cloudsql" }
        content {
          name       = volume_mounts.key
          mount_path = volume_mounts.value
        }
      }
    }
  }
}

resource "google_cloud_run_service_iam_member" "member" {
  location = google_cloud_run_v2_service.default.location
  project  = google_cloud_run_v2_service.default.project
  service  = google_cloud_run_v2_service.default.name
  role     = "roles/run.invoker"
  member   = "allUsers"
}