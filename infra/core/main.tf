provider "google" {
  project = var.project_id
  region  = var.region
}

data "google_project" "project" {
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

// cloud run set iam
resource "google_service_account" "run" {
  account_id   = "cloud-run-sa"
  display_name = "Service Account For Cloud Run"
  project      = var.project_id
}

module "cloud_run_runner" {
  source          = "./components/iam_setting"
  project_id      = var.project_id
  service_account = "serviceAccount:${google_service_account.run.email}"
  roles           = [
    "roles/cloudsql.admin",
    "roles/firebase.sdkAdminServiceAgent",
    "roles/pubsub.editor",
    "roles/secretmanager.secretAccessor",
    "roles/storage.admin"
  ]

  depends_on = [google_service_account.run]
}

// init cloud run
module "mail-run" {
  source       = "./components/run"
  service_name = "mail"
  region       = var.region
  service_env  = {
    ASPNETCORE_ENVIRONMENT = "Production",
  }
  service_secrets   = ["POSTGRES_PASSWORD", "SMTP_PASSWORD"]
  service_account   = google_service_account.run.email
  cloudsql_instance = "tw-rd-ca-leon-lin:asia-east1:meal-dev"

  depends_on = [module.cloud_run_runner.results]
}

module "user-run" {
  source          = "./components/run"
  service_name    = "user"
  region          = var.region
  service_account = google_service_account.run.email

  depends_on = [module.cloud_run_runner.results]
}

module "order-run" {
  source          = "./components/run"
  service_name    = "order"
  region          = var.region
  service_account = google_service_account.run.email
  service_env     = {
    ASPNETCORE_ENVIRONMENT = "Production",
  }
  service_secrets = ["SQL_PASSWORD"]
  depends_on      = [module.cloud_run_runner.results]
}

module "menu-run" {
  source          = "./components/run"
  service_name    = "menu"
  region          = var.region
  service_account = google_service_account.run.email
  service_env     = {
    ASPNETCORE_ENVIRONMENT = "Production",
  }
  service_secrets = ["MONGO_PASSWORD"]
  depends_on      = [module.cloud_run_runner.results]
}

// init cloud function
resource "google_storage_bucket" "function-source" {
  name                        = "gcf-source-${data.google_project.project.number}" # Every bucket name must be globally unique
  location                    = "ASIA"
  uniform_bucket_level_access = true
}

module "storage-function" {
  source                = "./components/function"
  service_name          = "storage"
  region                = var.region
  service_account_email = google_service_account.run.email
  source_bucket         = google_storage_bucket.function-source.name

  depends_on = [module.cloud_run_runner.results]
}

// cloud build set iam
module "cloud_build_builder" {
  source          = "./components/iam_setting"
  project_id      = var.project_id
  # service account in project named "Cloud Build Service Account"
  service_account = "serviceAccount:${data.google_project.project.number}@cloudbuild.gserviceaccount.com"
  roles           = [
    "roles/run.admin",
    "roles/cloudfunctions.developer",
    "roles/iam.serviceAccountUser",
  ]
}

// cloud build run
module "build_mail" {
  source           = "./components/build_run"
  name             = module.mail-run.name
  region           = var.region
  docker_file_path = "mail/mail/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "mail"

  depends_on = [module.cloud_build_builder.results]
}

module "build_user" {
  source           = "./components/build_run"
  name             = module.user-run.name
  region           = var.region
  docker_file_path = "user/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "user"

  depends_on = [module.cloud_build_builder.results]
}

module "build_order" {
  source           = "./components/build_run"
  name             = module.order-run.name
  region           = var.region
  docker_file_path = "core/order/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "core"

  depends_on = [module.cloud_build_builder.results]
}

module "build_menu" {
  source           = "./components/build_run"
  name             = module.menu-run.name
  region           = var.region
  docker_file_path = "core/menu/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "core"

  depends_on = [module.cloud_build_builder.results]
}

// cloud build function
module "build_storage" {
  source        = "./components/build_function"
  entry_point   = "storage"
  function_path = "./functions/storage"
  region        = var.region
  service_name  = module.storage-function.name
  source_repo   = module.repository.id

  depends_on = [module.cloud_build_builder.results]
}