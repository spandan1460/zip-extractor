#!/bin/sh

docker run --rm -it --env ASPNETCORE_ENVIRONMENT=Development --hostname oct --name oct -p 8000:8080 oct:v1.0.0
