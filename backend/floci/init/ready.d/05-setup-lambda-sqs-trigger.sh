#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

if ! aws lambda get-function --function-name "$FUNCTION" >/dev/null 2>&1; then
  echo "Skipping SQS trigger: $FUNCTION does not exist." >&2
  exit 0
fi

EXISTING=$(aws lambda list-event-source-mappings --function-name "$FUNCTION" \
  --query 'length(EventSourceMappings)' --output text)
if [ "$EXISTING" != "0" ]; then
  exit 0
fi

QUEUE_URL=$(aws sqs get-queue-url --queue-name "$QUEUE" --query QueueUrl --output text)
QUEUE_ARN=$(aws sqs get-queue-attributes --queue-url "$QUEUE_URL" \
  --attribute-names QueueArn --query Attributes.QueueArn --output text)

aws lambda create-event-source-mapping --function-name "$FUNCTION" \
  --event-source-arn "$QUEUE_ARN" --batch-size 1
