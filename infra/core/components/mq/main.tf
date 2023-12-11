resource "google_pubsub_topic" "topic" {
  name = var.topic_name
}
resource "google_pubsub_subscription" "sub" {
  name  = "${var.topic_name}-subscription"
  topic = google_pubsub_topic.topic.name

  ack_deadline_seconds       = 20
  message_retention_duration = "600s"

  retry_policy {
    minimum_backoff = "10s"
    maximum_backoff = "600s"
  }

  push_config {
    push_endpoint = "${var.service_url}${var.push_path}"

    attributes = {
      x-goog-version = "v1"
    }
  }

  dynamic "dead_letter_policy" {
    for_each = var.dead_letter_topic != "" ? { topic : var.dead_letter_topic } : {}
    content {
      dead_letter_topic     = dead_letter_policy.value
      max_delivery_attempts = 5
    }
  }
}