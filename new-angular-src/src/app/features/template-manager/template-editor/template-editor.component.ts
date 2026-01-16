import { Component, EventEmitter, Input, Output, OnChanges, inject } from '@angular/core';
import { QuestionEditorComponent } from './question-editor/question-editor.component';
import { Question, Template, TemplateStatus } from '../../../shared/models/template.model';

import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { TemplateService } from '../services/template.service';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { DuplicateWarningDialog } from './dialog/duplicate-warning-dialog/DuplicateWarningDialog.component';
import { FinalizeConfirmDialog } from './dialog/finalize-confirm-dialog/FinalizeConfirmDialog.component';


/**
 * Template editor component.
 *
 * Provides an interface for editing a single questionnaire template.
 *
 * Handles:
 * - Displaying and updating template title/description.
 * - Adding, editing, and deleting questions.
 * - Saving or canceling template edits.
 * - Finalizing (publishing) a draft template.
 * - Switching to readonly mode if the template is finalized.
 */
@Component({
    selector: 'app-template-editor',
    standalone: true,
    imports: [QuestionEditorComponent, FormsModule, TranslateModule, DragDropModule, CommonModule],
    templateUrl: './template-editor.component.html',
    styleUrl: './template-editor.component.css'
})
export class TemplateEditorComponent implements OnChanges {
  /** Template being edited. */
  @Input() template!: Template;

  /** Emits when the template is saved. */
  @Output() saveTemplate = new EventEmitter<Template>();

  /** Emits when the draft is finalized. */
  @Output() finalizeDraft  = new EventEmitter<Template>();

  /** Emits when editing is canceled. */
  @Output() cancelEdit = new EventEmitter<void>();

  /** Currently selected question (if editing one). */
  selectedQuestion: Question | null = null;

  /** True if the template is finalized (readonly mode). */
  readonly = false;

  // Holds IDs of questions that have duplicate prompts
  duplicateQuestionIds: number[] = [];

  // Holds IDs of questions that contain options that are duplicated across the template
  duplicateOptionQuestionIds: number[] = [];


  /** Set to true when order changed via drag-drop; used to avoid auto-saving on drop */
  orderChanged = false;

  /** Title validation state */
  titleError: string | null = null;
  isTitleChecking = false;
  private titleCheckSubject = new Subject<string>();
  private templateService = inject(TemplateService);
  private dialog = inject(MatDialog);

  ngOnInit() {
    // Set up debounced title checking (1 second delay)
    this.titleCheckSubject
      .pipe(debounceTime(1000), distinctUntilChanged())
      .subscribe((title) => {
        this.checkTitleAvailability(title);
      });
  }

  ngOnChanges() {
    this.readonly = this.template.templateStatus === TemplateStatus.Finalized;
    
    // Ensure questions and options are sorted by sortOrder when template changes
    if (this.template && this.template.questions) {
      this.sortTemplateData();
    }

    // Reset title error when template changes
    this.titleError = null;
  }

  /** Sorts questions and options by their sortOrder values. */
  private sortTemplateData() {
    if (this.template.questions) {
      this.template.questions.sort((a, b) => a.sortOrder - b.sortOrder);
      this.template.questions.forEach(question => {
        if (question.options) {
          question.options.sort((a, b) => a.sortOrder - b.sortOrder);
        }
      });
    }
  }

  /**
   * Handles title input changes and triggers debounced validation.
   */
  onTitleChange(newTitle: string) {
    const trimmedTitle = newTitle?.trim();
    
    if (!trimmedTitle) {
      this.titleError = null;
      return;
    }

    // Emit to subject for debounced checking
    this.titleCheckSubject.next(trimmedTitle);
  }

  /**
   * Performs the actual title availability check against the API.
   */
  private checkTitleAvailability(title: string) {
    this.isTitleChecking = true;
    this.templateService.checkTitleAvailability(title).subscribe({
      next: (titleExists) => {
        this.isTitleChecking = false;
        if (titleExists) {
          this.titleError = 'TEMPLATE.EDITOR.TITLE_ALREADY_EXISTS';
        } else {
          this.titleError = null;
        }
      },
      error: (err) => {
        console.error('Error checking title availability:', err);
        this.isTitleChecking = false;
        this.titleError = null; // Don't show error to user, just log it
      }
    });
  }

  // Method to emit the saveTemplate event with the updated template
  onSave() {
    // Validate template for duplicates before emitting save
    const valid = this.validateTemplate();
    if (!valid) {
      // open dialog to inform user
      this.openDuplicateWarningDialog();
      return;
    }


    this.saveTemplate.emit(this.template);
  }

