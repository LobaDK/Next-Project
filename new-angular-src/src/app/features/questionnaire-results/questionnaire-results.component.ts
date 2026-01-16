import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';
import { QuestionnaireMatrixViewComponent, MatrixViewData } from './questionnaire-matrix-view/questionnaire-matrix-view.component';
import { QuestionnaireComparisonViewComponent, ComparisonViewData } from './questionnaire-comparison-view/questionnaire-comparison-view.component';

export interface QuestionnaireTemplate {
  id: number;
  name: string;
  description: string;
  questions: Question[];
}

export interface Question {
  id: number;
  text: string;
  type: 'text' | 'rating';
  maxRating?: number;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

export interface Answer {
  id: number;
  userId: number;
  userName: string;
  templateId: number;
  templateName: string;
  questionId: number;
  questionText: string;
  value: string | number;
  submittedAt: Date;
}

export interface PaginationData {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalItems: number;
}

@Component({
  selector: 'app-questionnaire-results',
  standalone: true,
  imports: [CommonModule, FormsModule, QuestionnaireMatrixViewComponent, QuestionnaireComparisonViewComponent],
  templateUrl: './questionnaire-results.component.html',
  styleUrl: './questionnaire-results.component.css'
})
export class QuestionnaireResultsComponent implements OnInit, AfterViewInit {
  title = 'My Questionnaire Analytics';
  
  @ViewChild('ratingHistoryChart', { static: false }) chartCanvas!: ElementRef<HTMLCanvasElement>;
  private chart: Chart | null = null;
  
  constructor() {
    Chart.register(...registerables);
  }
  
  // Current user (always included)
  currentUserId = 1; // John Doe (Owner)
  
  // Search filters
  searchExtraUser = '';
  selectedExtraUserId: number | null = null;
  selectedTemplateId: number | null = null;
  
  // View options
  showRatingHistory = false;
  currentViewMode: 'table' | 'matrix' | 'comparison' = 'table';
  
  // Data for view components
  matrixViewData: MatrixViewData[] = [];
  comparisonViewData: ComparisonViewData[] = [];
  
  // Data
  users: User[] = [
    { id: 1, name: 'John Doe (Owner)', email: 'john@company.com' },
    { id: 2, name: 'Jane Smith', email: 'jane@company.com' }
  ];
  
  templates: QuestionnaireTemplate[] = [
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
      name: 'Training Feedback Form',
      description: 'Feedback on training programs and effectiveness',
      questions: [
        { id: 5, text: 'Rate the training content quality', type: 'rating', maxRating: 10 },
        { id: 6, text: 'What was the most valuable part of the training?', type: 'text' },
        { id: 7, text: 'How would you rate the trainer?', type: 'rating', maxRating: 5 },
        { id: 8, text: 'Additional comments or suggestions', type: 'text' }
      ]
    }
  ];
  
