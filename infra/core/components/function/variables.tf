variable "service_name" {
  description = "The name of the service"
  type        = string
}
variable "source_bucket" {
  description = "The name of the source bucket"
  type        = string
}
variable "region" {
  description = "The region to deploy to"
  type        = string
}
variable "service_account_email" {
  description = "The email of the service account"
  type        = string
}