    /**
   * Validate the template for duplicates:
   * - No two questions may have the same prompt (case-insensitive, trimmed)
   * - No two options within the same question may have the same label (case-insensitive, trimmed)
   * Marks offending questions in duplicateQuestionIds and duplicateOptionQuestionIds.
   * Returns true if there are no duplicates, false otherwise.
   */
  validateTemplate(): boolean {
    this.duplicateQuestionIds = [];
    this.duplicateOptionQuestionIds = [];

    if (!this.template || !this.template.questions) {
      return true;
    }

    // Check question prompt duplicates
    const promptMap = new Map<string, number[]>();
    this.template.questions.forEach((q) => {
      const key = (q.prompt || "").trim().toLowerCase();
      const qid = q.id ?? -999999;
      if (!promptMap.has(key)) promptMap.set(key, []);
      promptMap.get(key)!.push(qid);
    });

    promptMap.forEach((ids, key) => {
      if (key !== "" && ids.length > 1) {
        this.duplicateQuestionIds.push(...ids);
      }
    });

    // Check option label duplicates WITHIN each question (not across questions)
    this.template.questions.forEach((q) => {
      const qid = q.id ?? -999999;
      const optionLabels = new Map<string, number>();
      
      q.options?.forEach((opt) => {
        const key = (opt.displayText || "").trim().toLowerCase();
        if (key !== "") {
          const count = optionLabels.get(key) || 0;
          optionLabels.set(key, count + 1);
        }
      });

      // If any label appears more than once in this question, mark this question as having duplicates
      const hasDuplicates = Array.from(optionLabels.values()).some(count => count > 1);
      if (hasDuplicates) {
        this.duplicateOptionQuestionIds.push(qid);
      }
    });

    return this.duplicateQuestionIds.length === 0 && this.duplicateOptionQuestionIds.length === 0;
  }

  isQuestionDuplicate(q: Question): boolean {
    const qid = q.id ?? -999999;
    return this.duplicateQuestionIds.includes(qid) || this.duplicateOptionQuestionIds.includes(qid);
  }



  
  // Method to emit the cancelEdit event
  onCancel() {
    this.cancelEdit.emit();
  }
  
  addQuestion() {
    const newQuestion: Question = {
      id: -1 * (this.template.questions.length + 1), // Unique negative ID
      prompt: 'New Question',
      allowCustom: true,
      sortOrder: this.template.questions.length, // Set sort order to be at the end
      options: [],
    };
  
    this.template.questions.push(newQuestion);
  }
  // Select a question for editing
  editQuestion(question: Question): void {
    // Toggle editor: if the same question is already open, close it and run cancel cleanup
    if (this.selectedQuestion && this.selectedQuestion.id === question.id) {
      this.onCancelEdit(); // ensures any cancel cleanup behavior is executed
      return;
    }

    // If another question was open, discard its edits first
    if (this.selectedQuestion && this.selectedQuestion.id !== question.id) {
      this.onCancelEdit();
    }

    // Open editor for the clicked question (work on a deep copy so edits don't mutate original until Save)
    // Use JSON clone because Question is a simple data object (id, prompt, options[])
    this.selectedQuestion = JSON.parse(JSON.stringify(question)) as Question;
  }

  // Save the edited question
  onSaveQuestion(updatedQuestion: Question): void {
    console.log(updatedQuestion)
    // Find the index of the question in the template
    const questionIndex = this.template.questions.findIndex(q => q.id === updatedQuestion.id);

    if (questionIndex > -1) {
      // Update the question in the template's questions array
      this.template.questions[questionIndex] = { ...updatedQuestion };
    }

    this.selectedQuestion = null; // Close the editor
  }

  // Cancel editing
  onCancelEdit(): void {
    this.selectedQuestion = null; // Close the editor
  }

  deleteQuestion(question: Question): void {
    this.template.questions = this.template.questions.filter(q => q.id !== question.id);

    // Re-index sortOrder for remaining questions
    this.template.questions.forEach((q, index) => {
      q.sortOrder = index;
    });
  }

  /**
   * Handle drop event from Angular CDK drag-and-drop.
   * Reorders the `template.questions` array locally and updates sort orders.
   */
  drop(event: CdkDragDrop<Question[]>) {
    // Update the array order in-place
    moveItemInArray(this.template.questions, event.previousIndex, event.currentIndex);

    // Update sortOrder for all questions to match the new array order
    this.template.questions.forEach((question, index) => {
      question.sortOrder = index;
    });

    // Mark that order changed but do NOT auto-emit saveTemplate here.
    // Emitting saveTemplate caused the parent to treat this as a full save
    // (and in your app that likely navigated back to the templates list).
    // Leave persistence to the user's explicit Save action.
    this.orderChanged = true;
  }
  
  onFinalize() { this.finalizeDraft.emit(this.template); }
  
  openDuplicateWarningDialog(): void {
    this.dialog.open(DuplicateWarningDialog, {
      panelClass: 'app-modal',
      maxWidth: '28rem',
      width: '100%',
      disableClose: false,
    });
  }
  
  openFinalizeDialog(): void {
    this.dialog
      .open(FinalizeConfirmDialog, {
        panelClass: 'app-modal',
        maxWidth: '28rem',
        width: '100%',
        disableClose: true,
        data: this.template,
      })
      .afterClosed()
      .subscribe(confirmed => {
        if (confirmed) {
          this.onFinalize();
        }
      });
  }

  /**
   * Get duplicate option IDs for a specific question.
   * Returns an array of option IDs that are duplicated within the same question only.
   * (Same options can exist in different questions, which is allowed)
   */
  getDuplicateOptionIdsForQuestion(question: Question): number[] {
    if (!question.options) {
      return [];
    }

    const duplicateIds: number[] = [];

    // Check for duplicates WITHIN this question only
    const labelCountInQuestion = new Map<string, number[]>();
    question.options.forEach((opt) => {
      const key = (opt.displayText || "").trim().toLowerCase();
      const optId = opt.id ?? -999999;
      if (!labelCountInQuestion.has(key)) labelCountInQuestion.set(key, []);
      labelCountInQuestion.get(key)!.push(optId);
    });

    // Mark options with duplicate labels within the same question
    labelCountInQuestion.forEach((ids, key) => {
      if (key !== "" && ids.length > 1) {
        duplicateIds.push(...ids);
      }
    });

    return duplicateIds;
  }

}
