import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SingleChoiceMatrixQuestion } from '../../../../../shared/models/template-edit.model';

@Component({
  selector: 'app-matrix-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './matrix-editor.component.html',
})
export class MatrixEditorComponent {
  @Input({ required: true }) question!: SingleChoiceMatrixQuestion;
  @Output() questionChange = new EventEmitter<SingleChoiceMatrixQuestion>();

  readonly MAX_ROWS = 10;
  readonly MAX_COLUMNS = 10;

  get canAddRow(): boolean {
    return this.question.rows.length < this.MAX_ROWS;
  }

  get canAddColumn(): boolean {
    return this.question.columns.length < this.MAX_COLUMNS;
  }

  private patch(patch: Partial<SingleChoiceMatrixQuestion>) {
    this.questionChange.emit({ ...this.question, ...patch });
  }

  updatePrompt(prompt: string) {
    this.patch({ prompt });
  }

  // --- Rows ---
  addRow() {
    if (!this.canAddRow) return;

    const nextIndex = this.question.rows.length + 1;
    this.patch({ rows: [...this.question.rows, `Row ${nextIndex}`] });
  }

  updateRow(index: number, value: string) {
    const rows = this.question.rows.map((r, i) => (i === index ? value : r));
    this.patch({ rows });
  }

  removeRow(index: number) {
    const rows = this.question.rows.filter((_, i) => i !== index);
    this.patch({ rows });
  }

  // --- Columns ---
  addColumn() {
    if (!this.canAddColumn) return;

    const nextIndex = this.question.columns.length + 1;
    this.patch({ columns: [...this.question.columns, `Column ${nextIndex}`] });
  }

  updateColumn(index: number, value: string) {
    const columns = this.question.columns.map((c, i) => (i === index ? value : c));
    this.patch({ columns });
  }

  removeColumn(index: number) {
    const columns = this.question.columns.filter((_, i) => i !== index);
    this.patch({ columns });
  }
}
