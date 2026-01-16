import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { QuestionnaireUtils } from '../../../../shared/utils/questionnaire.utils';

@Component({
  selector: 'app-timestamp-filter',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <!-- Timestamp Navigation Controls -->
    <div class="mb-8 p-4 bg-gray-50 rounded-lg border border-gray-200">
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        
        <!-- Timestamp Selector -->
        <div class="flex items-center gap-2">
          <label class="text-sm font-medium text-primary_dark_blue">Select by Date:</label>
          <select 
            class="select-field w-48"
            [(ngModel)]="selectedTimestamp"
            (change)="onTimestampChange()">
            <option [ngValue]="null">All Dates</option>
            @for (timestamp of availableTimestamps; track timestamp.getTime()) {
              <option [ngValue]="timestamp">{{ getTimestampDisplayText(timestamp) }}</option>
            }
          </select>
          @if (selectedTimestamp) {
            <button 
              class="btn-sm btn-secondary"
              (click)="clearTimestamp()">
              Clear
            </button>
          }
        </div>

        <!-- Results Info -->
        <div class="text-sm app-text-muted text-center sm:text-left">
          @if (selectedTimestamp) {
            <span>Showing responses from {{ getTimestampDisplayText(selectedTimestamp) }}</span>
          } @else {
            <span>Showing {{ totalItems }} total responses</span>
          }
        </div>

        <!-- Timestamp Navigation -->
        @if (selectedTimestamp) {
          <div class="flex items-center gap-2">
            <button 
              class="btn-sm btn-secondary"
              [disabled]="getCurrentTimestampIndex() === 0"
              (click)="goToPreviousTimestamp()">
              ← Earlier
            </button>
            
            <span class="text-sm text-gray-600">
              {{ getCurrentTimestampIndex() + 1 }} of {{ availableTimestamps.length }}
            </span>
            
            <button 
              class="btn-sm btn-secondary"
              [disabled]="getCurrentTimestampIndex() === availableTimestamps.length - 1"
              (click)="goToNextTimestamp()">
              Later →
            </button>
          </div>
        }
      </div>
    </div>
  `
})
export class TimestampFilterComponent {
  @Input() availableTimestamps: Date[] = [];
  @Input() selectedTimestamp: Date | null = null;
  @Input() totalItems: number = 0;
  
  @Output() timestampSelected = new EventEmitter<Date>();
  @Output() timestampCleared = new EventEmitter<void>();

  onTimestampChange() {
    if (this.selectedTimestamp) {
      this.timestampSelected.emit(this.selectedTimestamp);
    } else {
      this.timestampCleared.emit();
    }
  }

  clearTimestamp() {
    this.selectedTimestamp = null;
    this.timestampCleared.emit();
  }

  getCurrentTimestampIndex(): number {
    if (!this.selectedTimestamp) return -1;
    return this.availableTimestamps.findIndex(t => t.getTime() === this.selectedTimestamp!.getTime());
  }

  goToPreviousTimestamp() {
    const currentIndex = this.getCurrentTimestampIndex();
    if (currentIndex > 0) {
      this.selectedTimestamp = this.availableTimestamps[currentIndex - 1];
      this.timestampSelected.emit(this.selectedTimestamp);
    }
  }

  goToNextTimestamp() {
    const currentIndex = this.getCurrentTimestampIndex();
    if (currentIndex < this.availableTimestamps.length - 1) {
      this.selectedTimestamp = this.availableTimestamps[currentIndex + 1];
      this.timestampSelected.emit(this.selectedTimestamp);
    }
  }

  getTimestampDisplayText(timestamp: Date): string {
    return QuestionnaireUtils.getTimestampDisplayText(timestamp);
  }
}