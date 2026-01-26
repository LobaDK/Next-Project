import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { QuestionComponent } from './question/question.component';
import {
  QuestionType,
  TemplateQuestion,
  QuestionnaireTemplateEditor,
  TemplateStatus,
} from '../../../shared/models/template-edit.model';
import { QuestionaireConversionService } from '../../../shared/services/questionaire-conversion.service';
// survey.component.ts
import "survey-core/survey-core.min.css";
import { LayeredDarkPanelless, LayeredLight } from "survey-core/themes";
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';


// SurveyJS
import { Model } from 'survey-core';
import { SurveyModule } from 'survey-angular-ui';
import { QUESTIONNAIRE_LIMITS } from '../../../shared/constants/questionnaire-limits';
import { brandSurveyTheme } from '../../../shared/constants/survey-theme';
import { QuestionnaireTemplateEditorMock } from './mock/mock-data';

@Component({
  selector: 'app-template-edit',
  standalone: true,
  imports: [FormsModule, QuestionComponent, SurveyModule, DragDropModule],
  templateUrl: './template-edit.component.html',
  styleUrls: ['./template-edit.component.css'],
})
export class TemplateEditComponent {
  expandedIndex: number | null = null;

  showPreview = false;
  surveyModel: Model | null = null;
  readonly LIMITS = QUESTIONNAIRE_LIMITS;
  

  editor: QuestionnaireTemplateEditor = QuestionnaireTemplateEditorMock;

  constructor(private conversion: QuestionaireConversionService) {}

  get questions(): TemplateQuestion[] {
    return this.editor.questions;
  }

  toggleExpanded(index: number) {
    this.expandedIndex = this.expandedIndex === index ? null : index;
  }

  deleteQuestion(index: number) {
    this.editor.questions = this.editor.questions.filter((_, i) => i !== index);

    if (this.expandedIndex === index) this.expandedIndex = null;
    if (this.expandedIndex !== null && this.expandedIndex > index) {
      this.expandedIndex--;
    }

    // refresh preview if open
    this.refreshPreview();
  }

  addQuestion() {
    const nextId = `q${this.editor.questions.length + 1}`;

    this.editor.questions = [
      ...this.editor.questions,
      {
        id: nextId,
        type: QuestionType.RadioGroup,
        prompt: 'New question',
        options: [
          { value: 1, label: 'Option 1' },
          { value: 2, label: 'Option 2' },
        ],
        allowOtherComment: false,
        otherLabel: null,
      },
    ];

    this.expandedIndex = this.editor.questions.length - 1;

    // refresh preview if open
    this.refreshPreview();
  }

  togglePreview() {
    this.showPreview = !this.showPreview;

    if (this.showPreview) {
      this.refreshPreview(true);
    }
  }

  refreshPreview(force = false) {
    if (!this.showPreview && !force) return;

    const surveyJson = this.conversion.toSurveyJsJson(this.editor);
    console.log("SurveyJS JSON:", surveyJson);

    this.surveyModel = new Model(surveyJson);
    this.surveyModel.applyTheme(brandSurveyTheme);

    // optional: capture preview results
    this.surveyModel.onComplete.add((sender) => {
      console.log('Preview results:', sender.data);
    });
  }

  finalize() {
    // WIP
    this.editor.status = TemplateStatus.Finalized;
    console.log('Finalized template:', this.editor);

    this.refreshPreview();
  }

  saveQuestionnaire() {
    console.log('Saved template:', this.editor);
  }

dropQuestion(event: CdkDragDrop<any[]>) {
  if (event.previousIndex === event.currentIndex) return;

  // keep track of which question was expanded BEFORE move
  const expandedQuestion =
    this.expandedIndex !== null ? this.questions[this.expandedIndex] : null;

  moveItemInArray(this.questions, event.previousIndex, event.currentIndex);

  // restore expanded index to the same question (new position)
  if (expandedQuestion) {
    this.expandedIndex = this.questions.findIndex(q => q.id === expandedQuestion.id);
  }
}
}
