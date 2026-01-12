# QuestionnaireDatabaseV2 Entity Documentation

## Overview

This database design provides a streamlined, flexible questionnaire system that integrates with external user management (like Active Directory) while maintaining simplicity and SRS compliance. The system is designed around the principle that **all users are equal, except for Managers** who have special permissions.

## Architecture Principles

- **External Role Integration**: User roles come from external sources (AD groups, etc.)
- **Manager-Centric Permissions**: Only Managers have special privileges; other roles are primarily for organization/search
- **Copy-Based Versioning**: Simple questionnaire versioning through copying rather than complex version management
- **Assignment-Driven Visibility**: Users see questionnaires through assignments, not direct permissions
- **Soft Delete Pattern**: All entities support soft deletion for audit trails

## Core Entities

### 1. User Entity
**Purpose**: Represents system users with external role integration

```csharp
public class User
{
    public Guid Id { get; set; }                    // Primary key
    public Guid ActiveDirectoryGuid { get; set; }   // Link to AD identity
    public string UserName { get; set; }            // From AD
    public string DisplayName { get; set; }         // From AD
    public string Email { get; set; }               // From AD
    public string UserRoles { get; set; }           // JSON array of role names from external source
    public bool IsManager { get; set; }             // Calculated by API based on role configuration
    public bool IsDeleted { get; set; }             // Soft delete flag
    // Timestamps...
}
```

**Key Concepts**:
- **UserRoles**: JSON array storing roles from external sources (e.g., `["Student", "ClassRepresentative"]`)
- **IsManager**: Set by API when user's external roles match configured manager groups
- **External Integration**: ActiveDirectoryGuid links to external identity system

**Usage Example**:
```csharp
// API determines manager status
user.IsManager = user.UserRoles.Contains("Domain-Teachers") || user.UserRoles.Contains("School-Administrators");
```

### 2. Questionnaire Entity
**Purpose**: Defines questionnaire templates that can be assigned to users

```csharp
public class Questionnaire
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public QuestionnaireStatus Status { get; set; }  // Draft, Published, Archived
    public string Category { get; set; }             // For organization
    public Guid CreatedByUserId { get; set; }
    public string SchemaJson { get; set; }           // SurveyJS-compatible schema
    public QuestionnaireVersion Version { get; set; } // Copy tracking
    public bool IsDeleted { get; set; }
    // Timestamps and navigation properties...
}

public class QuestionnaireVersion
{
    public Guid? CopiedFromQuestionnaireId { get; set; }
    public string? CopiedFromTitle { get; set; }
}
```

**Key Concepts**:
- **Copy-Based Versioning**: When users need to update published questionnaires, they "copy" to create a new version
- **Schema Storage**: Questions and structure stored as JSON for maximum flexibility
- **Status Workflow**: Draft → Published → Archived
- **Category Organization**: Helps with searching and organization

**Usage Example**:
```csharp
// Creating a copy of an existing questionnaire
var newQuestionnaire = new Questionnaire
{
    Title = "Updated Survey",
    SchemaJson = originalQuestionnaire.SchemaJson,
    Version = new QuestionnaireVersion 
    {
        CopiedFromQuestionnaireId = originalQuestionnaire.Id,
        CopiedFromTitle = originalQuestionnaire.Title
    }
};
```

### 3. Assignment Entity
**Purpose**: Connects questionnaires to participants and defines who can view results

```csharp
public class Assignment
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid QuestionnaireId { get; set; }
    public AssignmentType Type { get; set; }      // Survey, Evaluation, Feedback
    public AssignmentStatus Status { get; set; }  // Draft, Active, Closed
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    // Collections: Participants, Viewers, Responses...
}
```

**Key Concepts**:
- **Assignment-Driven Access**: Users access questionnaires through assignments, not direct permissions
- **Flexible Participation**: Can assign to individuals or groups
- **Result Visibility Control**: Separate configuration for who can view results
- **Time-Bounded**: Optional start/end dates for controlled access

### 4. AssignmentParticipant Entity
**Purpose**: Defines who should complete the questionnaire

```csharp
public class AssignmentParticipant
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid UserId { get; set; }
    public bool IsRequired { get; set; }      // Mandatory vs optional participation
    public DateTime AddedAt { get; set; }
    public Guid? AddedByUserId { get; set; }
}
```

**Usage Patterns**:
```csharp
// Add all students to a survey
foreach (var student in studentsInClass)
{
    assignment.Participants.Add(new AssignmentParticipant
    {
        UserId = student.Id,
        IsRequired = true,
        AddedByUserId = teacherId
    });
}
```

### 5. AssignmentViewer Entity
**Purpose**: Controls who can view assignment results

