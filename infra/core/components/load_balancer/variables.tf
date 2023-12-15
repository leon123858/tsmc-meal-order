variable "project_id" {
  description = "The project ID to deploy to"
  type        = string
}
variable "name" {
  description = "The name of the lb"
  type        = string
}
variable "domain" {
  description = "The domain url mapping to the lb"
  type        = string
}
variable "region" {
  description = "The region to deploy to"
  type        = string
}
variable "bucket" {
  description = "The bucket to save static files to"
  type        = string
}
variable "cloud_run_services" {
  description = "The cloud run services to map to the lb"
  type        = map(string)
  #  default     = {}
}
variable "cloud_function_services" {
  description = "The cloud function services to map to the lb"
  type        = map(string)
  #  default     = {}
}
variable "default_service_name" {
  description = "The default service name to url map to the lb"
  type        = string
  #  default     = "web"
}
variable "service_map_url" {
  description = "The url to map to the service map"
  type        = map(string)
  #    default     = {
  #        "user" = "/api/user/*",
  #    }
}
variable "bucket_prefix" {
  description = "The prefix to add to the bucket"
  type        = string
  #  default     = "images"
}