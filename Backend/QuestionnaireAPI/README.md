# Questionnaire API v2

A simplified, generic questionnaire API system designed to replace the old role-focused backend with external role integration.

## Overview

This API provides endpoints for managing questionnaires, assignments, responses, and users in a simplified system that integrates with external role management (like Active Directory).

## Key Features

- **External Role Integration**: Users are linked to Active Directory; roles stored as string arrays
- **Copy-Based Versioning**: Simple questionnaire copying for version management
- **Assignment-Driven Access**: Users access questionnaires through assignments, not direct permissions
- **Manager Permissions**: Special `IsManager` flag for administrative access
- **Soft Delete**: Questionnaires use soft delete with `IsDeleted` flag

## Architecture

### Core Entities

1. **User** (`/api/users`)
   - `Id` (Guid): Primary key
   - `ActiveDirectoryGuid` (Guid): Link to AD identity
   - `UserName` (string): AD username
   - `FullName` (string): Display name
   - `UserRoles` (string[]): Array of role names from external source
   - `IsManager` (bool): Special administrative permissions

2. **Questionnaire** (`/api/questionnaires`)
   - `Id` (Guid): Primary key
   - `Title` (string): Questionnaire name
   - `Description` (string): Optional description
   - `SchemaJson` (string): JSON definition of questions and structure
   - `Category` (string): Organization tag
   - `CreatedByUserId` (Guid): Creator reference
   - `CreatedAt`/`LastUpdatedAt` (DateTime): Audit timestamps
   - `Version` (QuestionnaireVersion): Copy tracking information
   - `IsDeleted` (bool): Soft delete flag

3. **Assignment** (`/api/assignments`)
   - `Id` (Guid): Primary key
   - `Title` (string): Assignment name
   - `QuestionnaireId` (Guid): Linked questionnaire
   - `StartDate`/`EndDate` (DateTime?): Optional time boundaries
   - `Participants` (User[]): Who can respond
   - `Viewers` (User[]): Who can view responses

4. **Response** (`/api/responses`)
   - `Id` (Guid): Primary key
   - `AssignmentId` (Guid): Linked assignment
   - `ParticipantId` (Guid): Responding user
   - `ResponseDataJson` (string): JSON answers
   - `CreatedAt` (DateTime): Started timestamp
   - `SubmittedAt` (DateTime?): Completion timestamp

## API Endpoints

### Users
- `GET /api/users` - List all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/by-ad-guid/{adGuid}` - Find user by AD GUID
- `GET /api/users/by-role/{role}` - Users with specific role
- `GET /api/users/managers` - All manager users
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Remove user

### Questionnaires
- `GET /api/questionnaires` - List active questionnaires (excluding deleted)
- `GET /api/questionnaires/{id}` - Get questionnaire details
- `POST /api/questionnaires` - Create new questionnaire
- `PUT /api/questionnaires/{id}` - Update questionnaire
- `DELETE /api/questionnaires/{id}` - Soft delete questionnaire
- `POST /api/questionnaires/{id}/copy` - Create copy for versioning

### Assignments
- `GET /api/assignments` - List all assignments
- `GET /api/assignments/{id}` - Get assignment with participants/viewers
- `POST /api/assignments` - Create new assignment
- `PUT /api/assignments/{id}` - Update assignment
- `DELETE /api/assignments/{id}` - Remove assignment

### Responses
- `GET /api/responses` - List all responses
- `GET /api/responses/{id}` - Get response details
- `GET /api/responses/assignment/{assignmentId}` - Responses for assignment
- `GET /api/responses/user/{userId}` - User's responses
- `POST /api/responses` - Create new response
- `PUT /api/responses/{id}` - Update response (save progress)
- `POST /api/responses/{id}/submit` - Submit final response
- `DELETE /api/responses/{id}` - Remove response

## Database Configuration

Update `appsettings.json` with your SQL Server connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuestionnaireSystemV2;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Running the API

1. **Build and Run**:
   ```bash
   dotnet run
   ```

2. **Access Swagger Documentation**:
   - Navigate to: `http://localhost:5293/swagger`
   - Interactive API documentation and testing interface

3. **Database Migration** (if needed):
   ```bash
   dotnet ef database update --project ../QuestionnaireDatabaseV2
   ```

## Design Principles

### External Role Integration
- Roles are managed externally (Active Directory, LDAP, etc.)
- API stores roles as string arrays on User entities
- No complex internal role/permission tables
- Manager flag provides special administrative access

### Copy-Based Versioning
- Simple "click copy" workflow for questionnaire versions
- `QuestionnaireVersion` tracks copy relationships
- No complex version numbering or branching
- Copies preserve original questionnaire reference

### Assignment-Driven Architecture
- Users don't have direct permissions on questionnaires
- Access controlled through Assignment participation
- Assignments define who can respond and who can view results
- Time-bounded assignments with optional start/end dates

### Simplified Data Model
- All entities use Guid primary keys for distributed systems
- Minimal required fields, extensive optional fields
- Soft delete patterns for audit trails
- JSON storage for flexible questionnaire schemas

## Migration from Old API

This new API replaces the old role-focused backend:

1. **Role Management**: Moved from complex internal tables to external integration
2. **Permissions**: Simplified from granular permissions to assignment-based access  
3. **Versioning**: Changed from complex version tracking to simple copy workflow
4. **Entity Structure**: Streamlined from many tables to 4 core entities

The new system is designed to be more maintainable and better suited for external integrations while providing the same core functionality.