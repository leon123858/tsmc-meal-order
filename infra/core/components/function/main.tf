resource "google_storage_bucket_object" "object" {
  name   = "${var.service_name}-source.zip"
  bucket = var.source_bucket
  source = "${path.module}/function-source.zip" # Add path to the zipped function source code
}

resource "google_cloudfunctions2_function" "default" {
  name     = var.service_name
  location = var.region

  build_config {
    runtime     = "nodejs16"
    entry_point = "helloHttp" # Set the entry point

    source {
      storage_source {
        bucket = google_storage_bucket_object.object.bucket
        object = google_storage_bucket_object.object.name
      }
    }
  }

  service_config {
    max_instance_count    = 1
    available_memory      = "256M"
    timeout_seconds       = 60
    service_account_email = var.service_account_email
  }
}

resource "google_cloud_run_service_iam_member" "member" {
  location = google_cloudfunctions2_function.default.location
  service  = google_cloudfunctions2_function.default.name
  role     = "roles/run.invoker"
  member   = "allUsers"
}