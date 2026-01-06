namespace API.Interfaces
{
    public interface IActiveQuestionnaireService
    {
        Task<ActiveQuestionnaireKeysetPaginationResultAdmin> FetchActiveQuestionnaireBases(ActiveQuestionnaireKeysetPaginationRequestFull request);
        Task<QuestionnaireGroupResult> ActivateQuestionnaireGroup(ActivateQuestionnaireGroup request);
        Task<QuestionnaireGroupOffsetPaginationResult> FetchQuestionnaireGroupsWithOffsetPagination(QuestionnaireGroupOffsetPaginationRequest request);
        Task<QuestionnaireGroupResult?> GetQuestionnaireGroup(Guid groupId);
        Task<List<QuestionnaireGroupResult>> GetAllQuestionnaireGroups();
        Task<List<QuestionnaireGroupBasicResult>> GetAllQuestionnaireGroupsBasic();
        Task<List<QuestionnaireGroupBasicResult>> GetQuestionnaireGroupsBasicForTemplate(Guid templateId);
        Task<ActiveQuestionnaire> FetchActiveQuestionnaire(Guid id);
        Task<List<ActiveQuestionnaire>> ActivateTemplate(ActivateQuestionnaire request);
        Task<Guid?> GetOldestActiveQuestionnaireForUser(Guid id);
        Task SubmitAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission);
        Task<FullResponse> GetFullResponseAsync(Guid id);
        Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId);
        Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid? userId = null);
        Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateAsync(Guid studentid, Guid templateid);
        Task<List<FullStudentRespondsDate>> GetResponsesFromStudentAndTemplateWithDateAsync(Guid studentid, Guid templateid);

        Task<List<FullResponse>> GetResponsesFromTeacherAndStudentAndTemplateWithDateAsync(Guid studentid,Guid teacherid,Guid templateid);

        Task<SurveyResponseSummary> GetAnonymisedResponses(Guid templateId, List<Guid> users, List<Guid> groups);
        Task<StudentResultHistory?> GetResponseHistoryAsync(Guid studentId, Guid teacherId, Guid templateId);
        Task<IEnumerable<CompletedStudentDto>> GetCompletedStudentsByGroup(Guid activeQuestionnaireId);
    }
}
