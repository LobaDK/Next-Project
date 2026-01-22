namespace QuestionnaireDatabaseV2.Enums;

[Flags]
public enum UserPermissions
{
    // Base permission (no permissions)
    None = 0,

    // Questionnaire Response Permissions
    ViewAssignedQuestionnaires = 1 << 0,      // Can view questionnaires assigned to them
    RespondToQuestionnaires = 1 << 1,         // Can submit responses to questionnaires
    ViewOwnResults = 1 << 2,                  // Can view their own questionnaire results

    // Teacher Permissions (includes base student permissions)
    ViewStudentList = 1 << 3,                 // Can view list of students
    ViewStudentResults = 1 << 4,              // Can view individual student results
    ViewResultsOverview = 1 << 5,             // Can view aggregated/summary results

    // Questionnaire Template Management
    ViewQuestionnaireTemplates = 1 << 6,      // Can view questionnaire templates
    CreateQuestionnaires = 1 << 7,            // Can create new questionnaire templates
    EditQuestionnaires = 1 << 8,              // Can edit questionnaire templates
    DeleteQuestionnaires = 1 << 9,            // Can delete questionnaire templates

    // Assignment Management
    ViewAssignments = 1 << 10,                // Can view questionnaire assignments
    CreateAssignments = 1 << 11,              // Can create/assign questionnaires to users/groups
    EditAssignments = 1 << 12,                // Can edit existing assignments
    DeleteAssignments = 1 << 13,              // Can delete assignments

    // User & Role Management
    ViewUsers = 1 << 14,                      // Can view list of users
    CreateUsers = 1 << 15,                    // Can create new user accounts
    EditUsers = 1 << 16,                      // Can edit user details
    DeleteUsers = 1 << 17,                    // Can delete user accounts
    ManageUserRoles = 1 << 18,                // Can assign/modify user roles and permissions
    ViewUserPermissions = 1 << 19,            // Can view user permissions

    // Application Settings
    ViewSettings = 1 << 20,                   // Can view application settings
    EditSettings = 1 << 21,                   // Can modify application settings
    EditRateLimitSettings = 1 << 22,          // Can modify rate limiting configuration

    // Logging & Audit
    ViewLogs = 1 << 23,                       // Can view application logs
    ViewAuditTrail = 1 << 24,                 // Can view audit logs of user actions
    ExportLogs = 1 << 25,                     // Can export logs

    // System Administration
    ViewSystemStatus = 1 << 26,               // Can view system health/status
    ManageBackgroundServices = 1 << 27,       // Can manage background services

    // Composite permissions for convenience
    Student = ViewAssignedQuestionnaires | RespondToQuestionnaires | ViewOwnResults,
    
    Teacher = Student 
        | ViewStudentList 
        | ViewStudentResults 
        | ViewResultsOverview 
        | ViewQuestionnaireTemplates,

    Management = ViewQuestionnaireTemplates 
        | CreateQuestionnaires 
        | EditQuestionnaires 
        | DeleteQuestionnaires 
        | ViewAssignments 
        | CreateAssignments 
        | EditAssignments 
        | DeleteAssignments 
        | ViewUsers 
        | CreateUsers 
        | EditUsers 
        | DeleteUsers 
        | ManageUserRoles 
        | ViewUserPermissions 
        | ViewSettings 
        | EditSettings 
        | EditRateLimitSettings 
        | ViewLogs 
        | ViewAuditTrail 
        | ExportLogs 
        | ViewSystemStatus 
        | ManageBackgroundServices,
    
    Admin = Management
}