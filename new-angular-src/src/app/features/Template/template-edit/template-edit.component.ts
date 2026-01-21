import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { QuestionComponent } from './question/question.component';
import { QuestionType, TemplateQuestion } from './template-edit.model';

@Component({
  selector: 'app-template-edit',
  imports: [FormsModule, QuestionComponent],
  templateUrl: './template-edit.component.html',
  styleUrls: ['./template-edit.component.css'],
})
export class TemplateEditComponent {
  expandedIndex: number | null = null; // example: open question 2 by default

  questions: TemplateQuestion[] = [
    {
      id: 'q1',
      type: QuestionType.RadioGroup,
      prompt: 'What is your primary focus area?',
      options: ['Option A', 'Option B', 'Option C'], // ✅ string[]
      allowOtherComment: true,
      otherLabel: 'Other (describe)',
    },
    {
      id: 'q2',
      type: QuestionType.Rating,
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
      type: QuestionType.MatrixSingle,
      prompt: 'Evaluate the student’s skills',
      rows: ['Indlæringsevne', 'Arbejdsindsats', 'Orden og omhyggelighed'],
      columns: ['Low', 'Medium', 'High'],
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