```csharp
public class AssignmentViewer
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid UserId { get; set; }              // Individual user access
    public bool CanViewResponses { get; set; }    // See individual responses
    public bool CanViewIdentities { get; set; }   // See participant names
    public bool IsDeleted { get; set; }
}
```

**Permission Levels**:
- **CanViewResponses**: Access to response data
- **CanViewIdentities**: Can see who submitted what (privacy control)

### 6. Response Entity
**Purpose**: Stores participant responses to questionnaires

```csharp
public class Response
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid QuestionnaireId { get; set; }     // Maintains integrity
    public string AnswersJson { get; set; }      // Actual response data
    public bool IsCompleted { get; set; }        // Draft vs final
    public DateTime CreatedAt { get; set; }
    public DateTime SubmittedAt { get; set; }
}
```

**Key Features**:
- **Questionnaire Link**: Maintains reference to questionnaire version used
- **Draft Support**: Partial responses can be saved and completed later
- **JSON Storage**: Flexible answer format matching questionnaire schema

## Workflow Examples

### Creating and Assigning a Survey

```csharp
// 1. Manager creates questionnaire
var questionnaire = new Questionnaire
{
    Title = "Student Satisfaction Survey",
    SchemaJson = surveyDefinition,
    Status = QuestionnaireStatus.Draft,
    CreatedByUserId = managerId
};

// 2. Create assignment
var assignment = new Assignment
{
    Title = "Fall 2024 Satisfaction Survey",
    QuestionnaireId = questionnaire.Id,
    Type = AssignmentType.Survey,
    Status = AssignmentStatus.Active,
    StartDate = DateTime.UtcNow,
    EndDate = DateTime.UtcNow.AddDays(14),
    CreatedByUserId = managerId
};

// 3. Add participants (all students in a class)
foreach (var student in classStudents)
{
    assignment.Participants.Add(new AssignmentParticipant
    {
        UserId = student.Id,
        IsRequired = false
    });
}

// 4. Configure viewers (teachers can see results)
foreach (var teacher in classTeachers)
{
    assignment.Viewers.Add(new AssignmentViewer
    {
        UserId = teacher.Id,
        CanViewResponses = true,
        CanViewIdentities = false  // Anonymous viewing
    });
}
```

### Role Management Integration

```csharp
// API login flow - sync roles from external source
public async Task SyncUserRoles(User user, string[] externalGroups)
{
    // Store external roles as JSON
    user.UserRoles = JsonSerializer.Serialize(externalGroups);
    
    // Determine manager status based on configuration
    var managerGroups = configuration.GetSection("ManagerRoles").Get<string[]>();
    user.IsManager = externalGroups.Any(group => managerGroups.Contains(group));
    
    await dbContext.SaveChangesAsync();
}
```

### Questionnaire Copying

```csharp
// User wants to modify a published questionnaire
public async Task<Questionnaire> CopyQuestionnaire(Guid originalId, string newTitle)
{
    var original = await dbContext.Questionnaires.FindAsync(originalId);
    
    var copy = new Questionnaire
    {
        Title = newTitle,
        SchemaJson = original.SchemaJson,
        Status = QuestionnaireStatus.Draft,
        Version = new QuestionnaireVersion
        {
            CopiedFromQuestionnaireId = original.Id,
            CopiedFromTitle = original.Title
        },
        CreatedByUserId = currentUserId
    };
    
    return copy;
}
```

## Database Design Benefits

### 1. **Simplified Role Management**
- No complex role tables or many-to-many relationships
- External roles stored as simple JSON on user
- Manager status computed by business logic

### 2. **Flexible Questionnaire System**
- JSON schema storage supports any question type
- Copy-based versioning is intuitive and safe
- No complex version management

### 3. **Assignment-Driven Architecture**
- Clean separation between questionnaire definition and usage
- Flexible participation and viewing controls
- Supports various use cases (surveys, evaluations, feedback)

### 4. **Audit and Compliance**
- Soft delete preserves data integrity
- Timestamp tracking on all entities
- Copy lineage tracking for questionnaires

### 5. **Performance Optimized**
- Streamlined entities with minimal overhead
- Strategic indexes for common queries
- Reduced complexity compared to over-engineered alternatives

## Configuration Requirements

### Application Settings
```json
{
  "ManagerRoles": [
    "CN=Teachers,OU=Groups,DC=school,DC=edu",
    "CN=Administrators,OU=Groups,DC=school,DC=edu"
  ],
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection"
  }
}
```

This design provides a robust, scalable foundation for questionnaire management while maintaining simplicity and focusing on real-world usage patterns.