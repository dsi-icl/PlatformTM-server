#!/bin/bash
set -e
dotnet restore
rm -rf $(pwd)/publish/web
dotnet publish PlatformTM.API/project.json -c release -o /sln/publish/web
