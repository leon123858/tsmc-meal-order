resource "google_workflows_workflow" "default" {
  name   = var.name
  region = var.region

  source_contents = file("${path.module}/${var.file}")
}