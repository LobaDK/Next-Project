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
    },
    {
      id: 2,
      name: 'Employee Satisfaction Survey',
      description: 'Annual employee satisfaction and engagement survey',
      questions: [
        { id: 5, text: 'How satisfied are you with your current role?', type: 'rating', maxRating: 5 },
        { id: 6, text: 'What do you like most about working here?', type: 'text' },
        { id: 7, text: 'Rate your work-life balance', type: 'rating', maxRating: 5 },
        { id: 8, text: 'Any suggestions for improvement?', type: 'text' }
      ]
    }
  ];
  
  private mockAnswers: Answer[] = [
    // Employee Satisfaction - John Doe (Owner)
    { id: 1, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 4, submittedAt: new Date('2024-01-15') },
    { id: 2, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'Great team collaboration and flexible working hours. The company culture is very supportive and encourages innovation.', submittedAt: new Date('2024-01-15') },
    { id: 3, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 3, submittedAt: new Date('2024-01-15') },
    { id: 4, userId: 1, userName: 'John Doe (Owner)', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'More professional development opportunities would be great.', submittedAt: new Date('2024-01-15') },
    
    // Employee Satisfaction - Jane Smith
    { id: 5, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 1, questionText: 'How satisfied are you with your current role?', value: 5, submittedAt: new Date('2024-01-16') },
    { id: 6, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 2, questionText: 'What do you like most about working here?', value: 'The challenging projects and opportunities to learn new technologies.', submittedAt: new Date('2024-01-16') },
    { id: 7, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 3, questionText: 'Rate your work-life balance', value: 4, submittedAt: new Date('2024-01-16') },
    { id: 8, userId: 2, userName: 'Jane Smith', templateId: 1, templateName: 'Employee Satisfaction Survey', questionId: 4, questionText: 'Any suggestions for improvement?', value: 'Better communication between departments.', submittedAt: new Date('2024-01-16') },
    
    // Employee Satisfaction - John Doe (Owner) - second submission
    { id: 9, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 5, questionText: 'How satisfied are you with your current role?', value: 4, submittedAt: new Date('2024-01-20') },
    { id: 10, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 6, questionText: 'What do you like most about working here?', value: 'The practical examples and case studies were very helpful.', submittedAt: new Date('2024-01-20') },
    { id: 11, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 7, questionText: 'Rate your work-life balance', value: 4, submittedAt: new Date('2024-01-20') },
    { id: 12, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 8, questionText: 'Any suggestions for improvement?', value: 'Would like more interactive sessions.', submittedAt: new Date('2024-01-20') },
    
    // Employee Satisfaction - Jane Smith - second submission
    { id: 13, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 5, questionText: 'How satisfied are you with your current role?', value: 5, submittedAt: new Date('2024-01-22') },
    { id: 14, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 6, questionText: 'What do you like most about working here?', value: 'The hands-on exercises and real-world examples made the concepts much clearer.', submittedAt: new Date('2024-01-22') },
    { id: 15, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 7, questionText: 'Rate your work-life balance', value: 4, submittedAt: new Date('2024-01-22') },
    { id: 16, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Employee Satisfaction Survey', questionId: 8, questionText: 'Any suggestions for improvement?', value: 'More time for Q&A would be helpful.', submittedAt: new Date('2024-01-22') }
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
    // Get unique questions by text and type (to avoid duplicates from different templates)
    const questionMap = new Map<string, Question>();
    filteredAnswers.forEach(answer => {
      const template = templates.find(t => t.id === answer.templateId);
      const question = template?.questions.find((q: Question) => q.id === answer.questionId);
      if (question) {
        const questionKey = `${question.text}-${question.type}`;
        if (!questionMap.has(questionKey)) {
          questionMap.set(questionKey, question);
        }
      }
    });
    
    const uniqueQuestions = Array.from(questionMap.values());
    
    // Group answers by user only (since templates are now identical)
    const userGroups = new Map<number, Answer[]>();
    filteredAnswers.forEach(answer => {
      const userId = answer.userId;
      if (!userGroups.has(userId)) {
        userGroups.set(userId, []);
      }
      userGroups.get(userId)!.push(answer);
    });
    
    // Build table rows - one per user with their most recent answers
    const tableData = Array.from(userGroups.entries()).map(([userId, answers]) => {
      // Sort answers by submission date (most recent first)
      answers.sort((a, b) => b.submittedAt.getTime() - a.submittedAt.getTime());
      
      const row: TableRowData = {
        id: userId.toString(),
        userName: answers[0].userName,
        templateName: answers[0].templateName,
        submittedAt: answers[0].submittedAt
      };
      
      // Add question answers to row - use most recent answer for each question type
      uniqueQuestions.forEach(question => {
        // Find the most recent answer for this question text (since questions might have different IDs but same text)
        const matchingAnswer = answers.find(a => {
          const answerTemplate = templates.find(t => t.id === a.templateId);
          const answerQuestion = answerTemplate?.questions.find((q: Question) => q.id === a.questionId);
          return answerQuestion?.text === question.text && answerQuestion?.type === question.type;
        });
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