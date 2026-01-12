# QuestionnaireDatabaseV2

A redesigned, generic questionnaire management database that supports flexible role-based assignments with configurable visibility rules.

## Overview

This database design replaces the rigid Student/Teacher model with a flexible system that can handle any organizational roles from Active Directory while supporting various assignment types including anonymous, peer, and mixed-role questionnaires.

## Key Features

### 🔧 **Flexible Role Management**
- AD integration with GUID mapping
- Support for both AD groups and custom application roles
- Many-to-many user-role relationships with assignment tracking

### 📋 **JSON-Based Questionnaires**
- Questionnaire schemas stored as JSON (SurveyJS-compatible)
- Built-in versioning system for schema evolution
- Response integrity maintained across questionnaire updates

### 🎯 **Assignment-Driven Architecture**
- Configurable participant and viewer assignments
- Support for anonymous, peer, and mixed-role assignment types
- Granular visibility controls (summary vs raw data access)

### 🔒 **Privacy & Security**
- Anonymous assignment support with identity protection
- Role-based viewing permissions
- Audit trails for all major operations

## Entity Relationships

```
Users ←→ UserRoles ←→ Roles
  ↓
Users → Questionnaires → QuestionnaireVersions
  ↓
Users → Assignments → AssignmentParticipants
  ↓                → AssignmentViewers  
Users → Responses
```

## Core Entities

### **User**
- AD integration via `ActiveDirectoryGuid`
- Tracks login and activity information
- Links to roles, assignments, and responses

### **Role** 
- Supports AD group mapping via `ActiveDirectoryGroupDN`
- System roles (Admin, Manager, Teacher, Student) + custom roles
- Controls default assignment viewing permissions

### **Questionnaire & QuestionnaireVersion**
- JSON schema storage with versioning
- Maintains response integrity across schema changes
- Status tracking (Draft, Published, Archived)

### **Assignment System**
- **Assignment**: Links questionnaire to participants/viewers
- **AssignmentParticipant**: Who must/can respond
- **AssignmentViewer**: Who can view results and at what level
- Configurable anonymity and visibility rules

### **Response**
- JSON answers storage linked to specific questionnaire versions
- Support for draft and final responses
- Metadata tracking (timing, IP, user agent)

## Assignment Types

### **Anonymous**
- Participant identities never shown in results
- Only aggregated data visible to viewers
- Participants can see their own responses

### **Peer**
- Participants can potentially see each other's responses
- Configured viewers have full raw access
- Used for team feedback scenarios

### **Mixed-Role** 
- Different roles have different visibility levels
- E.g., Teachers see student responses, Managers see summaries
- Highly configurable per assignment

### **Individual**
- Personal assessments or self-evaluations
- Limited visibility, typically only to participant and designated viewers

## Visibility Levels

- **No Access**: Cannot see assignment results
- **Summary Only**: Aggregated statistics and charts
- **Own Responses**: User's own answers only  
- **Raw Responses**: Individual participant answers (respects anonymity)
- **Full Access**: Raw responses with participant identities (non-anonymous only)

## Migration from V1

The new design replaces:
- Hardcoded `StudentModel`/`TeacherModel` → Flexible `User` + `UserRole`
- Fixed questionnaire structure → JSON schemas with versioning
- Rigid teacher-student assignments → Configurable assignment participants/viewers
- Binary response access → Granular visibility controls

## Getting Started

1. **Configure Connection**: Update connection string in `QuestionnaireDbContext.OnConfiguring()`
2. **Run Migration**: `dotnet ef database update`  
3. **Seed AD Roles**: Map AD groups to system roles in `Role.ActiveDirectoryGroupDN`
4. **Create Questionnaires**: Design JSON schemas (SurveyJS format recommended)
5. **Configure Assignments**: Set up participants, viewers, and visibility rules

## Database Schema Notes

- All JSON columns use `nvarchar(max)` for SQL Server compatibility
- Timestamps default to UTC via `GETUTCDATE()`
- Indexes optimized for common query patterns
- Check constraints ensure data integrity (e.g., AssignmentViewer user-or-role constraint)

This design provides the flexibility needed for any organizational structure while maintaining strong privacy controls and audit capabilities.