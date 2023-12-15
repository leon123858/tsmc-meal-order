resource "google_cloudbuild_trigger" "manual-trigger" {
  location = var.region
  name     = "${var.service_name}-trigger"

  source_to_build {
    repository = var.source_repo
    ref        = "refs/heads/main"
    repo_type  = "GITHUB"
  }

  build {
    step {
      name = "gcr.io/google.com/cloudsdktool/cloud-sdk"
      args = [
        "gcloud", "functions", "deploy", var.service_name, "--gen2", "--runtime=go121",
        "--region=${var.region}", "--source=${var.function_path}",
        "--entry-point=${var.entry_point}"
      ]
    }
  }

  // If this is set on a build, it will become pending when it is run,
  // and will need to be explicitly approved to start.
  approval_config {
    approval_required = false
  }
}