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

module "ai-run" {
  source          = "./components/run"
  service_name    = "ai"
  region          = var.region
  service_account = google_service_account.run.email
  service_secrets = ["OPENAI_KEY", "AZURE_PASSWORD"]

  depends_on = [module.cloud_run_runner.results]
}

module "web-run" {
  source          = "./components/run"
  service_name    = "web"
  region          = var.region
  service_account = google_service_account.run.email

  depends_on = [module.cloud_run_runner.results]
}

// init cloud function
resource "google_storage_bucket" "function-source" {
  name                        = "gcf-source-${data.google_project.project.number}"
  # Every bucket name must be globally unique
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

// load balancer (too expensive just stop it)
module "load_balancer" {
  source        = "./components/load_balancer"
  bucket        = "meal-order-static"
  bucket_prefix = "images"

  domain     = var.domain
  name       = "load-balancer"
  project_id = var.project_id
  region     = var.region

  default_service_name = "web"
  service_map_url      = {
    "storage" = "/api/storage",
    "mail"    = "/api/mail",
    "user"    = "/api/user",
    "order"   = "/api/order",
    "menu"    = "/api/menu",
    "ai"      = "/api/ai"
  }
  cloud_function_services = {
    "storage" = module.storage-function.name
  }
  cloud_run_services = {
    "mail"  = module.mail-run.name
    "user"  = module.user-run.name
    "order" = module.order-run.name
    "menu"  = module.menu-run.name
    "web"   = module.web-run.name
    "ai"    = module.ai-run.name
  }

  depends_on = [
    module.storage-function,
    module.mail-run,
    module.user-run,
    module.order-run,
    module.menu-run,
    module.web-run,
    module.ai-run
  ]
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

module "build_ai" {
  source           = "./components/build_run"
  name             = module.ai-run.name
  region           = var.region
  docker_file_path = "ai/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "ai"

  depends_on = [module.cloud_build_builder.results]
}

module "build_web" {
  source           = "./components/build_run"
  name             = module.web-run.name
  region           = var.region
  docker_file_path = "web/Dockerfile"
  source_repo      = module.repository.id
  source_path      = "web"

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

// pub/sub
module "user-mq" {
  source      = "./components/mq"
  push_path   = "/api/user/sync/user-create-event"
  service_url = module.user-run.url
  topic_name  = "user-create-topic"
}

module "mail-fail-mq" {
  source      = "./components/mq"
  push_path   = "/api/mail/event/fail-mail-event"
  service_url = module.mail-run.url
  topic_name  = "mail-fail-topic"
}

module "mail-mq" {
  source            = "./components/mq"
  push_path         = "/api/mail/event/send-mail-event"
  service_url       = module.mail-run.url
  topic_name        = "mail-create-topic"
  dead_letter_topic = module.mail-fail-mq.id
}

module "pubsub-iam" {
  source          = "./components/iam_setting"
  project_id      = var.project_id
  service_account = "serviceAccount:service-${data.google_project.project.number}@gcp-sa-pubsub.iam.gserviceaccount.com"
  roles           = [
    "roles/pubsub.publisher",
    "roles/pubsub.subscriber"
  ]

  depends_on = [google_service_account.run]
}

// cloud scheduler
module "menu-maintain-workflow" {
  source = "./components/workflow"
  file   = "menu-day.yaml"
  name   = "menu-maintain-workflow"
  region = var.region
}

module "menu-scheduler" {
  source        = "./components/scheduler"
  frequency     = "0 5 * * *"
  name          = "menu-scheduler"
  project_id    = var.project_id
  region        = var.region
  workflow_name = module.menu-maintain-workflow.name
}