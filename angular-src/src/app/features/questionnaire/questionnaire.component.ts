import { Component, computed, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { QuestionComponent } from './question/question.component';
import { AnswerService } from './services/answer.service';
import { Answer, AnswerSubmission, QuestionnaireState } from './models/answer.model';
import { LoadingComponent } from '../../shared/loading/loading.component';
import { Role, User } from '../../shared/models/user.model';
import { AuthService } from '../../core/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SubmitConfirmDialog } from './submit-confirm-modal/SubmitConfirmDialog.component';
import { finalize, map, Observable } from 'rxjs';
import { QuestionnaireSessionService } from '../../core/services/questionnaire-session.service';
import { QuestionnaireConfirmDialog } from './confirm-dialog/QuestionnaireConfirmDialog.component';


/**
 * Questionnaire component.
 *
 * Presents and submits an active questionnaire for the current user.
 *
 * Handles:
 * - Loading questionnaire by route id.
 * - Submitting answers when all questions are completed.
 */
@Component({
  selector: 'app-answer-questionnaire',
  imports: [CommonModule, QuestionComponent, TranslateModule],
  templateUrl: './questionnaire.component.html',
  styleUrls: ['./questionnaire.component.css']
})
export class QuestionnaireComponent {
  private answerService = inject(AnswerService);
  private authService = inject(AuthService)
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private translate = inject(TranslateService);
  private questionnaireSessionService = inject(QuestionnaireSessionService);

  // guard against constant opening of submit dialog

  private isSubmitDialogOpen = false;
  private isSubmitting = false;
  private currentQuestionnaireId: string | null = null;
  private allowNavigationWithoutPrompt = false;
  private isLeaveDialogOpen = false;

  readonly user = this.authService.user;

  state: QuestionnaireState = {
    template: {
      id: '',
      title: '',
      description: '',
      questions: [],
      activatedAt: new Date(),
    },
    currentQuestionIndex: 0,
    answers: [],
    progress: 0,
    isCompleted: false,
  };

  isLoading = true;
  errorMessage: string | null = null;

  @HostListener('window:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent): void {
    // Don't handle keyboard events if user is typing in a textarea
    const target = event.target as HTMLElement;
    if (target.tagName === 'TEXTAREA') {
      return;
    }

    switch (event.key) {
      case 'ArrowRight':
      case 'Enter':
        event.preventDefault();
        if (this.state.currentQuestionIndex === this.state.template.questions.length - 1) {
          // Last question - submit if all answered
          if (this.allQuestionsAnswered) {
            this.submitQuestionnaire();
          }
        } else if (this.isAnswered) {
          // Not last question - go to next if current is answered
          this.nextQuestion();
        }
        break;

      case 'ArrowLeft':
      case 'Backspace':
        event.preventDefault();
        this.previousQuestion();
        break;

      case 'ArrowUp':
        event.preventDefault();
        this.selectPreviousOption();
        break;

      case 'ArrowDown':
        event.preventDefault();
        this.selectNextOption();
        break;

      case 'Escape':
        event.preventDefault();
        this.openLeaveQuestionnaireDialog();
        break;
    }
  }



  ngOnInit() {
    const u = this.user();
    if (!u) { this.router.navigate(['/'], { replaceUrl: true }); return; }

    this.route.paramMap
      .subscribe((pm) => {
        const questionnaireId = pm.get('id');
        if (questionnaireId) {
          this.loadQuestionnaire(questionnaireId);
        } else {
          console.error('No questionnaire ID found in route!');
        }
      });
  }
  /**
 * Verifies whether the user already submitted the questionnaire;
 * loads details if not, otherwise navigates home.
 */
  private loadQuestionnaire(id: string) {
    this.isLoading = true;
    this.currentQuestionnaireId = id;
    // First, check if the user has already submitted the questionnaire
    this.answerService.getActiveQuestionnaireById(id)
      .subscribe({
        next: (template) => {
          this.state.template = template;
          this.state.answers = [];
          this.state.currentQuestionIndex = 0;
          this.state.progress = 0;
          this.state.isCompleted = false;
          this.tryRestoreSavedSession(id);
          this.isLoading = false;
        },
        error: (error) => {
          if (error.status === 403) {
            this.router.navigate(['/']);
            return;
          }

          this.errorMessage =
            'An error occurred while loading the questionnaire.';
          this.isLoading = false;
        }
      });
  }

  /** The question at the current index. */
  get currentQuestion() {
    return this.state.template.questions[this.state.currentQuestionIndex];
  }

  /** The saved answer for the current question (if any). */
  get currentAnswer(): Answer | undefined {
    return this.state.answers.find(
      (a) => a.questionId === this.currentQuestion.id
    );
  }

  /** True if every question has either an option selected or a non-empty custom answer. */
  get allQuestionsAnswered(): boolean {
    return this.state.template.questions.every((question) =>
      this.state.answers.some(
        (answer) =>
          answer.questionId === question.id &&
          (!!answer.optionId || !!answer.customAnswer?.trim())
      )
    );
  }

  get isAnswered(): boolean {
    const answer = this.currentAnswer;
    return !!answer && (!!answer.optionId || !!answer.customAnswer?.trim());
  }

  /** True if the current question has an answer (option or non-empty custom text). */
  onAnswerChange(answer: Answer): void {
    const existingIndex = this.state.answers.findIndex(
      (a) => a.questionId === answer.questionId
    );
    if (existingIndex > -1) {
      this.state.answers[existingIndex] = answer;
    } else {
      this.state.answers.push(answer);
    }
    this.updateProgress();
    this.persistSession();
  }

  /** Moves to the previous question and updates progress. */
  previousQuestion(): void {
    if (this.state.currentQuestionIndex > 0) {
      this.state.currentQuestionIndex--;
      this.updateProgress();
      this.persistSession();
    }
  }

  /** Moves to the next question and updates progress. */
  nextQuestion(): void {
    if (
      this.state.currentQuestionIndex <
      this.state.template.questions.length - 1
    ) {
      this.state.currentQuestionIndex++;
      this.updateProgress();
      this.persistSession();
    }
  }

  /**
 * Submits all answers when the questionnaire is complete.
 * On success, marks as completed and navigates home.
 */
  submitQuestionnaire(): void {
    if (!this.allQuestionsAnswered) {
      alert('Please answer all questions before submitting.');
      return;
    }
    if (this.isSubmitDialogOpen || this.isSubmitting || this.state.isCompleted) {
      return;
    }
    this.openSubmitConfirmDialog();
  }

  openSubmitConfirmDialog(): void {
    if (this.isSubmitDialogOpen || this.isSubmitting) {
      return;
    }
    this.isSubmitDialogOpen = true;
    this.dialog
      .open(SubmitConfirmDialog, {
        panelClass: 'app-modal',
        maxWidth: '28rem',
        width: '100%',
        disableClose: true,
      })
      .afterClosed()
      .subscribe(confirmed => {
        this.isSubmitDialogOpen = false;
        if (confirmed) {
          this.performSubmit();
        }
      });
  }

  private performSubmit(): void {
    if (this.isSubmitting || this.state.isCompleted) {
      return;
    }
    this.isSubmitting = true;
    const submission: AnswerSubmission = { answers: this.state.answers };
    this.answerService.submitAnswers(this.state.template.id, submission).subscribe({
      next: () => {
        this.state.isCompleted = true;
        this.clearCurrentSession();
        this.snackBar.open(
          this.translate.instant('QUESTIONNAIRE.SUBMIT_SUCCESS'),
          this.translate.instant('COMMON.BUTTONS.CLOSE'),
          {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: ['success-snackbar']
          }
        );
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Error submitting questionnaire:', error);
        this.isSubmitting = false;
        this.snackBar.open(
          this.translate.instant('QUESTIONNAIRE.SUBMIT_ERROR'),
          this.translate.instant('COMMON.BUTTONS.CLOSE'),
          {
            duration: 8000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
          }
        );
      }
    });
  }


  /** Recomputes progress percentage based on index and whether the current question is answered. */
  private updateProgress(): void {
    const currentQuestionAnswered = this.isAnswered ? 1 : 0;
    const totalQuestions = this.state.template.questions.length;
    const progressForCurrent = (this.state.currentQuestionIndex / totalQuestions) * 100;
    const progressForAnswer = currentQuestionAnswered * (100 / totalQuestions);
    this.state.progress = Math.min(progressForCurrent + progressForAnswer, 100);
  }

  private tryRestoreSavedSession(questionnaireId: string): void {
    const user = this.user();
    if (!user) {
      return;
    }

    const savedSession = this.questionnaireSessionService.getSession(questionnaireId, user.id);
    if (!savedSession || savedSession.answers.length === 0) {
      this.updateProgress();
      return;
    }

    this.dialog
      .open(QuestionnaireConfirmDialog, {
        panelClass: 'app-modal',
        maxWidth: '28rem',
        width: '100%',
        disableClose: true,
        data: {
          titleKey: 'QUESTIONNAIRE.RESUME_TITLE',
          textKey: 'QUESTIONNAIRE.RESUME_TEXT',
          confirmKey: 'QUESTIONNAIRE.RESUME_CONFIRM',
          cancelKey: 'QUESTIONNAIRE.RESUME_CANCEL'
        }
      })
      .afterClosed()
      .subscribe(shouldRestore => {
        if (!shouldRestore) {
          this.questionnaireSessionService.removeSession(questionnaireId, user.id);
          this.updateProgress();
          return;
        }

        const validQuestionIds = new Set(this.state.template.questions.map(question => question.id));
        this.state.answers = savedSession.answers.filter(answer => validQuestionIds.has(answer.questionId));
        this.state.currentQuestionIndex = Math.max(
          0,
          Math.min(savedSession.currentQuestionIndex, this.state.template.questions.length - 1)
        );
        this.updateProgress();
      });
  }

  private openLeaveQuestionnaireDialog(): void {
    if (!this.isLeaveDialogOpen) {
      this.askLeaveConfirmation().subscribe(shouldLeave => {
        if (shouldLeave) {
          this.allowNavigationWithoutPrompt = true;
          this.router.navigate(['/']);
        }
      });
    }
  }

  canDeactivate(): boolean | Observable<boolean> {
    if (this.allowNavigationWithoutPrompt || this.state.isCompleted) {
      return true;
    }

    return this.askLeaveConfirmation();
  }

  private askLeaveConfirmation(): Observable<boolean> {
    this.isLeaveDialogOpen = true;
    return this.dialog
      .open(QuestionnaireConfirmDialog, {
        panelClass: 'app-modal',
        maxWidth: '28rem',
        width: '100%',
        disableClose: true,
        data: {
          titleKey: 'QUESTIONNAIRE.LEAVE_TITLE',
          textKey: 'QUESTIONNAIRE.LEAVE_TEXT',
          confirmKey: 'QUESTIONNAIRE.LEAVE_CONFIRM',
          cancelKey: 'COMMON.BUTTONS.CANCEL'
        }
      })
      .afterClosed()
      .pipe(map(result => !!result), finalize(() => this.isLeaveDialogOpen = false));
  }

  private persistSession(): void {
    if (this.state.isCompleted || !this.currentQuestionnaireId) {
      return;
    }

    const user = this.user();
    if (!user) {
      return;
    }

    this.questionnaireSessionService.saveSession(
      this.currentQuestionnaireId,
      user.id,
      this.state.currentQuestionIndex,
      this.state.answers
    );
  }

  private clearCurrentSession(): void {
    if (!this.currentQuestionnaireId) {
      return;
    }

    const user = this.user();
    if (!user) {
      return;
    }

    this.questionnaireSessionService.removeSession(this.currentQuestionnaireId, user.id);
  }

  /** Select previous option in current question (wraps around) */
  private selectPreviousOption(): void {
    const question = this.currentQuestion;
    if (!question || question.options.length === 0) return;

    const currentAnswer = this.currentAnswer;
    let currentIndex = -1;

    if (currentAnswer?.optionId) {
      currentIndex = question.options.findIndex(opt => opt.id === currentAnswer.optionId);
    }

    // Move to previous option (wrap around to last if at first)
    const newIndex = currentIndex <= 0
      ? question.options.length - 1
      : currentIndex - 1;

    const selectedOption = question.options[newIndex];

    // Reuse onAnswerChange
    this.onAnswerChange({
      questionId: question.id,
      optionId: selectedOption.id,
      customAnswer: undefined
    });
  }

  /** Select next option in current question (wraps around) */
  private selectNextOption(): void {
    const question = this.currentQuestion;
    if (!question || question.options.length === 0) return;

    const currentAnswer = this.currentAnswer;
    let currentIndex = -1;

    if (currentAnswer?.optionId) {
      currentIndex = question.options.findIndex(opt => opt.id === currentAnswer.optionId);
    }

    // Move to next option (wrap around to first if at last)
    const newIndex = currentIndex >= question.options.length - 1
      ? 0
      : currentIndex + 1;

    const selectedOption = question.options[newIndex];

    // Reuse onAnswerChange
    this.onAnswerChange({
      questionId: question.id,
      optionId: selectedOption.id,
      customAnswer: undefined
    });
  }
  /**
 * Returns collaborator display text based on the viewer's role:
 * - Student sees teacher, teacher sees student.
 */
  getCollaboratorInfo(): string | null {
    const user = this.user();
    const role = user?.role;
    const q = this.state.template;
    const student = q?.student;
    const teacher = q?.teacher;

    if (!student || !teacher || !role) return null;

    switch (role) {
      case Role.Student:
        return `${teacher.fullName} (${teacher.userName})`;
      case Role.Teacher:
        return `${student.fullName} (${student.userName})`;
      case Role.Admin:
        return `Student: ${student.fullName} (${student.userName}), Teacher: ${teacher.fullName} (${teacher.userName})`;
      default:
        return null;
    }
  }
}