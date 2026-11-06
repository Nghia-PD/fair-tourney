#!/bin/sh
set -eu

. "$(dirname "$0")/resources.env"

aws s3api create-bucket --bucket "$BUCKET"
