import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RadioGroupQuestion } from '../../../../../shared/models/template-edit.model';

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

  updateOptionLabel(index: number, label: string) {
    const options = this.question.options.map((o, i) =>
      i === index ? { ...o, label } : o
    );
    this.patch({ options });
  }

  // If you want to allow editing the numeric value too (optional)
  updateOptionValue(index: number, value: number) {
    if (!Number.isFinite(value)) return;

    const options = this.question.options.map((o, i) =>
      i === index ? { ...o, value } : o
    );
    this.patch({ options });
  }

  private getNextOptionValue(): number {
    // safest: take max value + 1
    const maxValue = Math.max(...this.question.options.map(o => o.value), 0);
    return maxValue + 1;
  }

  addOption() {
    if (!this.canAddOption) return;

    const nextValue = this.getNextOptionValue();
    const nextIndex = this.question.options.length + 1;

    this.patch({
      options: [
        ...this.question.options,
        { value: nextValue, label: `Option ${nextIndex}` }
      ],
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

  moveOption(index: number, direction: -1 | 1) {
    const newIndex = index + direction;

    if (newIndex < 0 || newIndex >= this.question.options.length) return;

    const options = [...this.question.options];

    // swap positions
    [options[index], options[newIndex]] = [options[newIndex], options[index]];

    // re-number values so value = order (1..N)
    const renumbered = options.map((opt, i) => ({
      ...opt,
      value: i + 1
    }));

    this.patch({ options: renumbered });
  }


}