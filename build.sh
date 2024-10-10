#!/bin/sh

# Build each component individually
docker rmi -f oct:v1.0.0
docker build -t oct:v1.0.0 .
if [[ $? -eq 0 ]]; then
	./run-local.sh
	docker image list
fi
