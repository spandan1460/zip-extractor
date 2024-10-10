#!/bin/sh

echo "Available runtimes:"
dotnet --list-runtimes
echo
echo "Available SDKs:"
dotnet --list-sdks
echo
echo "Starting Tracking OCT Example"

DU_SPACE=$(du . -h | tail -n1)
echo "Space used by deployment ${DU_SPACE}"

set -x
cd /App/publish
chmod u+x oct-template

echo "On your local machine open: http://localhost:8000/ after starting the docker container"

./fbm-template --Logging:LogLevel:Default=Debug --urls http://0.0.0.0:8080 $*
set +x
echo "Exit code: $?"
