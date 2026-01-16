import { Injectable } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { ActiveQuestionnaireBase, ActiveQuestionnaireResponse, QuestionnaireGroupResponse } from '../models/dashboard.model';
import { ITeacherService } from '../../../core/interfaces/service.interfaces';

@Injectable({
  providedIn: 'root',
})
export class MockTeacherService implements ITeacherService {
  // Generate mock data as ActiveQuestionnaireBase objects.
  private readonly MOCK_DATA: ActiveQuestionnaireBase[] = Array.from({ length: 40 }, (_, i) => {
    const now = new Date();
    if (i === 0) {
      return {
        id: 'active1',
        title: 'Questionnaire 1',
        description: 'Description for Questionnaire 1',
        activatedAt: now,
        studentCompletedAt: new Date(now.getTime() - 3600 * 1000), // Completed 1 hour ago
        teacherCompletedAt: null,
      };
    } else {
      return {
        id: `${i + 1}`,
        title: `Questionnaire ${i + 1}`,
        description: `Description for Questionnaire ${i + 1}`,
        activatedAt: new Date(now.getTime() - i * 3600 * 1000),
        studentCompletedAt: Math.random() > 0.5 ? new Date(now.getTime() - i * 3600 * 1000) : null,
        teacherCompletedAt: Math.random() > 0.5 ? new Date(now.getTime() - i * 3600 * 1000) : null,
      };
    }
  });

  /**
   * Mimics the real service's NEWgetQuestionnaires method with keyset pagination.
   * Uses queryCursor as a stringified start index.
   */
  getQuestionnaires(
    searchTerm: string,
    searchType: 'name' | 'id',
    queryCursor: string | null,
    pageSize: number,
    filterStudentCompleted: boolean,
    filterTeacherCompleted: boolean
  ): Observable<ActiveQuestionnaireResponse> {
    // Clone the data.
    let filtered = [...this.MOCK_DATA];

    // Apply search filter based on search type.
    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();
      if (searchType === 'name') {
        filtered = filtered.filter(q => q.title.toLowerCase().includes(lowerTerm));
      } else {
        filtered = filtered.filter(q => q.id.toLowerCase().includes(lowerTerm));
      }
    }

    // Apply completion filters.
    if (filterStudentCompleted) {
      filtered = filtered.filter(q => q.studentCompletedAt !== null);
    }
    if (filterTeacherCompleted) {
      filtered = filtered.filter(q => q.teacherCompletedAt !== null);
    }

    const totalCount = filtered.length;
    // For keyset pagination, treat queryCursor as a stringified start index.
    const startIndex = queryCursor ? parseInt(queryCursor, 10) : 0;
    const items = filtered.slice(startIndex, startIndex + pageSize);
    // Set nextCursor if there are more items.
    const nextCursor = (startIndex + pageSize < totalCount) ? (startIndex + pageSize).toString() : null;

    const response: ActiveQuestionnaireResponse = {
      activeQuestionnaireBases: items,
      queryCursor: nextCursor,
      totalCount: totalCount,
    };

    // Return the response with a delay to simulate an HTTP call.
    return of(response).pipe(delay(2000));
  }

  /**
   * Mock implementation of getQuestionnaireGroups to match the interface
   */
  getQuestionnaireGroups(
    pageNumber: number,
    pageSize: number,
    searchTitle: string,
    filterPendingStudent: boolean,
    filterPendingTeacher: boolean
  ): Observable<QuestionnaireGroupResponse> {
    // Mock questionnaire groups data
    const mockGroups = [
      {
        groupId: 'group1',
        groupName: 'Math Group 1',
        createdAt: new Date('2024-01-01').toISOString(),
        templateId: 't101',
        questionnaires: this.MOCK_DATA.slice(0, 3)
      },
      {
        groupId: 'group2', 
        groupName: 'History Group 1',
        createdAt: new Date('2024-01-02').toISOString(),
        templateId: 't102',
        questionnaires: this.MOCK_DATA.slice(3, 6)
      },
      {
        groupId: 'group3',
        groupName: 'Science Group 1', 
        createdAt: new Date('2024-01-03').toISOString(),
        templateId: 't103',
        questionnaires: this.MOCK_DATA.slice(6, 9)
      }
    ];

    // Apply search filter
    let filteredGroups = mockGroups;
    if (searchTitle) {
      filteredGroups = mockGroups.filter(g => 
        g.groupName.toLowerCase().includes(searchTitle.toLowerCase())
      );
    }

    // Apply pending filters (mock logic)
    if (filterPendingStudent || filterPendingTeacher) {
      filteredGroups = filteredGroups.filter(g => {
        const hasPendingStudent = g.questionnaires.some(q => q.studentCompletedAt === null);
        const hasPendingTeacher = g.questionnaires.some(q => q.teacherCompletedAt === null);
        
        if (filterPendingStudent && filterPendingTeacher) {
          return hasPendingStudent && hasPendingTeacher;
        }
        if (filterPendingStudent) {
          return hasPendingStudent;
        }
        if (filterPendingTeacher) {
          return hasPendingTeacher;
        }
        return true;
      });
    }

    // Pagination
    const totalCount = filteredGroups.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    const startIndex = (pageNumber - 1) * pageSize;
    const paginatedGroups = filteredGroups.slice(startIndex, startIndex + pageSize);

    const response: QuestionnaireGroupResponse = {
      groups: paginatedGroups,
      totalCount,
      currentPage: pageNumber,
      totalPages
    };

    return of(response).pipe(delay(1000));
  }
}
