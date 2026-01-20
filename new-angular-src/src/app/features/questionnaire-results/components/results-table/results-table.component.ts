import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Question, TableRowData, PaginationData } from '../../../../shared/models/questionnaire-results.models';
import { QuestionnaireUtils } from '../../../../shared/utils/questionnaire.utils';

@Component({
  selector: 'app-results-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './results-table.component.html',
  styleUrl: './results-table.component.css'
})
export class ResultsTableComponent {
  @Input() displayedData: TableRowData[] = [];
  @Input() uniqueQuestions: Question[] = [];
  @Input() paginationData: PaginationData = {
    currentPage: 1,
    totalPages: 1,
    pageSize: 10,
    totalItems: 0
  };

  @Output() pageChanged = new EventEmitter<number>();
  @Output() pageSizeChanged = new EventEmitter<number>();

  getQuestionText(questionId: number): string {
    const question = this.uniqueQuestions.find(q => q.id === questionId);
    return question ? QuestionnaireUtils.truncateText(question.text, 50) : '';
  }

  getQuestionAnswer(row: TableRowData, questionId: number): any {
    return row[`question_${questionId}`];
  }

  truncateText(text: string, maxLength: number): string {
    return QuestionnaireUtils.truncateText(text, maxLength);
  }

  goToPage(page: number) {
    this.pageChanged.emit(page);
  }

  previousPage() {
    this.pageChanged.emit(this.paginationData.currentPage - 1);
  }

  nextPage() {
    this.pageChanged.emit(this.paginationData.currentPage + 1);
  }

  onPageSizeChange() {
    this.pageSizeChanged.emit(this.paginationData.pageSize);
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
}