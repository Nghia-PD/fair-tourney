#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

ACCOUNT=$(aws sts get-caller-identity --query Account --output text)
QUEUE_URL=$(aws sqs create-queue --queue-name "$QUEUE" --query QueueUrl --output text)
QUEUE_ARN=$(aws sqs get-queue-attributes --queue-url "$QUEUE_URL" \
  --attribute-names QueueArn --query Attributes.QueueArn --output text)

cat > /tmp/queue-attributes.json <<EOF
{"Policy":"{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{\"Service\":\"s3.amazonaws.com\"},\"Action\":\"sqs:SendMessage\",\"Resource\":\"$QUEUE_ARN\",\"Condition\":{\"ArnEquals\":{\"aws:SourceArn\":\"arn:aws:s3:::$BUCKET\"},\"StringEquals\":{\"aws:SourceAccount\":\"$ACCOUNT\"}}}]}"}
EOF
aws sqs set-queue-attributes --queue-url "$QUEUE_URL" --attributes file:///tmp/queue-attributes.json

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
