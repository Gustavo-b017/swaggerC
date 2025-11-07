#!/usr/bin/env sh
set -e
# Railway expõe a porta via $PORT
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"
exec dotnet BurgerApi.dll
