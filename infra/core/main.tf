provider "google" {
  project = var.project_id
  region  = var.region
}

// source code repository
module "repository" {
  source     = "./components/repository"
  name       = "meal-service"
  region     = var.region
  git_url    = var.git_url
  git_app_id = var.git_app_id
  git_token  = var.git_token
}

// cloud run account
resource "google_service_account" "run" {
  account_id   = "cloud-run-sa"
  display_name = "Service Account For Cloud Run"
  project      = var.project_id
}

resource "google_project_iam_member" "sa1" {
  project    = var.project_id
  role       = "roles/cloudsql.admin"
  member     = "serviceAccount:${google_service_account.run.email}"
  depends_on = [google_service_account.run]
}

resource "google_project_iam_member" "sa2" {
  project    = var.project_id
  role       = "roles/firebase.sdkAdminServiceAgent"
  member     = "serviceAccount:${google_service_account.run.email}"
  depends_on = [google_project_iam_member.sa1]
}

resource "google_project_iam_member" "sa3" {
  project    = var.project_id
  role       = "roles/pubsub.editor"
  member     = "serviceAccount:${google_service_account.run.email}"
  depends_on = [google_project_iam_member.sa1, google_project_iam_member.sa2]
}

resource "google_project_iam_member" "sa4" {
  project    = var.project_id
  role       = "roles/secretmanager.secretAccessor"
  member     = "serviceAccount:${google_service_account.run.email}"
  depends_on = [google_project_iam_member.sa1, google_project_iam_member.sa2, google_project_iam_member.sa3]
}

// init cloud run
module "mail-run" {
  source       = "./components/run"
  service_name = "mail"
  region       = var.region
  service_env  = {
    ASPNETCORE_ENVIRONMENT = "Production",
  }
  service_secrets = ["POSTGRES_PASSWORD", "SMTP_PASSWORD"]
  service_account = google_service_account.run.email
  depends_on      = [
    google_project_iam_member.sa1, google_project_iam_member.sa2, google_project_iam_member.sa3,
    google_project_iam_member.sa4
  ]
  cloudsql_instance = "tw-rd-ca-leon-lin:asia-east1:meal-dev"
}

module "user-run" {
  source       = "./components/run"
  service_name = "user"
  region       = var.region
  service_account = google_service_account.run.email
  depends_on      = [
    google_project_iam_member.sa1, google_project_iam_member.sa2, google_project_iam_member.sa3,
    google_project_iam_member.sa4
  ]
}

// cloud build set
data "google_project" "project" {
}

locals {
  cloud_build_sa = "serviceAccount:${data.google_project.project.number}@cloudbuild.gserviceaccount.com"
}

resource "google_project_iam_member" "build_r0" {
  project = var.project_id
  role    = "roles/run.admin"
  # service account in project named "Cloud Build Service Account"
  member  = local.cloud_build_sa
}

resource "google_project_iam_member" "build_r1" {
  project = var.project_id
  role    = "roles/cloudfunctions.developer"
  # service account in project named "Cloud Build Service Account"
  member  = local.cloud_build_sa

  depends_on = [google_project_iam_member.build_r0]
}

resource "google_project_iam_member" "build_r2" {
  project = var.project_id
  role    = "roles/iam.serviceAccountUser"
  # service account in project named "Cloud Build Service Account"
  member  = local.cloud_build_sa

  depends_on = [google_project_iam_member.build_r1]
}

// cloud build run
module "build_mail" {
  source           = "./components/build_run"
  name             = module.mail-run.name
  region           = var.region
  docker_file_path = "mail/mail/Dockerfile"
  source_repo      = module.repository.id
}

module "build_user" {
  source           = "./components/build_run"
  name             = module.user-run.name
  region           = var.region
  docker_file_path = "user/Dockerfile"
  source_repo      = module.repository.id
}