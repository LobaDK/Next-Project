# Questionnaire project

## Project Overview
A questionnaire application for students and teachers where users are assigned to answer questionnaires and teachers can compare results.

## Tech Stack
- **Frontend**: Angular 21, Angular Material, TailwindCSS
- **Backend**: ASP.NET Core Web API (.NET 8.0)
- **Database**: SQL Server with Entity Framework Core 9.0
- **Logging**: Serilog with custom database logger
- **Documentation**: DocFX
- **Testing**: xUnit (UnitTests project)

## Running the Application


### Backend Setup
1. Navigate to the `Backend/API` directory
2. Restore packages: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the application: `dotnet run`

> **Note**: The database will be created and migrations applied automatically on first run. Update `config.json` as needed before running.

### Backend Configuration
On first run, the application generates a `config.json` file in the `Backend/API` directory with default settings. Edit this file to match your environment:
- **ConnectionString**: Update database connection details (SQL Server location, credentials)
- **JWT**: Set secure access and refresh token secrets (minimum 32 characters)
- **LDAP**: Configure LDAP server settings for authentication

For detailed configuration options, see [docs/markdown/backend/getting-started.md](docs/markdown/backend/getting-started.md).

### Frontend
1. Navigate to the angular-src directory
2. Install dependencies: `npm install`
3. Start the development server: `ng serve`
4. Open browser to `http://localhost:4200`

## Documentation
Comprehensive API documentation is available using DocFX. The documentation is located in the `docs/` directory and includes detailed information about API endpoints, controllers, DTOs, and project architecture. To view the documentation, navigate to `docs/_site/index.html` after building with DocFX.