  allAnswers: Answer[] = [
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
    
    // Training Feedback - John Doe (Owner) - second submission
    { id: 9, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Training Feedback Form', questionId: 5, questionText: 'Rate the training content quality', value: 7, submittedAt: new Date('2024-01-20') },
    { id: 10, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Training Feedback Form', questionId: 6, questionText: 'What was the most valuable part of the training?', value: 'The practical examples and case studies were very helpful.', submittedAt: new Date('2024-01-20') },
    { id: 11, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Training Feedback Form', questionId: 7, questionText: 'How would you rate the trainer?', value: 4, submittedAt: new Date('2024-01-20') },
    { id: 12, userId: 1, userName: 'John Doe (Owner)', templateId: 2, templateName: 'Training Feedback Form', questionId: 8, questionText: 'Additional comments or suggestions', value: 'Would like more interactive sessions.', submittedAt: new Date('2024-01-20') },
    
    // Training Feedback - Jane Smith
    { id: 13, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Training Feedback Form', questionId: 5, questionText: 'Rate the training content quality', value: 8, submittedAt: new Date('2024-01-22') },
    { id: 14, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Training Feedback Form', questionId: 6, questionText: 'What was the most valuable part of the training?', value: 'The hands-on exercises and real-world examples made the concepts much clearer.', submittedAt: new Date('2024-01-22') },
    { id: 15, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Training Feedback Form', questionId: 7, questionText: 'How would you rate the trainer?', value: 5, submittedAt: new Date('2024-01-22') },
    { id: 16, userId: 2, userName: 'Jane Smith', templateId: 2, templateName: 'Training Feedback Form', questionId: 8, questionText: 'Additional comments or suggestions', value: 'More time for Q&A would be helpful.', submittedAt: new Date('2024-01-22') }
  ];
  
  // Filtered and paginated data
  availableTemplates: QuestionnaireTemplate[] = []; // Templates available to selected users
  filteredAnswers: Answer[] = [];
  displayedAnswers: Answer[] = [];
  paginationData: PaginationData = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 10,
    totalItems: 0
  };
  
  // Table structure
  uniqueQuestions: Question[] = [];
  tableData: any[] = [];
  
  // Timestamp navigation
  availableTimestamps: Date[] = [];
  selectedTimestamp: Date | null = null;
  
  // Expose Math and Array for template
  Math = Math;
  Array = Array;
  
  ngOnInit() {
    this.updateAvailableTemplates();
    this.populateAvailableTimestamps();
    this.applyFilters();
  }
  
  ngAfterViewInit() {
    // Chart will be initialized when showRatingHistory is toggled
  }
  
  // Populate available timestamps from data
  populateAvailableTimestamps() {
    const timestamps = [...new Set(this.allAnswers.map(answer => answer.submittedAt.getTime()))];
    this.availableTimestamps = timestamps.map(time => new Date(time)).sort((a, b) => b.getTime() - a.getTime());
  }
  
  // Search and filter methods
  onSearchExtraUser() {
    // Auto-select if only one result found
    const filtered = this.getAvailableExtraUsers().filter(user => 
      user.name.toLowerCase().includes(this.searchExtraUser.toLowerCase()) ||
      user.email.toLowerCase().includes(this.searchExtraUser.toLowerCase())
    );
    if (filtered.length === 1) {
      this.selectedExtraUserId = filtered[0].id;
    }
    this.updateAvailableTemplates();
    this.applyFilters();
  }
  
  selectExtraUser(userId: number) {
    this.selectedExtraUserId = userId;
    this.searchExtraUser = this.users.find(u => u.id === userId)?.name || '';
    this.updateAvailableTemplates();
    this.applyFilters();
  }
  
  getAvailableExtraUsers(): User[] {
    return this.users.filter(user => user.id !== this.currentUserId);
  }
  
  updateAvailableTemplates() {
    const selectedUserIds = this.getSelectedUserIds();
    
    // Find templates where at least one of the selected users has responses
    this.availableTemplates = this.templates.filter(template => {
      return selectedUserIds.some(userId => 
        this.allAnswers.some(answer => 
          answer.userId === userId && answer.templateId === template.id
        )
      );
    });
    
    // Clear template selection if current template is no longer available
    if (this.selectedTemplateId && !this.availableTemplates.find(t => t.id === this.selectedTemplateId)) {
      this.selectedTemplateId = null;
    }
  }
  
  getSelectedUserIds(): number[] {
    const userIds = [this.currentUserId];
    if (this.selectedExtraUserId) {
      userIds.push(this.selectedExtraUserId);
    }
    return userIds;
  }
  
  selectTemplate(templateId: number) {
    this.selectedTemplateId = templateId;
    this.applyFilters();
  }
  
  clearExtraUser() {
    this.searchExtraUser = '';
    this.selectedExtraUserId = null;
    this.updateAvailableTemplates();
    this.applyFilters();
  }
  
  clearTemplate() {
    this.selectedTemplateId = null;
    this.applyFilters();
  }
  
  applyFilters() {
    let filtered = [...this.allAnswers];
    
    // Always filter by selected users (current + optional extra)
    const selectedUserIds = this.getSelectedUserIds();
    filtered = filtered.filter(answer => selectedUserIds.includes(answer.userId));
    
    // Filter by selected template if one is chosen
    if (this.selectedTemplateId) {
      filtered = filtered.filter(answer => answer.templateId === this.selectedTemplateId);
    }
    
    // Filter by timestamp if selected
    if (this.selectedTimestamp) {
      filtered = filtered.filter(answer => 
        answer.submittedAt.getTime() === this.selectedTimestamp!.getTime()
      );
    }
    
    this.filteredAnswers = filtered;
    this.buildTableData();
    this.updatePagination();
    this.updateViewComponentData();
  }
  
  // View switching methods
  switchToTableView() {
    this.currentViewMode = 'table';
  }
  
  switchToMatrixView() {
    this.currentViewMode = 'matrix';
    this.updateViewComponentData();
  }
  
  switchToComparisonView() {
    this.currentViewMode = 'comparison';
    this.updateViewComponentData();
  }
  
  private updateViewComponentData() {
    // Transform filteredAnswers to format for view components
    this.matrixViewData = this.transformToMatrixViewData();
    this.comparisonViewData = this.transformToComparisonViewData();
  }
  
  private transformToMatrixViewData(): MatrixViewData[] {
    const userTemplateGroups = new Map<string, any>();
    
    this.filteredAnswers.forEach(answer => {
      const key = `${answer.userId}-${answer.templateId}`;
      if (!userTemplateGroups.has(key)) {
        userTemplateGroups.set(key, {
          id: key,
          userName: answer.userName,
          templateName: answer.templateName,
          submittedAt: answer.submittedAt,
          answers: []
        });
      }
      
      const template = this.templates.find(t => t.id === answer.templateId);
      const question = template?.questions.find(q => q.id === answer.questionId);
      
      userTemplateGroups.get(key).answers.push({
        questionId: answer.questionId,
        questionText: answer.questionText,
        questionType: question?.type || 'text',
        value: answer.value,
        maxRating: question?.maxRating
      });
    });
    
    return Array.from(userTemplateGroups.values());
  }
  
  private transformToComparisonViewData(): ComparisonViewData[] {
    return this.transformToMatrixViewData().map(item => ({
      id: item.id,
      userName: item.userName,
      templateName: item.templateName,
      submittedAt: item.submittedAt,
      answers: item.answers.map((answer: any) => ({
        questionId: answer.questionId,
        questionText: answer.questionText,
        questionType: answer.questionType,
        value: answer.value,
        maxRating: answer.maxRating
      }))
    }));
  }
  
  // Timestamp navigation methods
  selectTimestamp(timestamp: Date) {
    this.selectedTimestamp = timestamp;
    this.applyFilters();
  }
  
  clearTimestampFilter() {
    this.selectedTimestamp = null;
    this.applyFilters();
  }
  
  getTimestampDisplayText(timestamp: Date): string {
    return timestamp.toLocaleDateString() + ' ' + timestamp.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
  }
  
  getCurrentTimestampIndex(): number {
    if (!this.selectedTimestamp) return -1;
    return this.availableTimestamps.findIndex(t => t.getTime() === this.selectedTimestamp!.getTime());
  }
  
  goToPreviousTimestamp() {
    const currentIndex = this.getCurrentTimestampIndex();
    if (currentIndex > 0) {
      this.selectTimestamp(this.availableTimestamps[currentIndex - 1]);
    }
  }
  
  goToNextTimestamp() {
    const currentIndex = this.getCurrentTimestampIndex();
    if (currentIndex < this.availableTimestamps.length - 1) {
      this.selectTimestamp(this.availableTimestamps[currentIndex + 1]);
    }
  }
  
  // Table building methods
  buildTableData() {
    // Get unique questions from filtered data
    const questionMap = new Map<number, Question>();
    this.filteredAnswers.forEach(answer => {
      if (!questionMap.has(answer.questionId)) {
        const template = this.templates.find(t => t.id === answer.templateId);
        const question = template?.questions.find(q => q.id === answer.questionId);
        if (question) {
          questionMap.set(answer.questionId, question);
        }
      }
    });
    
    this.uniqueQuestions = Array.from(questionMap.values());
    
    // Group answers by user and template
    const userTemplateGroups = new Map<string, Answer[]>();
    this.filteredAnswers.forEach(answer => {
      const key = `${answer.userId}-${answer.templateId}`;
      if (!userTemplateGroups.has(key)) {
        userTemplateGroups.set(key, []);
      }
      userTemplateGroups.get(key)!.push(answer);
    });
    
    // Build table rows
    this.tableData = Array.from(userTemplateGroups.entries()).map(([key, answers]) => {
      const row: any = {
        id: key,
        userName: answers[0].userName,
        templateName: answers[0].templateName,
        submittedAt: answers[0].submittedAt
      };
      
      // Add question answers to row
      this.uniqueQuestions.forEach(question => {
        const answer = answers.find(a => a.questionId === question.id);
        row[`question_${question.id}`] = answer ? answer.value : null;
      });
      
      return row;
    });
  }
  
  // Pagination methods
  updatePagination() {
    this.paginationData.totalItems = this.tableData.length;
    this.paginationData.totalPages = Math.ceil(this.paginationData.totalItems / this.paginationData.pageSize);
    this.paginationData.currentPage = Math.min(this.paginationData.currentPage, this.paginationData.totalPages);
    if (this.paginationData.currentPage < 1) this.paginationData.currentPage = 1;
    
    this.updateDisplayedData();
  }
  
  updateDisplayedData() {
    const startIndex = (this.paginationData.currentPage - 1) * this.paginationData.pageSize;
    const endIndex = startIndex + this.paginationData.pageSize;
    this.displayedAnswers = this.tableData.slice(startIndex, endIndex);
  }
  
  goToPage(page: number) {
    if (page >= 1 && page <= this.paginationData.totalPages) {
      this.paginationData.currentPage = page;
      this.updateDisplayedData();
    }
  }
  
  previousPage() {
    this.goToPage(this.paginationData.currentPage - 1);
  }
  
  nextPage() {
    this.goToPage(this.paginationData.currentPage + 1);
  }
  
  onPageSizeChange() {
    this.paginationData.currentPage = 1;
    this.updatePagination();
  }
  
  // Chart methods
  toggleRatingHistory() {
    this.showRatingHistory = !this.showRatingHistory;
    if (this.showRatingHistory) {
      setTimeout(() => this.createRatingHistoryChart(), 0);
    } else {
      this.destroyChart();
    }
  }
  
  createRatingHistoryChart() {
    if (!this.chartCanvas) return;
    
    this.destroyChart();
    
    const chartData = this.prepareRatingHistoryData();
    
    const selectedUsers = this.getSelectedUserIds().map(id => this.getUserName(id)).join(', ');
    const templateName = this.selectedTemplateId ? 
      this.availableTemplates.find(t => t.id === this.selectedTemplateId)?.name || 'Unknown Template' : 
      'All Available Templates';
    
    const config: ChartConfiguration<'line'> = {
      type: 'line',
      data: chartData,
      options: {
        responsive: true,
        plugins: {
          title: {
            display: true,
            text: `Rating History: ${templateName} (${selectedUsers})`
          },
          legend: {
            display: true,
            position: 'bottom'
          }
        },
        scales: {
          x: {
            title: {
              display: true,
              text: 'Submission Date'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Rating'
            },
            min: 0,
            max: 10,
            ticks: {
              stepSize: 1
            }
          }
        },
        interaction: {
          intersect: false,
          mode: 'index'
        }
      }
    };
    
    this.chart = new Chart(this.chartCanvas.nativeElement, config);
  }
  
  destroyChart() {
    if (this.chart) {
      this.chart.destroy();
      this.chart = null;
    }
  }
  
  prepareRatingHistoryData() {
    // Use the already filtered answers (which include selected users and optional template filter)
    let ratingsData = [...this.filteredAnswers];
    
    // If no data after filtering, return empty chart
    if (ratingsData.length === 0) {
      return {
        labels: [],
        datasets: []
      };
    }
    
    // Filter only rating questions
    ratingsData = ratingsData.filter(answer => {
      const template = this.templates.find(t => t.id === answer.templateId);
      const question = template?.questions.find(q => q.id === answer.questionId);
      return question?.type === 'rating';
    });
    
    // Get all unique dates and sort them
    const uniqueDates = [...new Set(ratingsData.map(answer => answer.submittedAt.toLocaleDateString()))];
    uniqueDates.sort((a, b) => new Date(a).getTime() - new Date(b).getTime());
    
    // Group by question and create datasets
    const questionGroups = new Map<number, Answer[]>();
    ratingsData.forEach(answer => {
      if (!questionGroups.has(answer.questionId)) {
        questionGroups.set(answer.questionId, []);
      }
      questionGroups.get(answer.questionId)!.push(answer);
    });
    
    const colors = [
      'rgb(255, 99, 132)',   // Red
      'rgb(54, 162, 235)',   // Blue
      'rgb(255, 205, 86)',   // Yellow
      'rgb(75, 192, 192)',   // Teal
      'rgb(153, 102, 255)',  // Purple
      'rgb(255, 159, 64)'    // Orange
    ];
    
    const datasets = Array.from(questionGroups.entries()).map(([questionId, answers], index) => {
      const question = this.getQuestionById(questionId);
      
      // Create data points for each date
      const dataPoints = uniqueDates.map(date => {
        const answerForDate = answers.find(answer => answer.submittedAt.toLocaleDateString() === date);
        return answerForDate ? Number(answerForDate.value) : null;
      });
      
      return {
        label: question ? this.truncateText(question.text, 30) : `Question ${questionId}`,
        data: dataPoints,
        borderColor: colors[index % colors.length],
        backgroundColor: colors[index % colors.length] + '20',
        tension: 0.1,
        fill: false,
        spanGaps: true // This allows the line to continue even when there are null values
      };
    });
    
    return {
      labels: uniqueDates,
      datasets: datasets
    };
  }
  
  getUserName(userId: number): string {
    return this.users.find(u => u.id === userId)?.name || `User ${userId}`;
  }
  
  getQuestionById(questionId: number): Question | undefined {
    for (const template of this.templates) {
      const question = template.questions.find(q => q.id === questionId);
      if (question) return question;
    }
    return undefined;
  }
  
  // Utility methods
  truncateText(text: string, maxLength: number = 100): string {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
  }
  
  getQuestionText(questionId: number): string {
    const question = this.uniqueQuestions.find(q => q.id === questionId);
    return question ? this.truncateText(question.text, 50) : '';
  }
  
  getPaginationNumbers(): number[] {
    const totalPages = this.paginationData.totalPages;
    const currentPage = this.paginationData.currentPage;
    const numbers: number[] = [];
    
    // Show max 5 page numbers
    const start = Math.max(1, currentPage - 2);
    const end = Math.min(totalPages, start + 4);
    
    for (let i = start; i <= end; i++) {
      numbers.push(i);
    }
    
    return numbers;
  }
  
  // Helper methods for template
  getCurrentUserName(): string {
    return this.users.find(u => u.id === this.currentUserId)?.name || 'Current User';
  }
  
  getSelectedExtraUserName(): string {
    return this.users.find(u => u.id === this.selectedExtraUserId)?.name || '';
  }
  
  getSelectedTemplateName(): string {
    return this.availableTemplates.find(t => t.id === this.selectedTemplateId)?.name || '';
  }
  
  canShowChart(): boolean {
    return this.selectedTemplateId !== null;
  }
  
  getResponseCountForQuestion(questionId: number): number {
    return this.filteredAnswers.filter(a => a.questionId === questionId).length;
  }
  
  getQuestionAnswer(row: any, questionId: number): any {
    return row['question_' + questionId];
  }
}