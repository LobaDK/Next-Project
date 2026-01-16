import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';
import { 
  QuestionnaireTemplate, 
  Question, 
  User, 
  Answer, 
  PaginationData,
  TableRowData 
} from '../../shared/models/questionnaire-results.models';
import { QuestionnaireUtils } from '../../shared/utils/questionnaire.utils';
import { QuestionnaireResultsDataService } from './services/questionnaire-results-data.service';
import { TimestampFilterComponent } from './components/timestamp-filter/timestamp-filter.component';
import { ResultsTableComponent } from './components/results-table/results-table.component';

@Component({
  selector: 'app-questionnaire-results',
  standalone: true,
  imports: [CommonModule, FormsModule, TimestampFilterComponent, ResultsTableComponent],
  templateUrl: './questionnaire-results.component.html',
  styleUrl: './questionnaire-results.component.css'
})
export class QuestionnaireResultsComponent implements OnInit, AfterViewInit {
  title = 'Questionnaire Results & Analytics';
  
  @ViewChild('ratingHistoryChart', { static: false }) chartCanvas!: ElementRef<HTMLCanvasElement>;
  private chart: Chart | null = null;
  
  // Chart options
  selectedUsersForChart: Set<number> = new Set();
  
  // View options
  showRatingHistory = false;
  
  // Data from service
  users: User[] = [];
  templates: QuestionnaireTemplate[] = [];
  allAnswers: Answer[] = [];
  
  constructor(private dataService: QuestionnaireResultsDataService) {
    Chart.register(...registerables);
  }
  
  // Filtered and paginated data
  filteredAnswers: Answer[] = [];
  displayedAnswers: TableRowData[] = [];
  paginationData: PaginationData = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 10,
    totalItems: 0
  };
  
  // Table structure
  uniqueQuestions: Question[] = [];
  tableData: TableRowData[] = [];
  
  // Timestamp navigation
  availableTimestamps: Date[] = [];
  selectedTimestamp: Date | null = null;
  
  // Expose Math and Array for template
  Math = Math;
  Array = Array;
  
  ngOnInit() {
    this.loadData();
  }
  
  loadData() {
    this.users = this.dataService.getUsers();
    this.templates = this.dataService.getTemplates();
    this.allAnswers = this.dataService.getAllAnswers();
    this.populateAvailableTimestamps();
    this.applyFilters();
  }
  
  ngAfterViewInit() {
    // Chart will be initialized when showRatingHistory is toggled
  }
  
  // Populate available timestamps from data
  populateAvailableTimestamps() {
    this.availableTimestamps = this.dataService.getAvailableTimestamps();
  }
  
  // Filter methods
  clearFilters() {
    this.selectedTimestamp = null;
    this.applyFilters();
  }
  
  applyFilters() {
    this.filteredAnswers = this.dataService.filterAnswersByTimestamp(
      this.allAnswers, 
      this.selectedTimestamp
    );
    this.buildTableData();
    this.updatePagination();
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
    const result = this.dataService.buildTableData(this.filteredAnswers, this.templates);
    this.tableData = result.tableData;
    this.uniqueQuestions = result.uniqueQuestions;
  }
  
  // Pagination methods
  updatePagination() {
    this.paginationData = this.dataService.calculatePagination(
      this.tableData.length,
      this.paginationData.currentPage,
      this.paginationData.pageSize
    );
    this.updateDisplayedData();
  }
  
  updateDisplayedData() {
    this.displayedAnswers = this.dataService.paginateData(this.tableData, this.paginationData);
  }
  
  // Event handlers for child components
  onTimestampSelected(timestamp: Date) {
    this.selectedTimestamp = timestamp;
    this.applyFilters();
  }
  
  onTimestampCleared() {
    this.selectedTimestamp = null;
    this.applyFilters();
  }
  
  onPageChanged(page: number) {
    if (page >= 1 && page <= this.paginationData.totalPages) {
      this.paginationData.currentPage = page;
      this.updateDisplayedData();
    }
  }
  
  onPageSizeChanged(pageSize: number) {
    this.paginationData.pageSize = pageSize;
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
  
  toggleUserForChart(userId: number) {
    if (this.selectedUsersForChart.has(userId)) {
      this.selectedUsersForChart.delete(userId);
    } else {
      this.selectedUsersForChart.add(userId);
    }
    if (this.showRatingHistory) {
      this.createRatingHistoryChart();
    }
  }
  
  isUserSelectedForChart(userId: number): boolean {
    return this.selectedUsersForChart.has(userId);
  }
  
  selectAllUsersForChart() {
    this.selectedUsersForChart = new Set(this.users.map(u => u.id));
    if (this.showRatingHistory) {
      this.createRatingHistoryChart();
    }
  }
  
  clearAllUsersForChart() {
    this.selectedUsersForChart.clear();
    if (this.showRatingHistory) {
      this.createRatingHistoryChart();
    }
  }
  
  createRatingHistoryChart() {
    if (!this.chartCanvas) return;
    
    this.destroyChart();
    
    const chartData = this.prepareRatingHistoryData();
    
    const config: ChartConfiguration<'line'> = {
      type: 'line',
      data: chartData,
      options: {
        responsive: true,
        plugins: {
          title: {
            display: true,
            text: this.selectedUsersForChart.size > 0 
              ? `Rating History for Selected Users (${Array.from(this.selectedUsersForChart).map(id => this.getUserName(id)).join(', ')})`
              : 'Rating History - No Users Selected'
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
    let ratingsData = [...this.allAnswers];
    
    // Filter by selected users if any are specified
    if (this.selectedUsersForChart.size > 0) {
      ratingsData = ratingsData.filter(answer => this.selectedUsersForChart.has(answer.userId));
    }
    
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
    
    const colors = QuestionnaireUtils.generateColors();
    
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
    return QuestionnaireUtils.getUserName(userId, this.users);
  }
  
  getQuestionById(questionId: number): Question | undefined {
    return QuestionnaireUtils.getQuestionById(questionId, this.templates);
  }
  
  // Utility methods
  truncateText(text: string, maxLength: number = 100): string {
    return QuestionnaireUtils.truncateText(text, maxLength);
  }
}