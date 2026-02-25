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
### Backend
1. Navigate to the `Backend/API` directory
2. Restore dependencies: `dotnet restore`
3. Update database (run EF Core migrations): `dotnet ef database update`
4. Run the API: `dotnet run`
5. The API will be available at `https://localhost:5001` or `http://localhost:5000`

### Frontend
1. Navigate to the angular-src directory
2. Install dependencies: `npm install`
3. Start the development server: `ng serve`
4. Open browser to `http://localhost:4200`

## Documentation
Comprehensive API documentation is available using DocFX. The documentation is located in the `docs/` directory and includes detailed information about API endpoints, controllers, DTOs, and project architecture. To view the documentation, navigate to `docs/_site/index.html` after building with DocFX.