import { Injectable, inject } from "@angular/core";
import { Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { ApiService } from "../../../core/services/api.service";
import { environment } from "../../../../environments/environment";
import { Result } from "../../../shared/models/result.model";
import {
  Template,
  TemplateBase,
  TemplateStatus,
} from "../../../shared/models/template.model";
import { Role, User } from "../../../shared/models/user.model";
import { HttpParams } from "@angular/common/http";
import {
  Attempt,
  AttemptAnswer,
  StudentResultHistory,
  TemplateBaseResponse,
  UserPaginationResult,
  AnswerInfo,
  AnswerDetails,
} from "../models/result-history.model";

@Injectable({
  providedIn: "root",
})
export class ResultHistoryService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiUrlTemplate = `${environment.apiUrl}/questionnaire-template`;
  private apiService = inject(ApiService);

  /**
   * Get result history for a specific student and template combination
   * @param studentId - The ID of the student
   * @param templateId - The ID of the questionnaire template
   * @returns Observable of StudentResultHistory
   */
  getStudentResultHistory(
    studentId: string,
    templateId: string,
  ): Observable<StudentResultHistory> {
    const params = new HttpParams()
      .set("studentId", studentId)
      .set("templateId", templateId);

    return this.apiService.get<StudentResultHistory>(
      `${this.apiUrl}/responseHistory`,
      params,
    );
  }

  // -------------------
  // SEARCH HELPERS
  // -------------------

  /**
   * Search for students related to the current teacher user.
   * Uses the new teacher-specific endpoint that only returns students
   * the teacher has worked with through active questionnaires.
   */
  searchStudentsRelatedToTeacher(
    studentUsernameQuery: string,
  ): Observable<User[]> {
    const params = new HttpParams().set(
      "studentUsernameQuery",
      studentUsernameQuery,
    );

    return this.apiService.get<User[]>(
      `${environment.apiUrl}/user/teacher/students/search`,
      params,
    );
  }

  /**
   * Get questionnaire template bases that both the teacher and specified student have completed.
   * Uses the new teacher-specific endpoint that filters templates to shared completions only.
   */
  getTemplateBasesAnsweredByStudent(
    studentId: string,
  ): Observable<{ templateBases: TemplateBase[] }> {
    return this.apiService
      .get<
        TemplateBase[]
      >(`${environment.apiUrl}/questionnaire-template/answeredbystudent/${studentId}`)
      .pipe(
        // Transform to match expected format
        map((templates: TemplateBase[]) => ({ templateBases: templates })),
      );
  }

  // Legacy search methods (kept for backward compatibility if needed elsewhere)
  searchTemplates(
    term: string,
    queryCursor?: string,
  ): Observable<TemplateBaseResponse> {
    let params = new HttpParams()
      .set("title", term)
      .set("pageSize", 5)
      .set("templateStatus", "Finalized");

    if (queryCursor) params = params.set("queryCursor", queryCursor);

    return this.apiService.get<TemplateBaseResponse>(
      `${environment.apiUrl}/questionnaire-template/`,
      params,
    );
  }

  searchUsers(
    term: string,
    role: "student" | "teacher",
    pageSize: number,
    sessionId?: string,
  ): Observable<UserPaginationResult> {
    const formattedRole = role.charAt(0).toUpperCase() + role.slice(1);

    let params = new HttpParams()
      .set("User", term)
      .set("Role", formattedRole)
      .set("PageSize", pageSize.toString());

    if (sessionId) params = params.set("SessionId", sessionId);

    return this.apiService.get<UserPaginationResult>(
      `${environment.apiUrl}/User`,
      params,
    );
  }

  /**
   * Get questionnaire data by active questionnaire ID
   * @param activeQuestionnaireId Active questionnaire GUID
   * @returns Observable with questionnaire data
   */
  getQuestionnaireDataByID(activeQuestionnaireId: string) {
    let url = `${environment.apiUrl}/active-questionnaire/${activeQuestionnaireId}`;
    return this.apiService.get<any>(url);
  }

  /**
   * Get questionnaire template options by template ID
   * @param templateId Template GUID
   * @returns Observable with template options
   */
  getQuestionnaireOptionsByID(templateId: string) {
    let apiUrlTemplate = `${environment.apiUrl}/questionnaire-template/${templateId}`;
    return this.apiService.get<any>(apiUrlTemplate);
  }

  /**
   * Get responses by student, teacher, and template IDs with dates
   * @param studentId Student GUID
   * @param teacherId Teacher GUID
   * @param templateId Template GUID
   * @returns Observable with responses
   */
  getResponsesByID(studentId: string, teacherId: string, templateId: string) {
    let url = `${environment.apiUrl}/active-questionnaire/${studentId},${teacherId},${templateId}/getresponsesfromteacherandstudentandtemplatewithdate`;
    return this.apiService.get<any>(url);
  }

 /**
   * Get questionnaire data by active questionnaire ID
   * @param activeQuestionnaireId Active questionnaire GUID
   * @returns Observable with questionnaire data
   */
  getQuestionnaireDataByID(activeQuestionnaireId: string) {
    let url = `${environment.apiUrl}/active-questionnaire/${activeQuestionnaireId}`;
    return this.apiService.get<any>(url);
  }

    /**
   * Get questionnaire template options by template ID
   * @param templateId Template GUID
   * @returns Observable with template options
   */
  getQuestionnaireOptionsByID(templateId: string) {
    let apiUrlTemplate = `${environment.apiUrl}/questionnaire-template/${templateId}`;
    return this.apiService.get<any>(apiUrlTemplate);
  }

    /**
   * Get responses by student, teacher, and template IDs with dates
   * @param studentId Student GUID
   * @param teacherId Teacher GUID
   * @param templateId Template GUID
   * @returns Observable with responses
   */
  getResponsesByID(studentId: string, teacherId: string, templateId: string) {
    let url = `${environment.apiUrl}/active-questionnaire/${studentId},${teacherId},${templateId}/getresponsesfromteacherandstudentandtemplatewithdate`;
    return this.apiService.get<any>(url);
  }




}

