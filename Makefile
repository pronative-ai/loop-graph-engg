.PHONY: build run clean restore test help

# Default target
all: build

# Build the project
build:
	dotnet build src/AksAgenticWorkflowConsole.csproj --configuration Release

# Build in Debug mode
build-debug:
	dotnet build src/AksAgenticWorkflowConsole.csproj --configuration Debug

# Run the application
run:
	dotnet run --project src/AksAgenticWorkflowConsole.csproj

# Run in Release mode
run-release:
	dotnet run --project src/AksAgenticWorkflowConsole.csproj --configuration Release

# Restore packages
restore:
	dotnet restore src/AksAgenticWorkflowConsole.csproj

# Clean build artifacts
clean:
	dotnet clean src/AksAgenticWorkflowConsole.csproj --configuration Release
	dotnet clean src/AksAgenticWorkflowConsole.csproj --configuration Debug
	rm -rf src/bin src/obj

# Run tests (when test project is added)
test:
	dotnet test

# Format code
format:
	dotnet format src/AksAgenticWorkflowConsole.csproj

# Publish for deployment
publish:
	dotnet publish src/AksAgenticWorkflowConsole.csproj --configuration Release --output ./publish

# Show help
help:
	@echo "Available targets:"
	@echo "  build        - Build the project in Release mode"
	@echo "  build-debug  - Build the project in Debug mode"
	@echo "  run          - Run the application"
	@echo "  run-release  - Run in Release mode"
	@echo "  restore      - Restore NuGet packages"
	@echo "  clean        - Clean build artifacts"
	@echo "  test         - Run tests"
	@echo "  format       - Format code"
	@echo "  publish      - Publish for deployment"
	@echo "  help         - Show this help message"
