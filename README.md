# Spectrum OS Management Tool

A web application for managing user groups and permissions across different environments (QA, UAT, PROD).

## 🚀 How to Run

### Prerequisites
- **.NET 9 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js** (version 20+) - [Download here](https://nodejs.org/)

### 1. Start the Backend
```bash
cd backend-csharp/SpectrumManagement
dotnet run --urls "http://localhost:5142"
```

The API will be available at `http://localhost:5142`

### 2. Start the Frontend
Open a new terminal:
```bash
cd frontend-angular/spectrum-management
npm install
npm start
```

The application will be available at `http://localhost:4200`

### 3. Login
1. Visit `http://localhost:4200`
2. Select any demo user (e.g., Patrick Dugan - P1234567)
3. Choose your environment (QA, UAT, PROD)
4. Click "Sign In"

That's it! The application is now running locally.
