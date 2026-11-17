#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

ACCOUNT=$(aws sts get-caller-identity --query Account --output text)
QUEUE_URL=$(aws sqs create-queue --queue-name "$QUEUE" --query QueueUrl --output text)
QUEUE_ARN=$(aws sqs get-queue-attributes --queue-url "$QUEUE_URL" \
  --attribute-names QueueArn --query Attributes.QueueArn --output text)

# Lets S3 send to this queue. The bucket ARN is only a string here, so the bucket need not exist yet.
cat > /tmp/queue-attributes.json <<EOF
{"VisibilityTimeout":"$VISIBILITY_TIMEOUT","Policy":"{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{\"Service\":\"s3.amazonaws.com\"},\"Action\":\"sqs:SendMessage\",\"Resource\":\"$QUEUE_ARN\",\"Condition\":{\"ArnEquals\":{\"aws:SourceArn\":\"arn:aws:s3:::$BUCKET\"},\"StringEquals\":{\"aws:SourceAccount\":\"$ACCOUNT\"}}}]}"}
EOF
aws sqs set-queue-attributes --queue-url "$QUEUE_URL" --attributes file:///tmp/queue-attributes.json
