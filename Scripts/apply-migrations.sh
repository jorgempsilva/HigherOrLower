#!/bin/bash
set -e
dotnet ef --version || echo "dotnet-ef is not installed."
echo "Starting migrations..."
cd /app/HigherOrLower || { echo "HigherOrLower directory not found"; exit 1; }
echo "Current directory: $(pwd)"
echo "Running migrations..."

dotnet ef database update --project /app/Infrastructure/ --startup-project /app/HigherOrLower/ --verbose || { echo "Migrations failed"; exit 1; }

echo "Building the project..."
dotnet build || { echo "Build failed. Check the errors above."; exit 1; }