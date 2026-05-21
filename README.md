# DTS Developer Technical Test

## Overview
This repository contains a completed task management solution for the HMCTS caseworker scenario.

The application includes:
- A .NET backend API for creating, retrieving, updating, and deleting tasks
- A React frontend for viewing and managing tasks
- A SQLite database for persistence
- Unit tests for backend controller and service behaviour

## What Was Delivered

### Backend
The backend API supports:
- Create task
- Get all tasks
- Get task by ID
- Update task status
- Update full task details
- Delete task

The backend implementation also includes:
- Entity Framework Core with SQLite
- Validation and error handling
- OpenAPI support in development
- A service layer and interface layer using dependency injection
- Unit tests for service and controller responsibilities

### Frontend
The frontend provides:
- Task list view using MUI DataGrid
- Create and edit task modal
- Delete task flow
- Status chips for task states
- MUI date picker using UK date format

The frontend has also been refactored so that:
- API calls live under `src/api`
- Reusable UI lives under `src/components`
- Shared task types live under `src/types`
- Shared date helpers live under `src/utils`

## Tech Stack
- Backend: ASP.NET Core, Entity Framework Core, SQLite
- Frontend: React, TypeScript, Vite, MUI
- Testing: xUnit, Moq, EF Core InMemory

## Running The Application

### Prerequisites
- .NET 10 SDK
- Node.js and npm

### Run The Backend
From the repository root:

```bash
dotnet run --project backend/TaskApi/TaskApi.csproj
```

The backend runs in development on:
- `http://localhost:5090`

The SQLite database is stored at:
- `backend/TaskApi/tasks.db`

### Run The Frontend
In a separate terminal:

```bash
cd frontend/task-ui
npm install
npm run dev
```

The frontend runs through Vite. The development server proxies `/api` requests to:
- `http://localhost:5090`

Once started, open the local Vite URL shown in the terminal, typically:
- `http://localhost:5173`

## Running Unit Tests

### Backend Tests
From the repository root:

```bash
dotnet test backend/TaskApi.Tests/TaskApi.Tests.csproj
```

This runs:
- Controller tests
- Service tests

## Notes
- The backend creates the SQLite database automatically if it does not already exist.
- The frontend expects the backend to be running on port `5090` during local development.
