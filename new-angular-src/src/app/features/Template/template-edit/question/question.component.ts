import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RadioGroupQuestion, TemplateQuestion } from '../template-edit.model';
import { RadioGroupEditorComponent } from './radio-group-editor/radio-group-editor.component';
import { MatrixEditorComponent } from './matrix-editor/matrix-editor.component';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [RadioGroupEditorComponent, MatrixEditorComponent],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
})
export class QuestionComponent {
  @Input({ required: true }) item!: TemplateQuestion;
  @Output() itemChange = new EventEmitter<TemplateQuestion>();

  @Input() index = 0;
  @Input() expanded = false;

  @Output() toggle = new EventEmitter<void>();
  @Output() delete = new EventEmitter<void>();
  onToggle() {
    this.toggle.emit();
  }

  onDelete(event: MouseEvent) {
    event.stopPropagation();
    this.delete.emit();
  }


  // ✅ When editor updates radiogroup, push it into itemChange
  onRadioGroupChange(updated: RadioGroupQuestion) {
    this.itemChange.emit(updated);
  }

  get typeLabel(): string {
    switch (this.item.type) {
      case 'radiogroup':
        return 'Radio group';
      case 'matrix':
        return 'Single choice matrix';
      case 'rating':
        return 'Rating';
      default:
        return 'Unknown';
    }
  }
}
