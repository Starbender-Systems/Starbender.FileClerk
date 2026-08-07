#!/bin/sh
set -eu

test -w /tmp/nginx/proxy

curl --fail --silent --show-error http://127.0.0.1:5001/ >/dev/null
curl --fail --silent --show-error http://127.0.0.1:5002/health-status >/dev/null
curl --fail --silent --show-error http://127.0.0.1:5003/ >/dev/null
curl --fail --silent --show-error http://127.0.0.1:5103/health-status >/dev/null
curl --fail --silent --show-error http://127.0.0.1:5104/health-status >/dev/null
curl --fail --silent --show-error --header "Host: ${DEMO_PUBLIC_HOST:-localhost}" http://127.0.0.1:8080/health >/dev/null
