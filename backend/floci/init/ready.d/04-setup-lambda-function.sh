#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

ZIP=/etc/floci/artifacts/functions.zip

if [ ! -f "$ZIP" ]; then
  # A failed hook shuts Floci down, so skip instead: uploads still work without the Lambda.
  echo "Skipping Lambda setup: $ZIP is missing. From backend/, run: dotnet lambda package --project-location src/Functions -o floci/artifacts/functions.zip" >&2
  exit 0
fi

if aws lambda get-function --function-name "$FUNCTION" >/dev/null 2>&1; then
  aws lambda update-function-code --function-name "$FUNCTION" --zip-file "fileb://$ZIP"
else
  ACCOUNT=$(aws sts get-caller-identity --query Account --output text)
  aws lambda create-function --function-name "$FUNCTION" \
    --runtime dotnet10 \
    --handler Functions::Functions.Screenshots.ProcessScreenshotChunkFunction::FunctionHandler \
    --role "arn:aws:iam::$ACCOUNT:role/lambda" \
    --memory-size 512 \
    --timeout "$LAMBDA_TIMEOUT" \
    --environment "Variables={BEDROCK_MODEL_ID=$BEDROCK_MODEL_ID}" \
    --zip-file "fileb://$ZIP"
fi
