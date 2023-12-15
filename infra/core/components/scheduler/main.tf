data "google_compute_default_service_account" "default" {
}

resource "google_cloud_scheduler_job" "job" {
  name             = var.name
  schedule         = var.frequency # 0 5 * * * 每天早上 5 點
  time_zone        = "CST"
  attempt_deadline = "320s"

  retry_config {
    retry_count = 3
  }

  http_target {
    http_method = "POST"
    uri         = "https://workflowexecutions.googleapis.com/v1/projects/${var.project_id}/locations/${var.region}/workflows/${var.workflow_name}/executions"
    body        = base64encode("{\"argument\":\"${var.parameter}\",\"callLogLevel\":\"CALL_LOG_LEVEL_UNSPECIFIED\"}")
    headers     = {
      "Content-Type" = "application/json"
    }

    oauth_token {
      service_account_email = data.google_compute_default_service_account.default.email
      scope                 = "https://www.googleapis.com/auth/cloud-platform"
    }
  }
}