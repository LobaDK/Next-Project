import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RadioGroupQuestion } from '../../template-edit.model';

@Component({
  selector: 'app-radio-group-editor',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './radio-group-editor.component.html',
})
export class RadioGroupEditorComponent {
  @Input({ required: true }) question!: RadioGroupQuestion;
  @Output() questionChange = new EventEmitter<RadioGroupQuestion>();

  readonly MAX_OPTIONS = 10;

  get canAddOption(): boolean {
    return this.question.options.length < this.MAX_OPTIONS;
  }

  private patch(patch: Partial<RadioGroupQuestion>) {
    this.questionChange.emit({ ...this.question, ...patch });
  }

  updatePrompt(prompt: string) {
    this.patch({ prompt });
  }

  updateOption(index: number, label: string) {
    const options = this.question.options.map((o, i) =>
      i === index ? label : o
    );
    this.patch({ options });
  }

  addOption() {
    if (!this.canAddOption) return;

    const nextIndex = this.question.options.length + 1;

    this.patch({
      options: [...this.question.options, `Option ${nextIndex}`],
    });
  }

  removeOption(index: number) {
    const options = this.question.options.filter((_, i) => i !== index);
    this.patch({ options });
  }

  toggleOther(enabled: boolean) {
    this.patch({
      allowOtherComment: enabled,
      otherLabel: enabled ? (this.question.otherLabel ?? 'Other') : null,
    });
  }

  updateOtherLabel(otherLabel: string) {
    this.patch({ otherLabel });
  }
}
