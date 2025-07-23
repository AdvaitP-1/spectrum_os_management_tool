# Charter Access OS Management Tool

A comprehensive user and group management system built with .NET Core backend and Angular frontend.

## Features

- User authentication and management
- Group-based access control
- Environment-specific permissions (QA/UAT)
- Real-time dashboard with user and group management
- Permission-based access control system

## Prerequisites

- .NET 9.0 SDK
- Node.js 18+ and npm
- Angular CLI

## Quick Start

### Backend Setup
```bash
cd backend-csharp/SpectrumManagement
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5142`

### Frontend Setup
```bash
cd frontend-angular/spectrum-management
npm install
ng serve
```

The application will be available at `http://localhost:4200`

## Windows Troubleshooting

If you encounter authentication issues on Windows, try these solutions:

### 1. Use Windows-Specific Profile
```bash
cd backend-csharp/SpectrumManagement
dotnet run --launch-profile Windows
```

### 2. Check Port Availability
Ensure ports 5142 (backend) and 4200 (frontend) are not in use:
```bash
netstat -ano | findstr :5142
netstat -ano | findstr :4200
```

### 3. Windows Firewall
Allow the applications through Windows Firewall:
- .NET Core application
- Node.js/ng serve

### 4. Alternative Ports
If default ports are blocked, use:
```bash
# Backend on different port
dotnet run --urls "http://localhost:5000"

# Frontend on different port
ng serve --port 4201
```

### 5. CORS Issues
If you see CORS errors, the backend now supports:
- localhost and 127.0.0.1 variations
- Ports 4200, 4201, and 3000

### 6. Database Issues
The application uses in-memory database by default. If you encounter database errors:
- Ensure you have write permissions in the application directory
- Try running as administrator if needed

## API Endpoints

- `GET /api/users` - Get all users
- `POST /api/users/login` - User authentication
- `GET /api/groups` - Get groups by environment
- `POST /api/groups` - Create new group

## Demo Users

The system comes with pre-configured demo users:
- Patrick Dugan (P1234567)
- Kent Herbst (P2345678)
- Sion Pixley (P3456789)
- And more...

## Environment Support

- **QA Environment**: Development and testing access
- **UAT Environment**: User acceptance testing access
- Cross-environment user management

## License

This project is for demonstration purposes.
