variable "topic_name" {
  description = "The name of the topic"
  type        = string
}
variable "service_url" {
  description = "The url of the service"
  type        = string
}
variable "push_path" {
  description = "The path of the service"
  type        = string
}
variable "dead_letter_topic" {
  description = "The dead letter topic id"
  type        = string
  default     = ""
}