resource "google_project_iam_member" "build" {
  count   = length(var.roles)
  project = var.project_id
  role    = var.roles[count.index]
  member  = var.service_account
}