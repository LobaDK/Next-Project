import { Injectable } from "@angular/core";
import {
  QuestionnaireTemplateEditor,
  TemplateQuestion,
  QuestionType,
  RadioGroupQuestion
} from "../models/template-edit.model";

@Injectable({
  providedIn: "root",
})
export class QuestionaireConversionService {
  private static readonly OTHER_COMMENT_MAX_LEN = 500;

  // ----------------------------
  // Editor -> SurveyJS JSON
  // Each question = its own page
  // Each question required
  // ----------------------------
  toSurveyJsJson(editor: QuestionnaireTemplateEditor) {
    return {
      title: editor.title,
      description: editor.description ?? "",
      showProgressBar: "top",
      pages: editor.questions
        .map((q, index) => this.toSurveyJsPage(q, index))
        .filter(Boolean),
    };
  }

  private toSurveyJsPage(question: TemplateQuestion, index: number): any | null {
    const element = this.toSurveyJsElement(question);
    if (!element) return null;

    return {
      name: `page${index + 1}`,
      elements: [element],
    };
  }

  private toSurveyJsElement(question: TemplateQuestion): any | null {
    switch (question.type) {
      case QuestionType.RadioGroup: {
        const q = question as RadioGroupQuestion;

        const maxLen = QuestionaireConversionService.OTHER_COMMENT_MAX_LEN;

        const rtnQuestion: any = {
          type: "radiogroup",
          name: q.id,
          title: q.prompt,
          isRequired: true,
          choices: q.options.map(opt => ({
            value: opt.value,
            text: opt.label,
          })),
          hasOther: !!q.allowOtherComment,
          otherText: q.otherLabel ?? "Other",
          maxOthersLength: maxLen,
        };

        if (q.allowOtherComment) {
          rtnQuestion.validators = [
            {
              type: "regex",
              text: `Your answer must be between 1 and ${maxLen} characters.`,
              regex: `^[\\s\\S]{1,${maxLen}}$`,
            },
          ];
        }

        return rtnQuestion;
      }

      default:
        return null;
    }
  }

  // ----------------------------
  // SurveyJS JSON -> Editor
  // Supports multi-page surveys
  // ----------------------------
  fromSurveyJsJson(
    surveyJson: any,
    status: QuestionnaireTemplateEditor["status"]
  ): QuestionnaireTemplateEditor {
    const pages = Array.isArray(surveyJson?.pages) ? surveyJson.pages : [];

    const elements = pages.flatMap((p: any) =>
      Array.isArray(p?.elements) ? p.elements : []
    );

    const questions: TemplateQuestion[] = elements
      .map((el: any) => this.fromSurveyJsElement(el))
      .filter(Boolean) as TemplateQuestion[];

    return {
      title: surveyJson?.title ?? "Untitled template",
      description: surveyJson?.description ?? "",
      status,
      questions,
    };
  }

  private fromSurveyJsElement(el: any): TemplateQuestion | null {
    if (!el?.type) return null;

    if (el.type === "radiogroup") {
      const choices = Array.isArray(el.choices) ? el.choices : [];

      const options = choices
        .map((c: any) => {
          const value = Number(c?.value);
          const label = String(c?.text ?? "");

          if (!Number.isFinite(value)) return null;
          if (!label) return null;

          return { value, label };
        })
        .filter(Boolean);

      const question: RadioGroupQuestion = {
        id: el.name ?? `q_${Math.random().toString(36).slice(2)}`,
        type: QuestionType.RadioGroup,
        prompt: el.title ?? "",
        options: options as any,
        allowOtherComment: !!el.hasOther,
        otherLabel: el.otherText ?? null,
        expanded: false,
      };

      return question;
    }

    return null;
  }
}
