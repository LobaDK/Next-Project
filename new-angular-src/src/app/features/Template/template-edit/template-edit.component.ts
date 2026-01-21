import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { QuestionComponent } from './question/question.component';
import { TemplateQuestion } from './template-edit.model';

@Component({
  selector: 'app-template-edit',
  imports: [FormsModule, QuestionComponent],
  templateUrl: './template-edit.component.html',
  styleUrls: ['./template-edit.component.css'],
})
export class TemplateEditComponent {
  expandedIndex: number | null = 1; // example: open question 2 by default

  questions: TemplateQuestion[] = [
    {
      id: 'q1',
      type: 'radiogroup',
      prompt: 'What is your primary focus area?',
      options: [
        { value: 'a', label: 'Option A' },
        { value: 'b', label: 'Option B' },
        { value: 'c', label: 'Option C' },
      ],
      allowOtherComment: true,
      otherLabel: 'Other (describe)',
    },
    {
      id: 'q2',
      type: 'rating',
      prompt: 'How would you rate the student’s overall performance?',
      scale: [
        { value: 1, label: 'Poor' },
        { value: 2, label: 'Below average' },
        { value: 3, label: 'Average' },
        { value: 4, label: 'Good' },
        { value: 5, label: 'Excellent' },
      ],
    },
    {
      id: 'q3',
      type: 'matrix_single',
      prompt: 'Evaluate the student’s skills',
      rows: [
        { value: 'r1', label: 'Indlæringsevne' },
        { value: 'r2', label: 'Arbejdsindsats' },
        { value: 'r3', label: 'Orden og omhyggelighed' },
      ],
      columns: [
        { value: 'c1', label: 'Low' },
        { value: 'c2', label: 'Medium' },
        { value: 'c3', label: 'High' },
      ],
    },
  ];

  toggleExpanded(index: number) {
    this.expandedIndex = this.expandedIndex === index ? null : index;
  }

  deleteQuestion(index: number) {
    // mock only
    console.log('Delete question:', this.questions[index]);
  }

  editQuestion(index: number) {
    // mock only
    console.log('Edit question:', this.questions[index]);
  }
}
