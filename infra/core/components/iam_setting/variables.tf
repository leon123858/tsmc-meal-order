variable "project_id" {
  description = "value of the project id"
  type        = string
}
variable "service_account" {
  description = "value of the email of the service account"
  type        = string
}
variable "roles" {
  description = "value of the roles"
  type        = list(string)
}
