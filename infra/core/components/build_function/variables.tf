variable "service_name" {
  description = "The name of the service"
  type        = string
}
variable "region" {
  description = "The region to deploy to"
  type        = string
}
variable "source_repo" {
  description = "The source repo to deploy from"
  type        = string
}
variable "entry_point" {
  description = "The entry point for the service"
  type        = string
}
variable "function_path" {
  description = "The path to the function"
  type        = string
}