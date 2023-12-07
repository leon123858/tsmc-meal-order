provider "google" {
  project = var.project_id
  region  = var.region
}

// source code repository
module "repository" {
  source     = "./repository"
  name       = "meal-service"
  region     = var.region
  git_url    = var.git_url
  git_app_id = var.git_app_id
  git_token  = var.git_token
}

module "ai-run" {
  source       = "./run"
  service_name = "ai"
  region       = var.region
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
  # service accout in prject named "Cloud Build Service Account"
  member = local.cloud_build_sa
}

resource "google_project_iam_member" "build_r1" {
  project = var.project_id
  role    = "roles/cloudfunctions.developer"
  # service accout in prject named "Cloud Build Service Account"
  member = local.cloud_build_sa

  depends_on = [google_project_iam_member.build_r0]
}

resource "google_project_iam_member" "build_r2" {
  project = var.project_id
  role    = "roles/iam.serviceAccountUser"
  # service accout in prject named "Cloud Build Service Account"
  member = local.cloud_build_sa

  depends_on = [google_project_iam_member.build_r1]
}

// cloud build run
module "build_http" {
  source           = "./build_run"
  name             = module.ai-run.name
  region           = var.region
  docker_file_path = "ai/Dockerfile"
  source_repo      = module.repository.id
}
