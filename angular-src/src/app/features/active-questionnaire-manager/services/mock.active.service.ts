import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { ActiveQuestionnaire, ActiveQuestionnaireBase, QuestionnaireGroupOffsetPaginationResult, ResponseActiveQuestionnaireBase, TemplateBaseResponse, UserPaginationResult } from '../models/active.models';
import { User, Role } from '../../../shared/models/user.model';
import { Template, TemplateBase, TemplateStatus } from '../../../shared/models/template.model';
import { IActiveService } from '../../../core/interfaces/service.interfaces';


@Injectable({
  providedIn: 'root',
})
export class MockActiveService implements IActiveService {
private sessionOffsets = new Map<string, number>();
private mockUsers: User[] = [
  { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
  { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: Role.Student },
  { id: 's3', userName: 'jack789', fullName: 'Jack Brown', role: Role.Student},
  { id: 's4', userName: 'jessC', fullName: 'Jessica Carter', role: Role.Student },
  { id: 's5', userName: 'juliaS', fullName: 'Julia Sanchez', role: Role.Student },
  { id: 's6', userName: 'joelM', fullName: 'Joel Martinez', role: Role.Student},
  { id: 's7', userName: 'jacobR', fullName: 'Jacob Robinson', role: Role.Student },
  { id: 's8', userName: 'jordanD', fullName: 'Jordan Davis', role: Role.Student },
  { id: 's9', userName: 'jimB', fullName: 'Jim Brooks', role: Role.Student },
  { id: 's10', userName: 'jasmineW', fullName: 'Jasmine White', role: Role.Student },

  { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
  { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: Role.Teacher },
  { id: 't3', userName: 'jacksonT', fullName: 'Ms. Jackson', role: Role.Teacher },
  { id: 't4', userName: 'jonesT', fullName: 'Dr. Jones', role: Role.Teacher },
  { id: 't5', userName: 'jamesT', fullName: 'Mr. James', role: Role.Teacher },
  { id: 't6', userName: 'jacobsT', fullName: 'Mrs. Jacobs', role: Role.Teacher },
  { id: 't7', userName: 'juliaT', fullName: 'Professor Julia', role: Role.Teacher },
  { id: 't8', userName: 'joanneT', fullName: 'Ms. Joanne', role: Role.Teacher },
  { id: 't9', userName: 'jaredT', fullName: 'Mr. Jared', role: Role.Teacher},
  { id: 't10', userName: 'julianT', fullName: 'Dr. Julian', role: Role.Teacher },
];


  private mockTemplates: Template[] = [
    { id: 't101', title: 'Math Quiz', description: 'A basic math quiz', questions: [], templateStatus:TemplateStatus.Finalized },
    { id: 't102', title: 'History Quiz', description: 'A history knowledge test', questions: [], templateStatus:TemplateStatus.Finalized},
    { id: 't103', title: 'Nature Quiz', description: 'A Nature knowledge test', questions: [],templateStatus:TemplateStatus.Draft}
  ];

  private activeQuestionnaires: ActiveQuestionnaireBase[] = [
    {
      id: 'q1',
      title: 'Math Quiz',
      activatedAt: new Date('2024-02-10T12:00:00'),
      student: { id: 's1', userName: 'johnd123', fullName: 'John Doe', role: Role.Student },
      teacher: { id: 't1', userName: 'smithT', fullName: 'Mrs. Smith', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-10T14:30:00')
    },
    {
      id: 'q2',
      title: 'History Quiz',
      activatedAt: new Date('2024-02-10T12:30:00'),
      student: { id: 's2', userName: 'janes456', fullName: 'Jane Smith', role: Role.Student },
      teacher: { id: 't2', userName: 'johnsonT', fullName: 'Mr. Johnson', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-10T13:00:00'),
      teacherCompletedAt: new Date('2024-02-10T13:15:00')
    },
    {
      id: 'q3',
      title: 'Science Exam',
      activatedAt: new Date('2024-02-11T09:15:00'),
      student: { id: 's3', userName: 'markS99', fullName: 'Mark Spencer', role: Role.Student },
      teacher: { id: 't3', userName: 'leeT', fullName: 'Dr. Lee', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: null
    },
    {
      id: 'q4',
      title: 'English Grammar Test',
      activatedAt: new Date('2024-02-12T14:00:00'),
      student: { id: 's4', userName: 'emilyG', fullName: 'Emily Green', role: Role.Student },
      teacher: { id: 't4', userName: 'wilsonT', fullName: 'Ms. Wilson', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-12T14:45:00'),
      teacherCompletedAt: null
    },
    {
      id: 'q5',
      title: 'Geography Quiz',
      activatedAt: new Date('2024-02-13T08:30:00'),
      student: { id: 's5', userName: 'lucasB', fullName: 'Lucas Brown', role: Role.Student },
      teacher: { id: 't5', userName: 'hallT', fullName: 'Mr. Hall', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: new Date('2024-02-13T10:00:00')
    },
    {
      id: 'q6',
      title: 'Physics Assessment',
      activatedAt: new Date('2024-02-14T11:10:00'),
      student: { id: 's6', userName: 'sophiaW', fullName: 'Sophia White', role: Role.Student },
      teacher: { id: 't6', userName: 'jonesT', fullName: 'Dr. Jones', role: Role.Teacher },
      studentCompletedAt: new Date('2024-02-14T11:55:00'),
      teacherCompletedAt: new Date('2024-02-14T12:15:00')
    },
    {
      id: 'q7',
      title: 'Chemistry Lab Test',
      activatedAt: new Date('2024-02-15T16:45:00'),
      student: { id: 's7', userName: 'oliverR', fullName: 'Oliver Reynolds', role: Role.Student },
      teacher: { id: 't7', userName: 'smithJ', fullName: 'Mr. Smith', role: Role.Teacher },
      studentCompletedAt: null,
      teacherCompletedAt: null
    }
  ];
  

  constructor() {}

  testgetActiveQuestionnaires(
    page: number,
    pageSize: number,
    searchStudent: string = '',
    searchStudentType: 'fullName' | 'userName' | 'both' = 'both',
    searchTeacher: string = '',
    searchTeacherType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<PaginationResponse<ActiveQuestionnaire>> {
    // For mock implementation, convert filtered data to the expected format
    let filteredData = [...this.activeQuestionnaires];

    // Apply student search filter if provided
    if (searchStudent.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.student.fullName, searchStudent, searchStudentType) ||
        this.filterByType(q.student.userName, searchStudent, searchStudentType)
      );
    }

    // Apply teacher search filter if provided
    if (searchTeacher.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.teacher.fullName, searchTeacher, searchTeacherType) ||
        this.filterByType(q.teacher.userName, searchTeacher, searchTeacherType)
      );
    }

    // Mock pagination
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedData = filteredData.slice(startIndex, endIndex);

    // Convert to ActiveQuestionnaire format for response
    const result: PaginationResponse<ActiveQuestionnaire> = {
      items: paginatedData.map(q => ({
        id: q.id,
        title: q.title || 'Untitled',
        description: q.description,
        activatedAt: q.activatedAt,
        student: q.student,
        teacher: q.teacher,
        studentCompletedAt: q.studentCompletedAt,
        teacherCompletedAt: q.teacherCompletedAt
      })),
      totalItems: filteredData.length,
      currentPage: page,
      pageSize: pageSize,
      totalPages: Math.ceil(filteredData.length / pageSize)
    };

    return of(result).pipe(delay(500));
  }

  getActiveQuestionnaires(
    pageSize: number,
    queryCursor: string = '',
    studentSearch: string = '',
    studentSearchType: 'fullName' | 'userName' | 'both' = 'both',
    teacherSearch: string = '',
    teacherSearchType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<ResponseActiveQuestionnaireBase> {
    // Start with a copy of all active questionnaires.
    let filteredData = [...this.activeQuestionnaires];

    // Apply student search filter if provided.
    if (studentSearch.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.student.fullName, studentSearch, studentSearchType) ||
        this.filterByType(q.student.userName, studentSearch, studentSearchType)
      );
    }

    // Apply teacher search filter if provided.
    if (teacherSearch.trim()) {
      filteredData = filteredData.filter(q =>
        this.filterByType(q.teacher.fullName, teacherSearch, teacherSearchType) ||
        this.filterByType(q.teacher.userName, teacherSearch, teacherSearchType)
      );
    }

    // Sort data by activatedAt descending.
    filteredData.sort((a, b) => b.activatedAt.getTime() - a.activatedAt.getTime());

    // Implement keyset pagination:
    // If a queryCursor is provided, it should be in the format "activatedAt_ISO_{id}"
    let startIndex = 0;
    if (queryCursor.trim()) {
      // Attempt to find the index of the record that matches the cursor.
      const [cursorDate, cursorId] = queryCursor.split('_');
      const cursorTime = new Date(cursorDate).getTime();
      startIndex = filteredData.findIndex(q => {
        // We match if activatedAt equals cursorTime and id matches.
        return q.activatedAt.getTime() === cursorTime && q.id === cursorId;
      });
      // If found, start with the next record.
      if (startIndex !== -1) {
        startIndex = startIndex + 1;
      } else {
        startIndex = 0;
      }
    }

    // Slice the data for the given pageSize.
    const paginatedData = filteredData.slice(startIndex, startIndex + pageSize);

    // Create a new query cursor from the last record (if any).
    let newQueryCursor = '';
    if (paginatedData.length > 0) {
      const lastRecord = paginatedData[paginatedData.length - 1];
      newQueryCursor = `${lastRecord.activatedAt.toISOString()}_${lastRecord.id}`;
    }

    // Construct the response object.
    const response: ResponseActiveQuestionnaireBase = {
      activeQuestionnaireBases: paginatedData,
      queryCursor: newQueryCursor,
      totalCount: filteredData.length,
    };

    return of(response).pipe(delay(2000));
  }

  private filterByType(value: string, search: string, type: 'fullName' | 'userName' | 'both'): boolean {
    // For this mock, both cases simply do a case-insensitive search.
    return value.toLowerCase().includes(search.toLowerCase());
  }


  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire> {
    const questionnaire = this.activeQuestionnaires.find(q => q.id === id);
    if (!questionnaire) {
      throw new Error(`Active questionnaire with ID ${id} not found`);
    }
    // Convert to ActiveQuestionnaire
    const activeQuestionnaire: ActiveQuestionnaire = {
      id: questionnaire.id,
      title: questionnaire.title || 'Untitled',
      description: questionnaire.description,
      activatedAt: questionnaire.activatedAt,
      student: questionnaire.student,
      teacher: questionnaire.teacher,
      studentCompletedAt: questionnaire.studentCompletedAt,
      teacherCompletedAt: questionnaire.teacherCompletedAt
    };
    return of(activeQuestionnaire);
  }

  createActiveQuestionnaire(aq: { studentIds: string[]; teacherIds: string[]; templateId: string }): Observable<ActiveQuestionnaire[]> {
    const questionnaires: ActiveQuestionnaire[] = [];
    
    // Create questionnaires for each student-teacher combination
    for (const studentId of aq.studentIds) {
      for (const teacherId of aq.teacherIds) {
        const student = this.mockUsers.find(user => user.id === studentId && user.role === Role.Student);
        const teacher = this.mockUsers.find(user => user.id === teacherId && user.role === Role.Teacher);
        const template = this.mockTemplates.find(t => t.id === aq.templateId);

        if (student && teacher && template) {
          const newQuestionnaire: ActiveQuestionnaire = {
            id: `q${Date.now()}-${Math.random()}`,
            title: template.title,
            activatedAt: new Date(),
            student: student,
            teacher: teacher,
            studentCompletedAt: null,
            teacherCompletedAt: null
          };
          
          questionnaires.push(newQuestionnaire);
          this.activeQuestionnaires.push(newQuestionnaire as ActiveQuestionnaireBase);
        }
      }
    }
    
    return of(questionnaires).pipe(delay(500));
  }

  updateActiveQuestionnaire(id: string, data: Partial<ActiveQuestionnaire>): Observable<ActiveQuestionnaireBase | undefined> {
    const index = this.activeQuestionnaires.findIndex(q => q.id === id);
    if (index !== -1) {
      this.activeQuestionnaires[index] = { ...this.activeQuestionnaires[index], ...data };
      return of(this.activeQuestionnaires[index]);
    }
    return of(undefined);
  }
  

  deleteActiveQuestionnaire(id: string): Observable<boolean> {
    const index = this.activeQuestionnaires.findIndex(q => q.id === id);
    if (index !== -1) {
      this.activeQuestionnaires.splice(index, 1);
      return of(true);
    }
    return of(false);
  }
searchUsers(
  term: string,
  role: 'student' | 'teacher',
  pageSize: number,
  sessionId?: string
): Observable<UserPaginationResult> {

  const query = term.trim().toLowerCase();
  const filtered = this.mockUsers.filter(u =>
    (u.userName.toLowerCase().includes(query) ||
     u.fullName.toLowerCase().includes(query)) &&
    u.role.toLowerCase() === role                     // 'student' | 'teacher'
  );

  // Track pagination by sessionId
  let id = sessionId;
  let offset = 0;

  if (id && this.sessionOffsets.has(id)) {
    offset = this.sessionOffsets.get(id)!;
  } else {
    id = crypto.randomUUID();
  }

  const slice = filtered.slice(offset, offset + pageSize);
  const hasMore = offset + slice.length < filtered.length;

  this.sessionOffsets.set(id, offset + slice.length); // advance pointer

  return of({
    userBases: slice,
    sessionId: id,
    hasMore
  }).pipe(delay(300));
}
  
searchTemplates(term: string, queryCursor: string = ''): Observable<TemplateBaseResponse> {
  const pageSize = 5;
  const q = term.trim().toLowerCase();

  // 1️⃣ filter
  let filtered = this.mockTemplates.filter(t =>
    t.templateStatus === TemplateStatus.Finalized && (
      t.title.toLowerCase().includes(q) ||
      (t.description ?? '').toLowerCase().includes(q)
    )
  );

  // 2️⃣ deterministic order
  filtered = filtered.sort((a, b) => a.title.localeCompare(b.title));

  // 3️⃣ cursor → offset
  let start = 0;
  if (queryCursor) {
    const [cursorTitle, cursorId] = queryCursor.split('_');
    const idx = filtered.findIndex(t => t.title === cursorTitle && t.id === cursorId);
    if (idx !== -1) start = idx + 1;
  }

  const slice = filtered.slice(start, start + pageSize);

  const templateBases: TemplateBase[] = slice.map(t => ({
    id: t.id!,
    title: t.title,
    createdAt: (t as any).createdAt ?? new Date().toISOString(),
    lastUpdated: (t as any).lastUpdated ?? new Date().toISOString(),
    isLocked: (t as any).isLocked ?? false,
    templateStatus: t.templateStatus
  }));

  const hasMore = start + slice.length < filtered.length;
  const nextCursor =
    hasMore && slice.length
      ? `${slice[slice.length - 1].title}_${slice[slice.length - 1].id}`
      : '';

  return of({
    templateBases,
    queryCursor: nextCursor,
    totalCount: filtered.length
  }).pipe(delay(300));
}

createActiveQuestionnaireGroup(aq: { name: string; templateId: string; studentIds: string[]; teacherIds: string[] }): Observable<any> {
  const newGroup = {
    groupId: `group-${Date.now()}`,
    name: aq.name,
    templateId: aq.templateId,
    createdAt: new Date(),
    studentIds: aq.studentIds,
    teacherIds: aq.teacherIds
  };
  return of(newGroup).pipe(delay(500));
}

getQuestionnaireGroup(groupId: string): Observable<any> {
  const mockGroup = {
    groupId: groupId,
    name: `Mock Group ${groupId}`,
    templateId: 't101',
    createdAt: new Date(),
    questionnaires: []
  };
  return of(mockGroup).pipe(delay(300));
}

getQuestionnaireGroups(): Observable<any[]> {
  const mockGroups = [
    { groupId: 'group1', name: 'Math Group 1', templateId: 't101' },
    { groupId: 'group2', name: 'History Group 1', templateId: 't102' }
  ];
  return of(mockGroups).pipe(delay(300));
}

createAnonymousQuestionnaireGroup(payload: { participantIds: string[], templateId: string }): Observable<any> {
  const anonymousGroup = {
    groupId: `anon-group-${Date.now()}`,
    templateId: payload.templateId,
    participantIds: payload.participantIds,
    createdAt: new Date()
  };
  return of(anonymousGroup).pipe(delay(500));
}

getQuestionnaireGroupsPaginated(
  pageNumber: number,
  pageSize: number,
  searchTitle: string = '',
  filterPendingStudent: boolean = false,
  filterPendingTeacher: boolean = false
): Observable<QuestionnaireGroupOffsetPaginationResult> {
  // Mock questionnaire groups data
  let mockGroups = [
    { groupId: 'group1', name: 'Math Group 1', createdAt: new Date('2024-01-01'), templateId: 't101', pendingStudent: false, pendingTeacher: true },
    { groupId: 'group2', name: 'History Group 1', createdAt: new Date('2024-01-02'), templateId: 't102', pendingStudent: true, pendingTeacher: false },
    { groupId: 'group3', name: 'Science Group 1', createdAt: new Date('2024-01-03'), templateId: 't103', pendingStudent: false, pendingTeacher: false }
  ];

  // Apply filters
  if (searchTitle) {
    mockGroups = mockGroups.filter(g => g.name.toLowerCase().includes(searchTitle.toLowerCase()));
  }
  if (filterPendingStudent) {
    mockGroups = mockGroups.filter(g => g.pendingStudent);
  }
  if (filterPendingTeacher) {
    mockGroups = mockGroups.filter(g => g.pendingTeacher);
  }

  // Pagination
  const startIndex = (pageNumber - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const paginatedGroups = mockGroups.slice(startIndex, endIndex);

  const result: QuestionnaireGroupOffsetPaginationResult = {
    groups: paginatedGroups.map(g => ({
      groupId: g.groupId,
      name: g.name,
      createdAt: g.createdAt.toString(),
      templateId: g.templateId,
      questionnaires: [] // Mock empty questionnaires array
    })),
    totalCount: mockGroups.length,
    currentPage: pageNumber,
    totalPages: Math.ceil(mockGroups.length / pageSize)
  };

  return of(result).pipe(delay(500));
}
}