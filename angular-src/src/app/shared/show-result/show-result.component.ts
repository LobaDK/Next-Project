import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Result } from '../models/result.model';
import type { AgCartesianChartOptions } from 'ag-charts-community';
import { AgCharts } from 'ag-charts-angular';
import { ResultHistoryService } from '../../features/result-history/services/result-history.service';
import { AuthService } from '../../core/services/auth.service';



export interface ShowResultConfig {
  showTemplate?: boolean;
  showStudent?: boolean;
  showTeacher?: boolean;
  showCompletionDates?: boolean;
  customHeaderTitle?: string;
  hideTemplateName?: boolean;
  showActions?: boolean;
  useCardStyling?: boolean;
  showTestButtons?: boolean;
}

@Component({
  selector: 'app-show-result',
  standalone: true,
  imports: [CommonModule, TranslateModule, AgCharts],
  templateUrl: './show-result.component.html',
  styleUrl: './show-result.component.css'
})
export class ShowResultComponent {
  @Input() result: Result | null = null;
  @Input() isFullView: boolean = false;
  @Input() config: ShowResultConfig = {};
  @Input() studentId?: string;
  @Input() templateId?: string;
  

  private resultHistoryService = inject(ResultHistoryService);
  private authService = inject(AuthService);

  public showGraphOverlay = false;
  public currentQuestionIndex = 0;
  public allQuestions: any[] = [];
  public allResponses: any[] = [];
  public answers: string[] = [];
  public chartOptions: AgCartesianChartOptions = {
    title: { text: '' },
    data: [],
    series: [],
    axes: []
  };


  constructor(public translate: TranslateService) {}

  getTemplateQuestionOptions(questionPrompt: string): any[] {
    if (!this.result) return [];
    const answer = this.result.answers.find(a => a.question === questionPrompt);
    return answer?.options?.slice(0, 15) || [];
  }

  isOptionSelected(response: string, isCustom: boolean, option: any, index: number, role?: 'student' | 'teacher'): boolean {
    if (isCustom || !response) return false;
    
    if (role === 'student' && option.isSelectedByStudent !== undefined) {
      return option.isSelectedByStudent;
    }
    if (role === 'teacher' && option.isSelectedByTeacher !== undefined) {
      return option.isSelectedByTeacher;
    }
    
    if (response === option.displayText) return true;
    if (response === option.optionValue?.toString()) return true;
    if (response === (index + 1).toString()) return true;
    if (response === index.toString()) return true;
    
    if (response?.toLowerCase() === option.displayText?.toLowerCase()) return true;
    
    return false;
  }

  public showResultGraph(questionIndex: number) {
    if (!this.result) return;

    this.currentQuestionIndex = questionIndex;
    this.showGraphOverlay = true;
    
    const activeQuestionnaireId = this.result.id;
    const teacherId = this.result.teacher?.user?.id ?? this.authService.user()?.id;
    if (!teacherId) {
      console.error('Teacher ID is not available');
      this.createChartFromCurrentResult();
      return;
    }

    // Try to get student ID from Input, then from result object
    let studentIdToUse = this.studentId || this.result.student?.user?.id;
    
    if (!studentIdToUse) {
      console.warn('Student ID not available, showing current result only');
      this.createChartFromCurrentResult();
      return;
    }
   
       // Use templateId from Input if available, otherwise fetch it
    if (this.templateId && this.templateId !== '00000000-0000-0000-0000-000000000000') {
      console.log('Using provided templateId:', this.templateId);
      this.resultHistoryService.getQuestionnaireOptionsByID(this.templateId).subscribe({
        next: (res) => {
          if (res?.questions?.length) {
            this.allQuestions = res.questions;
          }
          this.loadGraphResponses(this.templateId!, studentIdToUse!, teacherId);
        },
        error: (err) => {
          console.error('Error loading questions:', err);
          this.createChartFromCurrentResult();
        }
      });
    } else {
      // Fallback to fetching questionnaire data
      this.resultHistoryService.getQuestionnaireDataByID(activeQuestionnaireId).subscribe({
        next: (questionnaireData) => {
          console.log('Questionnaire data received:', questionnaireData);
          
          if (questionnaireData?.questions?.length) {
            this.allQuestions = questionnaireData.questions;
          } else {
            console.warn('No questions found in questionnaire data');
            this.createChartFromCurrentResult();
            return;
          }
          
          const templateId = questionnaireData.templateId;
          
          // Check if templateId is valid
          if (!templateId || templateId === '00000000-0000-0000-0000-000000000000') {
            console.warn('Invalid templateId from questionnaire, using current result only');
            this.createChartFromCurrentResult();
            return;
          }
          
          console.log('Loading graph with:', { templateId, studentId: studentIdToUse, teacherId });
          this.loadGraphResponses(templateId, studentIdToUse!, teacherId);
        },
        error: (err) => {
          console.error('Error fetching questionnaire data:', err);
          this.createChartFromCurrentResult();
        }
      });
    }
  }
  
  
  public closeGraphOverlay() {
    this.showGraphOverlay = false;
  }
  
  
  private loadGraphResponses(templateId: string, studentId: string, teacherId: string) {
    this.resultHistoryService.getResponsesByID(studentId, teacherId, templateId).subscribe({
      next: (res) => {
        console.log('Raw API responses:', res);
        this.allResponses = Array.isArray(res) ? res : [];
        this.updateChartForQuestion(this.currentQuestionIndex);
      },
      error: (err) => console.error('Error fetching responses:', err),
    });
  }


