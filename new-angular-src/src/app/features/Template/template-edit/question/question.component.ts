import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TemplateQuestion } from '../template-edit.model';

@Component({
  selector: 'app-question',
  imports: [],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
})
export class QuestionComponent {
  @Input({ required: true }) item!: TemplateQuestion;
  @Input() index = 0;

  @Input() expanded = false;

  @Output() toggle = new EventEmitter<void>();
  @Output() delete = new EventEmitter<void>();
  @Output() edit = new EventEmitter<void>();

  onToggle() {
    this.toggle.emit();
  }

  onDelete(event: MouseEvent) {
    event.stopPropagation();
    this.delete.emit();
  }

  onEdit(event: MouseEvent) {
    event.stopPropagation();
    this.edit.emit();
  }

  // UI helper
  get typeLabel(): string {
    switch (this.item.type) {
      case "radiogroup":
        return "Radio group";
      case "matrix_single":
        return "Single choice matrix";
      case "rating":
        return "Rating";
      default:
        return "Unknown";
    }
  }
}