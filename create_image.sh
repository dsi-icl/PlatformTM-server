#!/bin/bash
set -x
# Compile the project
docker run -it --rm -v $(pwd):/sln --workdir /sln microsoft/aspnetcore-build:1.1-projectjson sh build.sh
# Create the image
docker build  -t platform-tm/platform-tm-server ./publish/web/

# Create the db-init image
docker build -t platform-tm/db-init -f Dockerfile.db-init .
