import { Observable } from 'rxjs';
import { Signal } from '@angular/core';
import { User } from '../../shared/models/user.model';
import { LoginResult } from '../../features/home/models/login.model';
import { AnswerSubmission, Questionnaire } from '../../features/questionnaire/models/answer.model';
import { Template } from '../../shared/models/template.model';
import { TemplateBaseResponse } from '../../features/template-manager/models/template.model';
import { ActiveQuestionnaire, ActiveQuestionnaireBase, QuestionnaireGroupOffsetPaginationResult, ResponseActiveQuestionnaireBase, TemplateBaseResponse as ActiveTemplateBaseResponse, UserPaginationResult } from '../../features/active-questionnaire-manager/models/active.models';
import { PaginationResponse } from '../../shared/models/Pagination.model';
import { Result } from '../../shared/models/result.model';
import { QuestionnaireGroupResponse } from '../../features/teacher-dashboard/models/dashboard.model';

/**
 * Interface defining the contract for authentication services.
 * Both real and mock AuthService implementations must implement this interface.
 */
export interface IAuthService {
  // Authentication methods
  login(userName: string, password: string): Observable<LoginResult>;
  logout(): void;
  refreshToken(): Observable<any>;
  initializeAuthState(): Promise<boolean>;
  
  // State signals - readonly access to authentication state
  readonly isAuthenticated: Signal<boolean>;
  readonly user: Signal<User | null>;
  readonly isOnline: Signal<boolean>;
}

/**
 * Interface defining the contract for home services.
 * Both real and mock HomeService implementations must implement this interface.
 */
export interface IHomeService {
  checkForExistingActiveQuestionnaires(): Observable<{ exists: boolean; id: string | null }>;
}

/**
 * Interface defining the contract for answer services.
 * Both real and mock AnswerService implementations must implement this interface.
 */
export interface IAnswerService {
  submitAnswers(id: string, answers: AnswerSubmission): Observable<void>;
  hasUserSubmited(id: string): Observable<boolean>;
  getActiveQuestionnaireById(instanceId: string): Observable<Questionnaire>;
}
export interface ITemplateService {
  getTemplateBases(
    pageSize: number,
    queryCursor?: string,
    searchTerm?: string,
    searchType?: 'name' | 'id'
  ): Observable<TemplateBaseResponse>;
  getTemplateDetails(id: string): Observable<Template>;
  addTemplate(template: Template): Observable<Template>;
  updateTemplate(templateId: string, updatedTemplate: Template): Observable<Template>;
  deleteTemplate(templateId: string): Observable<void>;
  upgradeTemplate(templateId: string): Observable<Template>;
  checkTitleAvailability(title: string): Observable<boolean>;
}

export interface IActiveService {
  testgetActiveQuestionnaires(
    page: number,
    pageSize: number,
    searchStudent?: string,
    searchStudentType?: 'fullName' | 'userName' | 'both',
    searchTeacher?: string,
    searchTeacherType?: 'fullName' | 'userName' | 'both'
  ): Observable<PaginationResponse<ActiveQuestionnaire>>;
  getActiveQuestionnaires(
    pageSize: number,
    queryCursor?: string,
    studentSearch?: string,
    studentSearchType?: 'fullName' | 'userName' | 'both',
    teacherSearch?: string,
    teacherSearchType?: 'fullName' | 'userName' | 'both'
  ): Observable<ResponseActiveQuestionnaireBase>;
  getQuestionnaireGroupsPaginated(
    pageNumber: number,
    pageSize: number,
    searchTitle?: string,
    filterPendingStudent?: boolean,
    filterPendingTeacher?: boolean
  ): Observable<QuestionnaireGroupOffsetPaginationResult>;
  createActiveQuestionnaire(aq: { studentIds: string[]; teacherIds: string[]; templateId: string }): Observable<ActiveQuestionnaire[]>;
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire>;
  searchUsers(
    term: string,
    role: 'student' | 'teacher',
    pageSize: number,
    sessionId?: string
  ): Observable<UserPaginationResult>;
  searchTemplates(term: string, queryCursor?: string): Observable<ActiveTemplateBaseResponse>;
  createActiveQuestionnaireGroup(aq: { name: string; templateId: string; studentIds: string[]; teacherIds: string[] }): Observable<any>;
  getQuestionnaireGroup(groupId: string): Observable<any>;
  getQuestionnaireGroups(): Observable<any[]>;
  createAggregatedQuestionnaireGroup(payload: { participantIds: string[], templateId: string }): Observable<any>;
}

export interface IResultService {
  canGetResult(id: string): Observable<boolean>;
  getResultById(id: string): Observable<Result>;
  getCompletedStudentsByGroup(activeQuestionnaireId: string): Observable<Array<{ id: string; student: { fullName: string; userName?: string } }>>;
}

export interface ITeacherService {
  getQuestionnaireGroups(
    pageNumber: number,
    pageSize: number,
    searchTitle: string,
    filterPendingStudent: boolean,
    filterPendingTeacher: boolean
  ): Observable<QuestionnaireGroupResponse>;
}