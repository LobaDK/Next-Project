import { Injectable } from '@angular/core';
import { 
  Answer, 
  QuestionnaireTemplate, 
  User, 
  Question, 
  TableRowData,
  PaginationData 
} from '../../../shared/models/questionnaire-results.models';

@Injectable({
  providedIn: 'root'
})
export class QuestionnaireResultsDataService {
  
  // Mock data - in real app, this would come from API
  private mockUsers: User[] = [
    { id: 1, name: 'John Doe (Owner)', email: 'john@company.com' },
    { id: 2, name: 'Jane Smith', email: 'jane@company.com' }
  ];
  
  private mockTemplates: QuestionnaireTemplate[] = [
    {
      id: 1,
      name: 'Employee Satisfaction Survey',
      description: 'Annual employee satisfaction and engagement survey',
      questions: [
        { id: 1, text: 'How satisfied are you with your current role?', type: 'rating', maxRating: 5 },
        { id: 2, text: 'What do you like most about working here?', type: 'text' },
        { id: 3, text: 'Rate your work-life balance', type: 'rating', maxRating: 5 },
        { id: 4, text: 'Any suggestions for improvement?', type: 'text' }
      ]
    }
  ];
  
  private mockAnswers: Answer[] = [
    // Survey Session 1 - 2024-01-15 - Both users respond
    { id: 1, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 4, submittedAt: new Date('2024-01-15') },
    { id: 2, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'Great team collaboration and flexible working hours.', submittedAt: new Date('2024-01-15') },
    { id: 3, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 3, submittedAt: new Date('2024-01-15') },
    { id: 4, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'More professional development opportunities would be great.', submittedAt: new Date('2024-01-15') },
    
    { id: 5, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 5, submittedAt: new Date('2024-01-15') },
    { id: 6, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'The challenging projects and opportunities to learn new technologies.', submittedAt: new Date('2024-01-15') },
    { id: 7, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 4, submittedAt: new Date('2024-01-15') },
    { id: 8, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'Better communication between departments.', submittedAt: new Date('2024-01-15') },
    
    // Survey Session 2 - 2024-01-20 - Both users respond again
    { id: 9, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 5, submittedAt: new Date('2024-01-20') },
    { id: 10, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'The practical examples and case studies were very helpful. Really enjoying the new projects.', submittedAt: new Date('2024-01-20') },
    { id: 11, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 4, submittedAt: new Date('2024-01-20') },
    { id: 12, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'Would like more interactive sessions and team building activities.', submittedAt: new Date('2024-01-20') },
    
    { id: 13, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 5, submittedAt: new Date('2024-01-20') },
    { id: 14, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'The hands-on exercises and real-world examples made the concepts much clearer. Great learning environment.', submittedAt: new Date('2024-01-20') },
    { id: 15, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 5, submittedAt: new Date('2024-01-20') },
    { id: 16, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'More time for Q&A would be helpful. Also, more flexible working hours.', submittedAt: new Date('2024-01-20') }
  ];

  getUsers(): User[] {
    return this.mockUsers;
  }

  getTemplates(): QuestionnaireTemplate[] {
    return this.mockTemplates;
  }

  getAllAnswers(): Answer[] {
    return this.mockAnswers;
  }

  getAvailableTimestamps(): Date[] {
    const timestamps = [...new Set(this.mockAnswers.map(answer => answer.submittedAt.getTime()))];
    return timestamps.map(time => new Date(time)).sort((a, b) => b.getTime() - a.getTime());
  }

  filterAnswersByTimestamp(answers: Answer[], timestamp: Date | null): Answer[] {
    if (!timestamp) return answers;
    
    return answers.filter(answer => 
      answer.submittedAt.getTime() === timestamp.getTime()
    );
  }

  buildTableData(filteredAnswers: Answer[], templates: QuestionnaireTemplate[]): { 
    tableData: TableRowData[], 
    uniqueQuestions: Question[] 
  } {
    // Get unique questions from filtered answers
    const questionMap = new Map<number, Question>();
    filteredAnswers.forEach(answer => {
      const template = templates.find(t => t.id === answer.templateId);
      const question = template?.questions.find((q: Question) => q.id === answer.questionId);
      if (question && !questionMap.has(question.id)) {
        questionMap.set(question.id, question);
      }
    });
    
    const uniqueQuestions = Array.from(questionMap.values());
    
    // Group answers by user and submission date
    const userDateGroups = new Map<string, Answer[]>();
    filteredAnswers.forEach(answer => {
      const key = `${answer.userId}-${answer.submittedAt.getTime()}`;
      if (!userDateGroups.has(key)) {
        userDateGroups.set(key, []);
      }
      userDateGroups.get(key)!.push(answer);
    });
    
    // Build table rows - one per user per submission date
    const tableData = Array.from(userDateGroups.entries()).map(([key, answers]) => {
      const row: TableRowData = {
        id: key,
        userName: answers[0].userName,
        templateName: answers[0].templateName,
        submittedAt: answers[0].submittedAt
      };
      
      // Add question answers to row
      uniqueQuestions.forEach(question => {
        const matchingAnswer = answers.find(a => a.questionId === question.id);
        row[`question_${question.id}`] = matchingAnswer ? matchingAnswer.value : null;
      });
      
      return row;
    });

    return { tableData, uniqueQuestions };
  }

  calculatePagination(totalItems: number, currentPage: number, pageSize: number): PaginationData {
    const totalPages = Math.ceil(totalItems / pageSize);
    const validCurrentPage = Math.min(Math.max(1, currentPage), totalPages || 1);
    
    return {
      currentPage: validCurrentPage,
      totalPages: totalPages || 1,
      pageSize,
      totalItems
    };
  }

  paginateData<T>(data: T[], pagination: PaginationData): T[] {
    const startIndex = (pagination.currentPage - 1) * pagination.pageSize;
    const endIndex = startIndex + pagination.pageSize;
    return data.slice(startIndex, endIndex);
  }
}