#!/bin/bash

echo "🚀 Setting up Charter Access OS Management Tool..."

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install .NET 9.0 SDK first."
    echo "Download from: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18+ first."
    echo "Download from: https://nodejs.org/"
    exit 1
fi

# Check if Angular CLI is installed
if ! command -v ng &> /dev/null; then
    echo "📦 Installing Angular CLI..."
    npm install -g @angular/cli
fi

echo "📦 Installing backend dependencies..."
cd backend-csharp/SpectrumManagement
dotnet restore

echo "📦 Installing frontend dependencies..."
cd ../../frontend-angular/spectrum-management
npm install

echo "✅ Setup complete!"
echo ""
echo "🚀 To run the application:"
echo "1. Start backend: cd backend-csharp/SpectrumManagement && dotnet run"
echo "2. Start frontend: cd frontend-angular/spectrum-management && ng serve"
echo ""
echo "🌐 Access the application at: http://localhost:4200"
echo "🔗 Backend API at: http://localhost:5142" 