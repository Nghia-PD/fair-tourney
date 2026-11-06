#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

QUEUE_URL=$(aws sqs get-queue-url --queue-name "$QUEUE" --query QueueUrl --output text)
QUEUE_ARN=$(aws sqs get-queue-attributes --queue-url "$QUEUE_URL" \
  --attribute-names QueueArn --query Attributes.QueueArn --output text)

cat > /tmp/bucket-notification.json <<EOF
{"QueueConfigurations":[{
  "Id":"screenshot-chunk-created",
  "QueueArn":"$QUEUE_ARN",
  "Events":["s3:ObjectCreated:*"],
  "Filter":{"Key":{"FilterRules":[{"Name":"prefix","Value":"$SCREENSHOT_PREFIX"}]}}
}]}
EOF
aws s3api put-bucket-notification-configuration --bucket "$BUCKET" \
  --notification-configuration file:///tmp/bucket-notification.json