  // New method: load responses using the active questionnaire ID directly
  private loadGraphResponsesOnly(activeQuestionnaireId: string, studentId: string, teacherId: string) {
    // Modified to work with active questionnaire instead of template
    this.resultHistoryService.getResponsesByID(studentId, teacherId, activeQuestionnaireId).subscribe({
      next: (res) => {
        console.log('Raw API responses:', res);
        this.allResponses = Array.isArray(res) ? res : [];
        this.updateChartForQuestion(this.currentQuestionIndex);
      },
      error: (err) => {
        console.error('Error fetching responses:', err);
        // If that doesn't work, create a chart with just the current data
        console.warn('Unable to load historical responses, showing current response only');
        this.createChartFromCurrentResult();
      },
    });
  }

  // Fallback: create chart from current result data when no historical data is available
  private createChartFromCurrentResult() {
    if (!this.result) return;
    
    const answer = this.result.answers[this.currentQuestionIndex];
    if (!answer) return;

    const options = this.getTemplateQuestionOptions(answer.question);
    this.answers = options.map(opt => opt.displayText);

    const toNumeric = (text: string) => {
      const idx = this.answers.indexOf(text);
      return idx !== -1 ? idx : null;
    };

    const singleDataPoint = {
      date: this.result.student.completedAt,
      dateLabel: this.result.student.completedAt.toLocaleString('da-DK', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      }),
      TeacherAnswer: toNumeric(answer.teacherResponse),
      StudentAnswer: toNumeric(answer.studentResponse),
      index: 0,
      displayLabel: `${this.result.student.completedAt.toLocaleString('da-DK', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      })} (#1)`
    };

    this.chartOptions = {
      title: { text: answer.question },
      data: [singleDataPoint],
      series: [
        {
          type: 'line',
          xKey: 'index',
          yKey: 'TeacherAnswer',
          yName: 'Lærer',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
        {
          type: 'line',
          xKey: 'index',
          yKey: 'StudentAnswer',
          yName: 'Elev',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
      ],
      axes: [
        {
          type: 'category',
          position: 'bottom',
          title: { text: 'Tidspunkt for besvarelse' },
          label: {
            rotation: 0,
            formatter: (params) => singleDataPoint.dateLabel
          }
        },
        {
          type: 'number',
          position: 'left',
          min: 0,
          max: this.answers.length - 1,
          nice: false,
          label: {
            avoidCollisions: false,
            formatter: (p) => {
              const v = p.value;
              if (Math.abs(v - Math.round(v)) > 1e-6) return '';
              const idx = Math.round(v);
              const text = this.answers[idx] ?? '';
              return text.length > 25 ? text.substring(0, 22) + '...' : text;
            },
          },
          tick: {
            // @ts-expect-error: supported at runtime
            step: 1,
          },
          title: { text: 'Svarmuligheder' },
        },
      ],
    };
  }

 private updateChartForQuestion(index: number) {
    const question = this.allQuestions[index];
    console.log('Updating chart for question:', question?.text ?? question);

    if (!question || !this.result) return;

    // Get the corresponding answer from result to match question
    const resultAnswer = this.result.answers[index];
    
    this.answers = (question.options ?? [])
      .map((opt: any) => opt.displayText)
      .filter((t: string) => !!t);
    console.log('Possible answers:', this.answers);

    const toNumeric = (text: string) => {
      const idx = this.answers.indexOf(text);
      return idx !== -1 ? idx : null;
    };

    const filtered = this.allResponses
      .flatMap((entry) => {
        const match = entry.answers?.filter(
          (a: any) =>
            a.question === question.prompt ||
            a.question === question.title ||
            a.question === question.text ||
            a.question === resultAnswer.question
        );
        return match?.map((a: any) => {
          const completedDate = new Date(
            entry.student?.completedAt ?? entry.teacher?.completedAt ?? Date.now()
          );
          return {
            date: completedDate,
            dateLabel: completedDate.toLocaleString('da-DK', {
              year: 'numeric',
              month: 'short',
              day: 'numeric',
            }),
            TeacherAnswer: toNumeric(a.teacherResponse),
            StudentAnswer: toNumeric(a.studentResponse),
          };
        });
      })
      .filter((x) => x && (x.TeacherAnswer !== null || x.StudentAnswer !== null))
      .sort((a, b) => a.date.getTime() - b.date.getTime());

    const indexedData = filtered.map((item, idx) => ({
      ...item,
      index: idx,
      displayLabel: `${item.dateLabel} (#${idx + 1})`
    }));

    console.log('Filtered chart data:', indexedData);

    this.chartOptions = {
      title: { text: resultAnswer.question || question.prompt || question.title || question.text || 'Spørgsmål' },
      data: indexedData,
      series: [
        {
          type: 'line',
          xKey: 'index',
          yKey: 'TeacherAnswer',
          yName: 'Lærer',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
        {
          type: 'line',
          xKey: 'index',
          yKey: 'StudentAnswer',
          yName: 'Elev',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
      ],
      axes: [
        {
          type: 'category',
          position: 'bottom',
          title: { text: 'Tidspunkt for besvarelse' },
          label: {
            rotation: 0,
            formatter: (params) => {
              const dataPoint = indexedData[params.value];
              return dataPoint ? dataPoint.dateLabel : '';
            }
          }
        },
        {
          type: 'number',
          position: 'left',
          min: 0,
          max: this.answers.length - 1,
          nice: false,
          label: {
            avoidCollisions: false,
            formatter: (p) => {
              const v = p.value;
              if (Math.abs(v - Math.round(v)) > 1e-6) return '';
              const idx = Math.round(v);
              const text = this.answers[idx] ?? '';
              return text.length > 25 ? text.substring(0, 22) + '...' : text;
            },
          },
          tick: {
            // @ts-expect-error: supported at runtime
            step: 1,
          },
          title: { text: 'Svarmuligheder' },
        },
      ],
    };
  }
}
