variable "service_name" {
  description = "The name of cloud run"
  type        = string
}
variable "region" {
  description = "The region of cloud run"
  type        = string
}
variable "service_env" {
  type        = map(string)
  description = "The environment variables for the service ex: {key = value}"
  default     = {
  }
}
variable "service_secrets" {
  type        = list(string)
  description = "The secrets for the service ex: {key}"
  default     = []
}
variable "service_account" {
  type        = string
  description = "The service account for the service"
  default     = ""
}
variable "cloudsql_instance" {
  type        = string
  description = "The cloudsql instance for the service"
  default     = ""
}