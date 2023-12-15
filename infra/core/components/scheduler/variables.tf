variable "name" {
  description = "scheduler name"
  type        = string
}
variable "frequency" {
  description = "scheduler feq str ex: 0 23 * * *"
  type        = string
}
variable "workflow_name" {
  description = "scheduler trigger workflow"
  type        = string
}
variable "project_id" {
  description = "project id"
  type        = string
}
variable "region" {
  description = "region"
  type        = string
}
variable "parameter" {
  description = "json string parameter"
  type        = string
  default     = "{}"
}